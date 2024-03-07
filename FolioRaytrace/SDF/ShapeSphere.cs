using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FolioRaytrace.RayMath;

namespace FolioRaytrace.SDF
{
    /// <summary>
    /// SDF図形自体としての球体を表す。
    /// </summary>
    public class ShapeSphere
    {
        public ShapeSphere(Vector3 center, double radius)
        {
            // if文からのthrowがこれに縮められる
            ArgumentOutOfRangeException.ThrowIfNegative(radius);

            _center = center;
            _radius = radius;
        }

        public Vector3 Center
        {
            get => _center;
            set => _center = value;
        }

        public double Radius
        {
            get => _radius;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _radius = value;
            }
        }

        /// <summary>
        /// SDF距離を計算して返す。
        /// </summary>
        public double Distance(Vector3 p)
        {
            // 浮動小数点の誤差でエラーが起きえるので一定小数点以下なら四捨五入する。
            var origDistance = (p - Center).Length - Radius;
            return Math.Round(origDistance, 5, MidpointRounding.ToZero);
        }

        /// <summary>
        /// rayがこの図形に交差しているかを判定する。
        /// </summary>
        public bool IsIntersected(Ray ray)
        {
            var distance = Distance(ray.Orig);
            if (distance <= 0)
            { return true; }

            // https://en.wikipedia.org/wiki/Line%E2%80%93sphere_intersection
            // u = ray.Direction
            // o = ray.Orig
            // c = Center
            // r = Radius
            var v1 = Math.Pow(ray.Direction.Dot(ray.Orig - Center), 2.0);
            var v2 = (ray.Orig - Center).LengthSquared - Radius * Radius;

            // < 0ならIntersectしない。0なら表面にタッチして終わりだが、ここでは交差するとみなす。
            return v1 - v2 >= 0;
        }

        /// <summary>
        /// rayがこの図形に交差しているかを判定する。ただしRayの方向から逆方向進むことはできない。
        /// </summary>
        public bool IsIntersectedStrict(Ray ray)
        {
            var distance = Distance(ray.Orig);
            if (distance <= 0)
            { return true; }

            var values = TryGetRayZeroValues(ray);
            return values != null && values.Count >= 1;
        }

        /// <summary>
        /// rayが進んで図形に当たる場合、表面の法線をoutDirに返してtrueを返す。
        /// 内部からだと内部に展開する法線を返す。
        /// </summary>
        /// <param name="outDir">法線自体を示す。これが内部か外部かはoutIsInternalを確認する。</param>
        /// <param name="outIsInternal">trueなら内部からの法線であることを示す。</param>
        public bool TryGetNormal(out Vector3 outDir, out bool outIsInternal, Ray ray)
        {
            // outは必ず指定することになっているので…
            outDir = Vector3.s_Zero;
            outIsInternal = false;

            // 交差しなきゃNormalが求められない
            if (!IsIntersected(ray))
            { return false; }

            // Sphereだけなら表面で近似で計算できる。
            // もし最初distanceが負の数なら、SDFによって内部だと仮定する。
            double distance = double.MaxValue;
            bool isFirst = true;
            bool isFromInternal = false;
            while (Math.Abs(distance) > 1e-3)
            {
                distance = Distance(ray.Orig);
                if (isFirst)
                {
                    isFromInternal = distance < 0;
                    isFirst = false;
                }

                ray.Orig = ray.Proceed(distance);
            }

            outIsInternal = isFromInternal;
            if (isFromInternal)
            {
                outDir = (Center - ray.Orig).Unit();
            }
            else
            {
                outDir = (ray.Orig - Center).Unit();
            }

            // 複雑なものは表面近似で計算できるかも。Tetrahedronで。
            // https://iquilezles.org/articles/normalsSDF/
            //const double k_EPS = 1e-3; // epsilon
            //var v1 = new Vector3(1, -1, -1) * Distance(ray.Orig + (new Vector3(1, -1, -1) * k_EPS));
            //var v2 = new Vector3(-1, 1, -1) * Distance(ray.Orig + (new Vector3(-1, 1, -1) * k_EPS));
            //var v3 = new Vector3(-1, -1, 1) * Distance(ray.Orig + (new Vector3(-1, -1, 1) * k_EPS));
            //var v4 = new Vector3(1, 1, 1) * Distance(ray.Orig + (new Vector3(1, 1, 1) * k_EPS));
            //outDir = (v1 + v2 + v3 + v4).Unit();
            return true;
        }

        /// <summary>
        /// rayが+方向に進んで図形に衝突できる進む距離Tのリストを返す。もし全部失敗したらnullを返す。
        /// </summary>
        private List<double>? TryGetRayZeroValues(Ray ray)
        {
            var o1 = ray.Direction.Dot(ray.Orig - Center);
            var v1 = Math.Pow(o1, 2.0);
            var v2 = (ray.Orig - Center).LengthSquared - Radius * Radius;
            var dt2 = v1 - v2;
            if (dt2 < 0)
            {
                // 実数の解が求められないので失敗。
                return null;
            }

            // これは正の数しか返さないので、直接判定。
            var dt = Math.Sqrt(dt2);
            var ans1 = -o1 - dt;
            var ans2 = -o1 + dt;
            // ansが全部負の数なら失敗。
            if (ans1 < 0 && ans2 < 0)
            {
                return null;
            }

            // +方向で行けるTだけ入れて返す。
            var results = new List<double>();
            if (ans1 >= 0)
            {
                results.Add(ans1);
            }
            if (ans2 >= 0)
            {
                results.Add(ans2);
            }
            return results;
        }

        /// <summary>
        /// rayが図形にヒットしたらnullじゃないHitResultを返す。じゃなきゃnullを返す。
        /// ただしrayが進むTが`[rayTMin, rayTMax)`の範囲外だとnullを返す。
        /// </summary>
        public HitResult? TryHit(Ray ray, double rayTMin, double rayTMax)
        {
            if (!IsIntersectedStrict(ray))
            {
                return null;
            }

            // チェックして一番短いTを求める。
            bool isFinalTVUpdated = false;
            double finalTV = double.MaxValue;
            var tValues = TryGetRayZeroValues(ray)!;
            if (tValues == null || tValues.Count == 0)
            {
                return null;
            }

            foreach (var tV in tValues)
            {
                // 範囲外だと失敗。
                if (tV < rayTMin || tV >= rayTMax)
                { continue; }

                finalTV = double.Min(tV, finalTV);
                isFinalTVUpdated = true;
            }
            if (!isFinalTVUpdated)
            {
                return null;
            }

            // 計算して返す。
            var proceedPos = ray.Proceed(finalTV);
            var proceedRay = new Ray(proceedPos, ray.Direction);
            var result = new HitResult();
            result.ProceedT = finalTV;
            if (!TryGetNormal(out result.Normal, out result.IsInternal, proceedRay))
            {
                return null;
            }
            result.Point = proceedPos;

            return result;
        }

        private Vector3 _center;
        private double _radius;
    }
}
