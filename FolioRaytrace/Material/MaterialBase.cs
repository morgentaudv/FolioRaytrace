using FolioRaytrace.RayMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Material
{
    /// <summary>
    /// Objectの表面特性を表す。
    /// </summary>
    public abstract class MaterialBase
    {
        public struct ProceedSetting
        {
            public Vector3 ShapeNormal;
            public Vector3 RayColor;
            public Vector3 RayEnergy;
        }

        public struct ProceedResult
        {
            public ProceedResult()
            {
                IsScattered = false;
            }

            public Vector3 Normal;
            public Vector3 RayColor;
            public Vector3 RayEnergy;
            public bool IsScattered;
        }

        public abstract ProceedResult Proeeed(ref ProceedSetting setting);
    }
}
