using FolioRaytrace.RayMath;
using FolioRaytrace.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Material
{
    public class BasicDiffuse : MaterialBase
    {
        public BasicDiffuse() 
        {
            Albedo = new Texture.SolidColor(RayMath.Vector3.s_Zero);
            AttenuationColor = Vector3.s_One * 1.0;

            lock(_lockRng)
            {
                _rng = new Random(Environment.TickCount);
            }
        }

        public override ProceedResult Proeeed(ref ProceedSetting setting) 
        {
            // もっとそれっぽくMicrofacetのNormalを計算する。
            var shapeNormal = setting.HitResult.ShapeNormal;
            var proceedT = setting.HitResult.ProceedT;
            var coordinates = Coordinates.FromAxisY(shapeNormal);

            double rngValue;
            lock (_lockRng)
            {
                rngValue = _rng.NextDouble();
            }

            var xAxisAngle = rngValue * RoughnessMaxAngle;
            var xAxisQuat = new Quaternion(coordinates.XAxis, xAxisAngle, EAngleUnit.Degrees);

            lock (_lockRng)
            {
                rngValue = _rng.NextDouble();
            }

            var yAxisAngle = rngValue * 360.0;
            var yAxisQuat = new Quaternion(shapeNormal, yAxisAngle, EAngleUnit.Degrees);
            var newNormal = yAxisQuat.Rotate(xAxisQuat.Rotate(shapeNormal));
            var rayEnergy = setting.RayEnergy * AttenuationColor;
            var outRay = new Ray(setting.Ray.Proceed(proceedT), newNormal);

            Vector3 rayColor;
            {
                ITextureBase.ValueSetting value;
                value.U = setting.HitResult.TextureU;
                value.V = setting.HitResult.TextureV;
                value.Point = outRay.Orig;
                rayColor = setting.RayColor * Albedo.Value(value);
            }

            // 計算完了。
            var result = new ProceedResult();
            result.Ray = outRay;
            result.RayEnergy = rayEnergy;
            result.RayColor = rayColor;
            result.IsEntered = false;
            return result;
        }

        /// <summary>
        /// マテリアルの色構成となるテクスチャー
        /// </summary>
        public Texture.ITextureBase Albedo { get; set; }

        /// <summary>
        /// 光の減衰計算で使われる。基本完全減衰
        /// </summary>
        public Vector3 AttenuationColor { get; set; }

        public double Roughness
        {
            get => _roughness;
            set => _roughness = Math.Clamp(value, 0, 1);
        }

        private double RoughnessMaxAngle => _roughness * 90.0;

        private double _roughness = 1.0;

        private readonly object _lockRng = new(); 
        private Random _rng;
    }
}
