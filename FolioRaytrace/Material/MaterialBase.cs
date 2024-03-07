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
    public class MaterialBase
    {
        public MaterialBase()
        {
            _Rng = new Random(Environment.TickCount);
        }

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

        public ProceedResult Proeeed(ref ProceedSetting setting)
        {
            // もっとそれっぽくMicrofacetのNormalを計算する。
            var coordinates = Coordinates.FromAxisY(setting.ShapeNormal);
            var xAxisAngle = _Rng.NextDouble() * RoughnessMaxAngle;
            var xAxisQuat = new Quaternion(coordinates.XAxis, xAxisAngle, EAngleUnit.Degrees);
            var yAxisAngle = _Rng.NextDouble() * 360.0;
            var yAxisQuat = new Quaternion(setting.ShapeNormal, yAxisAngle, EAngleUnit.Degrees);
            var newNormal = yAxisQuat.Rotate(xAxisQuat.Rotate(setting.ShapeNormal));

            var rayEnergy = setting.RayEnergy * AttenuationColor;
            var rayColor = setting.RayColor * Albedo;

            var result = new ProceedResult();
            result.RayEnergy = rayEnergy;
            result.RayColor = rayColor;
            result.Normal = newNormal;
            result.IsScattered = true;
            return result;
        }

        /// <summary>
        /// マテリアルのベースとなる色
        /// </summary>
        public Vector3 Albedo { get; set; }

        /// <summary>
        /// 光の減衰計算で使われる
        /// </summary>
        public Vector3 AttenuationColor { get; set; }

        public double Roughness
        {
            get => _roughness;
            set => _roughness = Math.Clamp(value, 0, 1);
        }

        private double RoughnessMaxAngle => _roughness * 90.0;

        private double _roughness = 1.0;
        private Random _Rng;
    }
}
