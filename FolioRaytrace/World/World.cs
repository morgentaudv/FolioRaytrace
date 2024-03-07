using FolioRaytrace.RayMath;
using FolioRaytrace.SDF;
using System;
using System.Collections.Generic;
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
            _objects = new List<(object, Material)> { };
            _globalRng = new Random(Environment.TickCount);
        }

        public void AddObject(ShapeSphere sphere, Material material)
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

            var ray = setting.Ray;
            uint cycleCount = 0;
            while (rayEnergy.LengthSquared > double.Epsilon)
            {
                HitResult? oFinalResult = null;
                Material? finalMaterial = null;
                foreach (var (shape, material) in _objects)
                {
                    if (shape is ShapeSphere)
                    {
                        var oResult = ((ShapeSphere)shape).TryHit(ray, 1e-5, 1000);
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
                    }
                }

                if (oFinalResult.HasValue)
                {
                    // 24-03-06 Diffuseを実装するためにNormalから半球範囲の法線を無作為取得する。
                    var result = oFinalResult.Value;

                    // もっとそれっぽくMicrofacetのNormalを計算する。
                    var matSetting = new Material.ProceedSetting();
                    matSetting.RayEnergy = rayEnergy;
                    matSetting.RayColor = rayColor;
                    matSetting.ShapeNormal = result.Normal;

                    // エネルギー減衰
                    var matResult = finalMaterial!.Proeeed(ref matSetting);
                    rayEnergy = matResult.RayEnergy;
                    rayColor = matResult.RayColor;

                    // ほんの少し前進させる。じゃないとRayの出発点が中心に埋められることがある。
                    ray = new Ray(ray.Proceed(result.ProceedT), matResult.Normal);

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

        private List<(object, Material)> _objects;
        private Random _globalRng;
    }
}
