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

            var yAxis = v.Unit();
            var zAxis = Vector3.s_UnitX.Cross(yAxis);
            if (zAxis.LengthSquared < double.Epsilon)
            {
                zAxis = Vector3.s_UnitZ;
            }
            else
            {
                zAxis = zAxis.Normalize();
            }
            var xAxis = yAxis.Cross(zAxis);

            return new Coordinates(xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// 入力のvを生成する座標系のZ軸としてみなして座標系を生成する。
        /// </summary>
        /// <exception cref="Exception">入力したv全体の長さが0に近いと発生</exception>
        public static Coordinates FromAxisZ(Vector3 v)
        {
            if (v.LengthSquared < double.Epsilon)
            {
                throw new Exception("Given v length must not be 0.");
            }

            var zAxis = v.Unit();
            var xAxis = Vector3.s_UnitY.Cross(zAxis);
            if (xAxis.LengthSquared < double.Epsilon)
            {
                xAxis = Vector3.s_UnitX;
            }
            else
            {
                xAxis = xAxis.Normalize();
            }
            var yAxis = zAxis.Cross(xAxis);

            return new Coordinates(xAxis, yAxis, zAxis);
        }

        public static Coordinates FromRotation(Rotation rot)
        {
            var quat = new Quaternion(rot);
            var axisX = quat.Rotate(Vector3.s_UnitX);
            var axisY = quat.Rotate(Vector3.s_UnitY);
            var axisZ = quat.Rotate(Vector3.s_UnitZ);
            return new Coordinates(axisX, axisY, axisZ);
        }

        public Vector3 XAxis => _xAxis;
        public Vector3 YAxis => _yAxis;
        public Vector3 ZAxis => _zAxis;

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

        private Vector3 _xAxis;
        private Vector3 _yAxis;
        private Vector3 _zAxis;

    }
}
