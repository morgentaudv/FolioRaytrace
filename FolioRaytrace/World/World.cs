using FolioRaytrace.RayMath;
using FolioRaytrace.SDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.World
{
    /// <summary>
    /// 一つの描画可能な世界を表す。
    /// </summary>
    public class World
    {
        public World() {
            _objects = new List<(object, Material.MaterialBase)> { };
            _globalRng = new Random(Environment.TickCount);
            RefractiveIndex = 1.0;
        }

        public void AddObject(ShapeSphere sphere, Material.MaterialBase material)
        {
            _objects.Add((sphere, material));
        }

        /// <summary>
        /// Worldを描画するための設定構造体
        /// </summary>
        public struct RenderSetting
        {
            public RenderSetting()
            {
                CycleLimitCount = 10;
            }

            /// <summary>
            /// 最初に飛ばすRay
            /// </summary>
            public RayMath.Ray Ray;
            /// <summary>
            /// Rayのバウンシングサイクル。
            /// 指定された数のサイクルを超えるとエネルギーが全部放出されたとみなす。
            /// </summary>
            public uint CycleLimitCount;
        }

        /// <summary>
        /// 描画を行し、設定からの最終的な描画色をoutColorにて返す。
        /// </summary>
        public void Render(out Vector3 outColor, RenderSetting setting)
        {
            outColor = Vector3.s_Zero;
            if (setting.CycleLimitCount <= 0)
            {
                return;
            }

            // 残存エネルギー量。
            //
            // 逆に考えるべき。
            // 現実なら光がエネルギーを持っていてその残存エネルギーから色や明度などが目にわかる。
            var rayEnergy = Vector3.s_One;
            var rayColor = Vector3.s_One;
            var enteredMaterials = new List<Material.MaterialBase>();

            var ray = setting.Ray;
            uint cycleCount = 0;
            while (rayEnergy.LengthSquared > double.Epsilon)
            {
                // まず基本Shapeからの基本情報を持ってくる。この段階ではマテリアルの適用はない。
                HitResult? oFinalResult = null;
                Material.MaterialBase? finalMaterial = null;
                foreach (var (shape, material) in _objects)
                {
                    // このようにtype判別でswitch/case可能。(C# 7.0)
                    // https://qiita.com/toRisouP/items/18b31b024b117009137a#%E5%9E%8B%E3%81%A7switch%E3%81%99%E3%82%8B-c-70
                    switch (shape)
                    {
                    case ShapeSphere sphere:
                    {
                        var oResult = sphere.TryHit(ray, 1e-6, 1000);
                        if (!oResult.HasValue)
                        { continue; }

                        if (!oFinalResult.HasValue)
                        {
                            oFinalResult = oResult;
                            finalMaterial = material;
                            continue;
                        }
                        else if (oResult.Value.ProceedT < oFinalResult.Value.ProceedT)
                        {
                            oFinalResult = oResult.Value;
                            finalMaterial = material;
                        }
                    }   break;
                    default:
                    { throw new UnreachableException(); }
                    }
                }

                // もし図形にどんな形でもぶつかったら、そのぶつかった図形が持つマテリアルを適用する。
                // 拡散、屈折などが適用される…
                // などもしマテリアルによってRayが中に入ったなら、スタックを積む必要もある。
                if (oFinalResult.HasValue)
                {
                    // 24-03-06 Diffuseを実装するためにNormalから半球範囲の法線を無作為取得する。
                    var result = oFinalResult.Value;

                    // もっとそれっぽくMicrofacetのNormalを計算する。
                    var matSetting = new Material.MaterialBase.ProceedSetting();
                    matSetting.RayEnergy = rayEnergy;
                    matSetting.RayColor = rayColor;
                    matSetting.RayDirection = ray.Direction;
                    matSetting.ShapeNormal = result.Normal;

                    matSetting.NowRefractiveIndex = RefractiveIndex; // 現在の屈折率
                    matSetting.IsInternal = false;
                    if (enteredMaterials.Count != 0)
                    {
                        matSetting.IsInternal = true;
                        //matSetting.ShapeNormal = result.Normal * -1; 

                        switch (enteredMaterials.Last())
                        {
                        case Material.BasicDielectric mat:
                        {
                            matSetting.NowRefractiveIndex = mat.RefractiveIndex;
                        }
                        break;
                        default:
                        { }
                        break;
                        }
                    }

                    matSetting.PrevRefractiveIndex = RefractiveIndex;
                    if (enteredMaterials.Count >= 2)
                    {
                        switch (enteredMaterials.ElementAt(enteredMaterials.Count - 2))
                        {
                        case Material.BasicDielectric mat:
                        {
                            matSetting.PrevRefractiveIndex = mat.RefractiveIndex;
                        }
                        break;
                        default:
                        { }
                        break;
                        }
                    }

                    // エネルギー減衰
                    var matResult = finalMaterial!.Proeeed(ref matSetting);
                    rayEnergy = matResult.RayEnergy;
                    rayColor = matResult.RayColor;

                    var oLastMaterial = enteredMaterials.LastOrDefault();
                    if (matResult.IsEntered)
                    {
                        // 24-03-07 もし中に入ったらストックする。
                        // ただ現在処理しているマテリアルがストックしているときの最後のものかを判定し
                        // 異なったらストックする。
                        if (oLastMaterial == null)
                        {
                            enteredMaterials.Add(finalMaterial!);
                        }
                        else if (!ReferenceEquals(oLastMaterial, finalMaterial))
                        {
                            enteredMaterials.Add(finalMaterial!);
                        }

                    }
                    else
                    {
                        // 抜けるのであれば、自分と同じであればストックを解除。
                        if (ReferenceEquals(oLastMaterial, finalMaterial))
                        {
                            enteredMaterials.RemoveAt(enteredMaterials.Count - 1);
                        }
                    }

                    // ほんの少し前進させる。じゃないとRayの出発点が中心に埋められることがある。
                    ray = new Ray(ray.Proceed(result.ProceedT), matResult.RayDirection)
                        .ProceedRay(1e-5);

                    cycleCount += 1;
                    if (cycleCount < setting.CycleLimitCount)
                    {
                        continue;
                    }

                    // エネルギーが全部減衰されたとみなす。
                }

                // 光（抗原）に対して色着せ
                outColor = rayEnergy * rayColor * GetBackgroundColor(ray);
                break;
            }
        }

        private static Vector3 GetBackgroundColor(Ray ray)
        {
            var direction = ray.Direction;
            var a = 0.5 * (direction.Y + 1.0); // [0, 1]に収束
            return (1.0 - a) * Vector3.s_One + (a * new Vector3(0.5, 0.7, 1.0));
        }

        /// <summary>
        /// Worldの基本屈折率を表す。基本1.0
        /// </summary>
        public double RefractiveIndex { get; set; }

        private List<(object, Material.MaterialBase)> _objects;
        private Random _globalRng;
    }
}
