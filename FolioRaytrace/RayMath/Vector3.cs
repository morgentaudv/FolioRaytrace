using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.RayMath
{
    /// <summary>
    /// xyzの3つの要素を保持するVectorクラス
    /// </summary>
    public struct Vector3
    {
        static public Vector3 s_Zero = new Vector3(0);
        static public Vector3 s_One = new Vector3(1);
        static public Vector3 s_UnitX = new Vector3(1, 0, 0);
        static public Vector3 s_UnitY = new Vector3(0, 1, 0);
        static public Vector3 s_UnitZ = new Vector3(0, 0, 1);

        public Vector3() { }
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(double s)
        {
            X = s;
            Y = s;
            Z = s;
        }

        public double X { get => _x; set => _x = value; }
        public double Y { get => _y; set => _y = value; }
        public double Z { get => _z; set => _z = value; }

        public double Length => System.Math.Sqrt(LengthSquared);
        public double LengthSquared => X * X + Y * Y + Z * Z;

        /// <summary>
        /// 一番大きい値を持つ要素のインデックスを返す。
        /// 0ならX、1はY、そして2はZを指す。
        /// もし各要素の値が数値演算できるものでなければ、例外を投げる。
        /// </summary>
        public int MaxElementI
        {
            get
            {
                if (IsAnyInvalid)
                {
                    throw new InvalidDataException();
                }

                int i = -1;
                double maxValue = double.MinValue;
                if (X > maxValue)
                {
                    i = 0;
                    maxValue = X;
                }
                if (Y > maxValue)
                {
                    i = 1;
                    maxValue = Y;
                }
                if (Z > maxValue)
                {
                    i = 2;
                    maxValue = Z;
                }
                return i;
            }
        }

        /// <summary>
        /// XYZ要素が全部計算可能な値なのか？
        /// </summary>
        public bool IsAnyInvalid => !double.IsFinite(X) || !double.IsFinite(Y) || !double.IsFinite(Z);

        /// <summary>
        /// XYZ要素の一つでも負の数を持っているか？
        /// </summary>
        public bool IsAnyNegative => double.IsNegative(X) || double.IsNegative(Y) || double.IsNegative(Z);

        /// <summary>
        /// 内積を行い、あたらしい値を返す。
        /// </summary>
        public double Dot(Vector3 v) => X * v.X + Y * v.Y + Z * v.Z;

        /// <summary>
        /// 外積を行い、あたらしい値を返す。
        /// </summary>
        public Vector3 Cross(Vector3 v)
        {
            return new Vector3(
                (Y * v.Z) - (Z * v.Y),
                (Z * v.X) - (X * v.Z),
                (X * v.Y) - (Y * v.X)
            );
        }

        /// <summary>
        /// Lengthが0ではないときに単位ベクトルを返す。
        /// </summary>
        /// <exception cref="DivideByZeroException">Lengthが0なら発生</exception>
        public Vector3 Unit()
        {
            var length = Length;
            if (length <= 0)
            {
                throw new DivideByZeroException();
            }
            return this / Length;
        }
        /// <summary>
        /// Lengthが0ではないときに単位ベクトルを返す。
        /// </summary>
        /// <exception cref="DivideByZeroException">Lengthが0なら発生</exception>
        public Vector3 Normalize() => Unit();

        /// <summary>
        /// 各要素同士に比較し小さい値を新しいVector3として返す。
        /// </summary>
        public Vector3 ElementMin(Vector3 v) => new Vector3(Math.Min(X, v.X), Math.Min(Y, v.Y), Math.Min(Z, v.Z));

        /// <summary>
        /// 各要素同士に比較し大きい値を新しいVector3として返す。
        /// </summary>
        public Vector3 ElementMax(Vector3 v) => new Vector3(Math.Max(X, v.X), Math.Max(Y, v.Y), Math.Max(Z, v.Z));

        public double this[int i]
        {
            get 
            {
                return i switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new IndexOutOfRangeException(),
                };
            }

            set
            {
                switch (i)
                {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                case 2:
                    Z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
                };
            }
        }

        public static Vector3 operator +(Vector3 v) => v;
        public static Vector3 operator -(Vector3 v) => new Vector3(-v.X, -v.Y, -v.Z);
        public static Vector3 operator +(Vector3 l, Vector3 r) => new Vector3(l.X + r.X, l.Y + r.Y, l.Z + r.Z);
        public static Vector3 operator -(Vector3 l, Vector3 r) => new Vector3(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
        public static Vector3 operator *(Vector3 v, double scalar) => new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        public static Vector3 operator *(double scalar, Vector3 v) => v * scalar;

        /// <summary>
        /// この場合、各要素だけ掛け算される。内積とは違う。
        /// </summary>
        public static Vector3 operator *(Vector3 l, Vector3 r) => new Vector3(l.X * r.X, l.Y * r.Y, l.Z * r.Z);
        public static Vector3 operator /(Vector3 v, double scalar)
        {
            if (scalar == 0)
            {
                throw new DivideByZeroException();
            }

            return new Vector3(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }
        public static Vector3 operator /(double scalar, Vector3 v) => new Vector3();

        public override string ToString()
        {
            return $"({X:F5}, {Y:F5}, {Z:F5})";
        }

        private double _x = 0;
        private double _y = 0;
        private double _z = 0;
    }
}
