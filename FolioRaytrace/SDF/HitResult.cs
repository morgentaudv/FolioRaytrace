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
        /// ヒットした表面の法線。内部からか外部からかはFrontFaceを参考すること。
        /// </summary>
        public Vector3 Normal;
        public double ProceedT;
        /// <summary>
        /// もし外部からヒットしているのであればtrueになる。
        /// </summary>
        public bool IsInternal;
    }
}
