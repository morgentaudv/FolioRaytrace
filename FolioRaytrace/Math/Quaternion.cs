using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.RayMath
{
    /// <summary>
    /// 回転量を表す構造体。
    /// </summary>
    public struct Quaternion
    {
        public static Quaternion s_Identity = new Quaternion();

        public Quaternion() { }
        public Quaternion(double w, double x, double y, double z)
        {
            _w = w;
            _x = x;
            _y = y;
            _z = z;
            ApplyNormalize();
        }
        public Quaternion(Rotation v)
        {
            // 3-2-1 (Z-Y-X) 順で変換していく。
            // Heading (Z) - Psi
            // Pitch (Y) - Theta
            // Bank (X) - Phi

            var cz = System.Math.Cos(v.RadZ * 0.5);
            var sz = System.Math.Sin(v.RadZ * 0.5);
            var cy = System.Math.Cos(v.RadY * 0.5);
            var sy = System.Math.Sin(v.RadY * 0.5);
            var cx = System.Math.Cos(v.RadX * 0.5);
            var sx = System.Math.Sin(v.RadX * 0.5);

            _w = (cx * cy * cz) + (sx * sy * sz);
            _x = (sx * cy * cz) - (cx * sy * sz);
            _y = (cx * sy * cz) + (sx * cy * sz);
            _z = (cx * cy * sz) - (sx * sy * cz);
        }

        public double Length => System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public Quaternion Normalize()
        {
            var length = Length;
            if (length <= 0)
            {
                throw new DivideByZeroException();
            }
            return new Quaternion(X / length, Y / length, Z / length, W / length);
        }
        public void ApplyNormalize()
        {
            var length = Length;
            if (length <= 0)
            {
                throw new DivideByZeroException();
            }

            _w /= length;
            _x /= length;
            _y /= length;
            _z /= length;
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        /// <summary>
        /// vを回転して返す。
        /// </summary>
        /// <param name="v">位置を示す</param>
        /// <returns>Quaternionによって回転されたベクトル</returns>
        public Vector3 Rotate(Vector3 v)
        {
            // https://gamedev.stackexchange.com/questions/28395/rotating-vector3-by-a-quaternion
            var u = new Vector3(X, Y, Z);
            var s = W;

            var v1 = 2.0 * u.Dot(v) * u;
            var v2 = (s * s - u.Dot(u)) * v;
            var v3 = 2.0 * s * u.Cross(v);
            return v1 + v2 + v3;
        }

        public double W { get => _w; set => _w = value; }
        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Z { get => _z; set => _z = value; }

        private double _w = 1;
        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
    }
}
