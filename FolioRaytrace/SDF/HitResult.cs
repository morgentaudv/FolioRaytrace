using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolioRaytrace.RayMath;

namespace FolioRaytrace.SDF
{
    /// <summary>
    /// 図形にヒットしたときの結果情報を保持する。
    /// </summary>
    public struct HitResult
    {
        public Vector3 Point;
        /// <summary>
        /// ヒットした表面の法線。ここで返された法線は全部外部に進む法線を指す。
        /// </summary>
        public Vector3 Normal;
        public double ProceedT;
    }
}
