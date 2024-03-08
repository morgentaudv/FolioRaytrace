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

        public static List<Rotation> FromCoordinates(Coordinates coords)
        {
            const int k_DECIMAL = 5;

            // https://eecs.qmul.ac.uk/~gslabaugh/publications/euler.pdf
            // Coordinates自体を一種の3x3マトリックスとして考えられる。
            // ただ、上の式は間違っているので（Yの部分がc-sscではない）注意すること。
            var r1 = coords.XAxis;
            var r2 = coords.YAxis;
            var r3 = coords.ZAxis;

            // 下のロジックでAtan2に-をつける理由？
            // このプロジェクトの回転は+方向が逆時計まわりなので、CSharpで提供しているものとは逆方向になる。
            var results = new List<Rotation>();

            var r31 = Math.Round(r3.X, k_DECIMAL);
            if (Math.Abs(r31) != 1)
            {
                // 2つの角度になれる。
                // r31自体はY角度を求めるときに使って、そのY角度が90度（-90度）じゃないことを示す。
                var radY0 = Math.Asin(r31);
                var radY1 = Math.PI - radY0;
                var cosRadY0 = Math.Cos(radY0);
                var cosRadY1 = Math.Cos(radY1);

                var r32 = Math.Round(r3.Y, k_DECIMAL);
                var r33 = Math.Round(r3.Z, k_DECIMAL);
                var radX0 = -Math.Atan2(r32 / cosRadY0, r33 / cosRadY0);
                var radX1 = -Math.Atan2(r32 / cosRadY1, r33 / cosRadY1);

                var r21 = Math.Round(r2.X, k_DECIMAL);
                var r11 = Math.Round(r1.X, k_DECIMAL);
                var radZ0 = -Math.Atan2(r21 / cosRadY0, r11 / cosRadY0);
                var radZ1 = -Math.Atan2(r21 / cosRadY1, r11 / cosRadY1);

                results.Add(new Rotation(radX0, radY0, radZ0, EAngleUnit.Radians));
                results.Add(new Rotation(radX1, radY1, radZ1, EAngleUnit.Radians));
            }
            else
            {
                // 角度は一つになれる（ジンバルロック状態？）
                var radX = 0.0;
                var radY = 0.0;
                var radZ = 0.0;

                var r12 = Math.Round(r1.Y, k_DECIMAL);
                var r13 = Math.Round(r1.Z, k_DECIMAL);

                if (r31 == -1)
                {
                    radY = Math.PI * 0.5;
                    radX = -Math.Atan2(r12, r13);
                }
                else
                {
                    radY = Math.PI * -0.5;
                    radX = -Math.Atan2(-r12, -r13);
                }

                results.Add(new Rotation(radX, radY, radZ, EAngleUnit.Radians));
            }

            return results;
        }

        public double RadX { get => _x; set => _x = value; }
        public double RadY { get => _y; set => _y = value; }
        public double RadZ { get => _z; set => _z = value; }

        public double DegX { get => _x * k_ToDegrees; set => _x = value * k_ToRadians; }
        public double DegY { get => _y * k_ToDegrees; set => _y = value * k_ToRadians; }
        public double DegZ { get => _z * k_ToDegrees; set => _z = value * k_ToRadians; }

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

        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
    }
}
