using FolioRaytrace.RayMath;
using FolioRaytrace.RayMath.SDF;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void AddObject(RayMath.SDF.Sphere sphere)
        {
            _objects.Add(sphere);
        }

        public void Render(out Vector3 outColor, RayMath.Ray inRay)
        {
            outColor = Vector3.s_Zero;

            // 残存エネルギー量。
            //
            // 逆に考えるべき。
            // 現実なら光がエネルギーを持っていてその残存エネルギーから色や明度などが目にわかる。
            double energy = 1.0;
            Ray ray = inRay;
            while (energy > double.Epsilon)
            {
                RayMath.SDF.HitResult? oFinalResult = null;
                foreach (var shape in _objects)
                {
                    if (shape is Sphere)
                    {
                        var oResult = ((Sphere)shape).TryHit(ray, 0, 100);
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
                    var result = oFinalResult.Value;

                    // エネルギー減衰
                    energy *= 0.5;
                    // ほんの少し前進させる。じゃないとRayの出発点が中心に埋められることがある。
                    ray = new Ray(inRay.Proceed(result.ProceedT), result.Normal)
                        .ProceedRay(1e-5);
                    continue;
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
    }
}
