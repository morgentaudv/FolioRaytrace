using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.SDF
{
    /// <summary>
    /// Axis-Aligned Bounding Box
    /// </summary>
    public class AABB
    {
        /// <summary>
        /// 球体からAABBを生成する。
        /// </summary>
        public static AABB From(ShapeSphere shape)
        {
            var center = shape.Center;
            var fullLengths = RayMath.Vector3.s_One * (shape.Radius * 2.0);
            return new AABB(center, fullLengths);
        }

        /// <summary>
        /// 入力のポイントリストを全部含めるAABBを生成する。
        /// </summary>
        /// <exception cref="ArgumentException">入力リストの数が0だと発生</exception>
        public static AABB FromPoints(IEnumerable<RayMath.Vector3> points)
        {
            // Span<T>で良いのではないかと思ったが、もっと汎用的なIEnumerableにしたほうが良いと。
            // https://qiita.com/aka-nse/items/cea3c6f91413c3582b5f
            if (points.Count() == 0)
            {
                throw new ArgumentException("Given points must have at least one elements.");
            }

            var min = new RayMath.Vector3(double.MaxValue);
            var max = new RayMath.Vector3(double.MinValue);
            foreach (var point in points)
            {
                min = min.ElementMin(point);
                max = max.ElementMax(point);
            }

            var fullLengths = max - min;
            var center = min + ((max - min) * 0.5);
            return new AABB(center, fullLengths);
        }

        public AABB() { }

        public AABB(RayMath.Vector3 center, RayMath.Vector3 fullLengths)
        {
            _center = center;
            _fullLengths = fullLengths;
        }

        /// <summary>
        /// AABBの中心位置を返す。
        /// </summary>
        public RayMath.Vector3 Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public RayMath.Vector3 Lengths
        {
            get { return _fullLengths; }
            set
            {
                if (!value.IsAnyInvalid)
                {
                    throw new InvalidDataException("Lengths must have finite values.");
                }
                if (value.IsAnyNegative)
                {
                    throw new InvalidDataException("Lengths must have 0 or positive values.");
                }

                _fullLengths = value;
            }
        }

        public RayMath.Vector3 HalfLengths => Lengths * 0.5;

        public RayMath.Vector3 MinPosition => Center - HalfLengths;
        public RayMath.Vector3 MaxPosition => Center + HalfLengths;

        /// <summary>
        /// 入力したrayがこのAABB図形にrayTMinとrayTMaxの範囲で当たれるか？
        /// </summary>
        /// <returns>当たれればtrueを返す。</returns>
        public bool CanHit(RayMath.Ray ray, double rayTMin, double rayTMax)
        {
            // Ray-Slab Methodを使う。
            // https://en.wikipedia.org/wiki/Slab_method
            var minP = MinPosition;
            var maxP = MaxPosition;

            for (int i = 0; i < 3; ++i)
            {
                var b = ray.Direction[i];
                var p = ray.Orig[i];
                if (Math.Abs(b) < double.Epsilon)
                {
                    // もし並行であれば、要素位置が中に入っているかだけを確認する。
                    if (p < minP[i] || p >= maxP[i])
                    {
                        return false;
                    }
                }

                var invB = double.ReciprocalEstimate(b);
                var t0 = (minP[i] - p) * invB; 
                var t1 = (maxP[i] - p) * invB; 
                if (invB < 0)
                {
                    // Tupleを使って臨時変数を使わずにSwapできる。
                    (t0, t1) = (t1, t0);
                }

                var ft0 = rayTMin;
                if (t0 > rayTMin)
                {
                    ft0 = t0;
                }
                var ft1 = rayTMax;
                if (t1 < rayTMax)
                {
                    ft1 = t1;
                }

                if (ft1 <= ft0)
                {  return false; }
            }

            return true;
        }

        /// <summary>
        /// AABB図形の中心を示す。
        /// </summary>
        private RayMath.Vector3 _center = RayMath.Vector3.s_Zero;
        /// <summary>
        /// 各軸の長さを保持。かならずすべての要素が0か正の数を持つこと。
        /// </summary>
        private RayMath.Vector3 _fullLengths = RayMath.Vector3.s_Zero;
    
    }
}
