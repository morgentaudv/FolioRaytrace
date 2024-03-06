using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.RayMath
{
    /// <summary>
    /// 座標系を表す。
    /// </summary>
    public struct Coordinates
    {
        public Coordinates()
        {
            _xAxis = Vector3.s_UnitX;
            _yAxis = Vector3.s_UnitY;
            _zAxis = Vector3.s_UnitZ;
        }

        private Coordinates(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            _xAxis = xAxis;
            _yAxis = yAxis;
            _zAxis = zAxis;
        }

        public static Coordinates s_Default = new Coordinates();

        /// <summary>
        /// 入力のvを生成する座標系のY軸としてみなして座標系を生成する。
        /// </summary>
        /// <exception cref="Exception">入力したv全体の長さが0に近いと起きる</exception>
        public static Coordinates FromAxisY(Vector3 v)
        {
            if (v.LengthSquared < double.Epsilon)
            {
                throw new Exception("Given v length must not be 0.");
            }

            // もしvがWorld空間のY軸と同じであれば、デフォルトを返す。
            var yAxis = v.Unit();
            var xAxis = Vector3.s_UnitY.Cross(yAxis);
            if (xAxis.LengthSquared < double.Epsilon)
            {
                return s_Default;
            }
            var zAxis = xAxis.Cross(yAxis);
            return new Coordinates(xAxis, yAxis, zAxis);
        }

        public Vector3 XAxis => _xAxis;
        public Vector3 YAxis => _yAxis;
        public Vector3 ZAxis => _zAxis;

        private Vector3 _xAxis;
        private Vector3 _yAxis;
        private Vector3 _zAxis;

    }
}
