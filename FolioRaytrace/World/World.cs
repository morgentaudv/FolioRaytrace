using FolioRaytrace.RayMath;
using FolioRaytrace.RayMath.SDF;
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
            _objects = new List<object> { };
            _globalRng = new Random(Environment.TickCount);
        }

        public void AddObject(RayMath.SDF.Sphere sphere)
        {
            _objects.Add(sphere);
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
            double energy = 1.0;
            var ray = setting.Ray;
            uint cycleCount = 0;
            while (energy > double.Epsilon)
            {
                RayMath.SDF.HitResult? oFinalResult = null;
                foreach (var shape in _objects)
                {
                    if (shape is Sphere)
                    {
                        var oResult = ((Sphere)shape).TryHit(ray, 1e-5, 1000);
                        if (!oResult.HasValue)
                        { continue; }

                        if (!oFinalResult.HasValue)
                        {
                            oFinalResult = oResult;
                            continue;
                        }
                        else if (oResult.Value.ProceedT < oFinalResult.Value.ProceedT)
                        {
                            oFinalResult = oResult.Value;
                        }
                    }
                }

                if (oFinalResult.HasValue)
                {
                    // 24-03-06 Diffuseを実装するためにNormalから半球範囲の法線を無作為取得する。
                    var result = oFinalResult.Value;

                    // もっとそれっぽくMicrofacetのNormalを計算する。
                    const double k_DEG_ANGLE = 45.0;
                    var coordinates = Coordinates.FromAxisY(result.Normal);
                    var xAxisAngle = _globalRng.NextDouble() * k_DEG_ANGLE;
                    var xAxisQuat = new Quaternion(coordinates.XAxis, xAxisAngle, EAngleUnit.Degrees);
                    var yAxisAngle = _globalRng.NextDouble() * 360.0;
                    var yAxisQuat = new Quaternion(result.Normal, yAxisAngle, EAngleUnit.Degrees);
                    var newNormal = yAxisQuat.Rotate(xAxisQuat.Rotate(result.Normal));

                    // エネルギー減衰
                    energy *= 0.5;
                    // ほんの少し前進させる。じゃないとRayの出発点が中心に埋められることがある。
                    ray = new Ray(ray.Proceed(result.ProceedT), newNormal);

                    cycleCount += 1;
                    if (cycleCount < setting.CycleLimitCount)
                    {
                        continue;
                    }

                    // エネルギーが全部減衰されたとみなす。
                }

                // 光（抗原）に対して色着せ
                outColor = energy * GetBackgroundColor(ray);
                break;
            }
        }

        private static Vector3 GetBackgroundColor(Ray ray)
        {
            var direction = ray.Direction;
            var a = 0.5 * (direction.Y + 1.0); // [0, 1]に収束
            return (1.0 - a) * Vector3.s_One + (a * new Vector3(0.5, 0.7, 1.0));
        }

        private List<object> _objects;
        private Random _globalRng;
    }
}
