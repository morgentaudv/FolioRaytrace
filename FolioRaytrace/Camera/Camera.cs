using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Camera
{
    /// <summary>
    /// 各オブジェクトのWorld空間での位置や回転の情報を保持する。
    /// </summary>
    public class Transform
    {
        public RayMath.Vector3 Position { get; set; }
        public RayMath.Rotation Rotation { get; set; }
        public RayMath.Quaternion RotationQuat => new RayMath.Quaternion(Rotation);
        public RayMath.Vector3 Scale { get; set; }

        public Transform()
        {
            Position = RayMath.Vector3.s_Zero;
            Rotation = new RayMath.Rotation();
            Scale = RayMath.Vector3.s_One;
        }

        /// <summary>
        /// 
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

    /// <summary>
    /// 基本的なPerspectiveCameraを表す。
    /// </summary>
    public class Camera
    {
        public Camera()
        {
            Transform = new Transform();
            FocalLength = 1.0;
        }

        /// <summary>
        /// カメラEntityの基本的な位置情報
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// カメラの中心からviewport（網膜）までどのぐらいに離れているか
        /// </summary>
        public double FocalLength { get; set; }

        public int ImageWidth
        {
            get => _imageWidth;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                _imageWidth = value;
            }
        }
        public int ImageHeight
        {
            get => _imageHeight;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                _imageHeight = value;
            }
        }

        public double ImageAspectRatio => ImageWidth / (double)ImageHeight;

        public double ViewportHeight
        {
            get => _viewportHeight;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                _viewportHeight = value;
            }
        }
        public double ViewportWidth => ViewportHeight * ImageAspectRatio;

        private int _imageWidth = 640;
        private int _imageHeight = 480;
        private double _viewportHeight = 1.0;
    }
}
