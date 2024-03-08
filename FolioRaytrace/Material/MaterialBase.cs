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
            public ProceedSetting()
            {
                IsInternal = false;
                NowRefractiveIndex = 0;
            }

            /// <summary>
            /// 図形からの法線。内部法線か外部法線かはIsInternalを確認すべき。
            /// </summary>
            public Vector3 ShapeNormal;
            public Vector3 RayColor;
            public Vector3 RayDirection;
            public Vector3 RayEnergy;
            public bool IsInternal;
            public double NowRefractiveIndex;
        }

        public struct ProceedResult
        {
            public ProceedResult()
            {
                IsEntered = false;
            }

            public Vector3 RayDirection;
            public Vector3 RayColor;
            public Vector3 RayEnergy;
            public bool IsEntered;
        }

        public abstract ProceedResult Proeeed(ref ProceedSetting setting);
    }
}
