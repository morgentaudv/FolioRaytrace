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
            var rayDirection = setting.RayDirection;

            var n = setting.ShapeNormal;
            var taiOverTat = setting.NowRefractiveIndex;
            if (!setting.IsInternal)
            {
                // 中に入る
                // tai = n, tat = n'で、既存屈折率/新規屈折率
                taiOverTat = setting.NowRefractiveIndex / RefractiveIndex;
            }
            else
            {
                // 外に出る
                // 上の逆。
                taiOverTat = 1.0 / setting.NowRefractiveIndex;
            }

            // 新規Ray方向はprep + perpendicular (perp) である。
            {
                var r = setting.RayDirection;
                var prep = taiOverTat * (r + (((r * -1).Dot(n)) * n));
                var perp = -1 * Math.Sqrt(1.0 - prep.Dot(prep)) * n;
                rayDirection = (prep + perp).Normalize();
            }

            // 計算完了。
            var result = new ProceedResult();
            result.RayEnergy = rayEnergy;
            result.RayColor = rayColor;
            result.RayDirection = rayDirection;
            result.IsEntered = !setting.IsInternal;
            return result;
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
