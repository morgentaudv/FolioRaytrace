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

        /// <summary>
        /// ここではスネルの法則を使って屈折や全反射を実装する。（ややこしい）
        /// </summary>
        public override ProceedResult Proeeed(ref ProceedSetting setting)
        {
            var n = setting.ShapeNormal;
            if (setting.IsInternal)
            {
                // 反転する。
                n *= -1;
            }

            double r;
            if (!setting.IsInternal)
            {
                // 中に入る
                // n1 / n2、既存屈折率/新規屈折率
                r = setting.NowRefractiveIndex / RefractiveIndex;
            }
            else
            {
                // 外に出る
                // 上の逆。n2 / n1なので計算に注意。
                r = RefractiveIndex / setting.PrevRefractiveIndex;
            }

            // 新規Ray方向はprep + perpendicular (perp) である。
            Vector3 rayDirection;
            bool isTotalReflection = false;
            {
                var l = setting.RayDirection;
                var cost0 = (l * -1).Dot(n); // 必ずPositiveになるべき。
                if (cost0 < 0)
                {
                    throw new Exception("cos0 must be 0 or positive.");
                }
                var c = cost0;
                var sint0 = Math.Sqrt(1 - Math.Pow(c, 2));
                if (r * sint0 > 1.0)
                {
                    // 全反射が起きる。
                    rayDirection = l + (2 * cost0 * n);
                    isTotalReflection = true;
                }
                else
                {
                    // 屈折する
                    var v0 = r * l;
                    var v1sqrt = 1.0 - (Math.Pow(r, 2) * (1 - Math.Pow(c, 2)));
                    var v1 = ((r * c) - Math.Sqrt(v1sqrt)) * n;
                    rayDirection = v0 + v1;
                }
            }

            // 計算完了。
            var result = new ProceedResult();
            result.RayEnergy = setting.RayEnergy * AttenuationColor;
            result.RayColor = setting.RayColor * Albedo;
            result.RayDirection = rayDirection;

            if (isTotalReflection)
            {
                result.IsEntered = setting.IsInternal;
            }
            else
            {
                result.IsEntered = !setting.IsInternal;
            }

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
