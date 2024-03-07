using FolioRaytrace.RayMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Material
{
    /// <summary>
    /// Diffuseではない、中が通れる非金属系の基本マテリアル
    /// </summary>
    internal class BasicDielectric : MaterialBase
    {
        public BasicDielectric()
        {
            RefractiveIndex = 1.0;
        }

        public override ProceedResult Proeeed(ref ProceedSetting setting)
        {
            var rayEnergy = setting.RayEnergy * AttenuationColor;
            var rayColor = setting.RayColor * Albedo;


            throw new NotImplementedException();
        }

        /// <summary>
        /// マテリアルのベースとなる色。基本黒
        /// </summary>
        public Vector3 Albedo { get; set; }

        /// <summary>
        /// 光の減衰計算で使われる。基本完全減衰
        /// </summary>
        public Vector3 AttenuationColor { get; set; }

        /// <summary>
        /// 基本屈折率を表す。基本1.0
        /// </summary>
        public double RefractiveIndex { get; set; }
    }
}
