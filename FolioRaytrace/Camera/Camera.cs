using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Camera
{
    /// <summary>
    /// 基本的なPerspectiveCameraを表す。
    /// </summary>
    public class Camera
    {
        public Camera()
        {
            Transform = new RayMath.Transform();
            FocusDistance = 1.0;
        }

        /// <summary>
        /// カメラEntityの基本的な位置情報
        /// </summary>
        public RayMath.Transform Transform { get; set; }
        /// <summary>
        /// カメラの中心からviewport（網膜）までどのぐらいに離れているか
        /// </summary>
        public double FocusDistance { get; set; }
        /// <summary>
        /// DepthOfFieldの描画に必要。0から179度まで可能。
        /// </summary>
        public double DefocusAngleDeg
        {
            get => _defocusAngleDeg;
            set => _defocusAngleDeg = Math.Clamp(value, 0.0, 179.0);
        }
        public double DefocusAngleRad => _defocusAngleDeg * RayMath.Rotation.k_ToRadians;

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
        public int ImagePixels => ImageWidth * ImageHeight;

        public double ImageAspectRatio => ImageWidth / (double)ImageHeight;

        public double FieldOfViewAngleDeg
        {
            get => _fieldOfViewDeg;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                _fieldOfViewDeg = Math.Clamp(value, 0, 179);
            }
        }

        public double ViewportHeight => 2.0 * FocusDistance * Math.Tan(_fieldOfViewDeg * 0.5 * Math.PI / 180);
        public double ViewportWidth => ViewportHeight * ImageAspectRatio;

        private int _imageWidth = 640;
        private int _imageHeight = 480;
        /// <summary>
        /// 画角(0度から180度まで)
        /// </summary>
        private double _fieldOfViewDeg = 30.0;
        /// <summary>
        /// DepthOfFieldの描画に必要。0から179度まで可能。
        /// </summary>
        private double _defocusAngleDeg = 0.0;
    }
}
