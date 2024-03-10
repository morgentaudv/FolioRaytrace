using System;
using System.Collections.Generic;

namespace FolioRaytrace.RayMath
{
    /// <summary>
    /// 各オブジェクトのWorld空間での位置や回転の情報を保持する。
    /// </summary>
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Rotation Rotation { get; set; }
        public Quaternion RotationQuat => new RayMath.Quaternion(Rotation);
        public Vector3 Scale { get; set; }

        public Transform()
        {
            Position = RayMath.Vector3.s_Zero;
            Rotation = new RayMath.Rotation();
            Scale = RayMath.Vector3.s_One;
        }

        /// <summary>
        /// fromを原点にし、to方向を見るようにTransformを作る。
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        static public Transform FromLookAt(RayMath.Vector3 from, RayMath.Vector3 to)
        {
            if ((to - from).LengthSquared < double.Epsilon)
            {
                throw new Exception("from and to must not be same position.");
            }

            var coordinates = RayMath.Coordinates.FromAxisZ(to - from);
            var rotation = RayMath.Rotation.FromCoordinates(coordinates)!.First();

            var result = new Transform();
            result.Position = from;
            result.Rotation = rotation;
            result.Scale = RayMath.Vector3.s_One;
            return result;
        }
    }

}
