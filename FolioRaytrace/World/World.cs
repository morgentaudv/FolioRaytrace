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

        public void Render(out Vector3 outColor, RayMath.Ray ray)
        {
            Vector3 color = Vector3.s_Zero;

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
                    }
                    else if (oResult.Value.ProceedT < oFinalResult.Value.ProceedT)
                    {
                        oFinalResult = oResult.Value;
                    }
                }
            }

            if (oFinalResult.HasValue)
            {
                color += (oFinalResult.Value.Normal + Vector3.s_One) * 0.5;
            }
            else
            {
                color += Utility.GetBackgroundColor(ray);
            }

            outColor = color;
        }

        private List<object> _objects;
    }
}
