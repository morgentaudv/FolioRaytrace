using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.RayMath
{
    /// <summary>
    /// 3つのオイラー角を示す回転構造体。ジンバルロックあり。
    /// 中にはRadianとして保持している。
    /// </summary>
    public struct Rotation
    {
        public static double k_ToDegrees = 180.0 / System.Math.PI;
        public static double k_ToRadians = System.Math.PI / 180.0;

        public Rotation() { }
        public Rotation(double x, double y, double z, EAngleUnit unit)
        {
            switch (unit)
            {
            case EAngleUnit.Degrees:
            {
                _x = x * k_ToRadians;
                _y = y * k_ToRadians;
                _z = z * k_ToRadians;
            }
            break;
            case EAngleUnit.Radians:
            {
                _x = x;
                _y = y;
                _z = z;
            }
            break;
            }
        }

        public double RadX { get => _x; set => _x = value; }
        public double RadY { get => _y; set => _y = value; }
        public double RadZ { get => _z; set => _z = value; }

        public double DegX { get => _x * k_ToDegrees; set => _x = value * k_ToRadians; }
        public double DegY { get => _y * k_ToDegrees; set => _y = value * k_ToRadians; }
        public double DegZ { get => _z * k_ToDegrees; set => _z = value * k_ToRadians; }

        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
    }
}
