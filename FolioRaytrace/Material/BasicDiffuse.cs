using FolioRaytrace.RayMath;
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
            Albedo = Vector3.s_Zero;
            AttenuationColor = Vector3.s_One * 1.0;

            lock(_lockRng)
            {
                _rng = new Random(Environment.TickCount);
            }
        }

        public override ProceedResult Proeeed(ref ProceedSetting setting) 
        {
            // もっとそれっぽくMicrofacetのNormalを計算する。
            var coordinates = Coordinates.FromAxisY(setting.ShapeNormal);

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
            var yAxisQuat = new Quaternion(setting.ShapeNormal, yAxisAngle, EAngleUnit.Degrees);
            var newNormal = yAxisQuat.Rotate(xAxisQuat.Rotate(setting.ShapeNormal));

            var rayEnergy = setting.RayEnergy * AttenuationColor;
            var rayColor = setting.RayColor * Albedo;

            // 計算完了。
            var result = new ProceedResult();
            result.RayEnergy = rayEnergy;
            result.RayColor = rayColor;
            result.RayDirection = newNormal;
            result.IsEntered = false;
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
