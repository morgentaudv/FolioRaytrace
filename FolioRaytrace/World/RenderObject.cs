using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.World
{
    /// <summary>
    /// 描画されるオブジェクトを指す。
    /// </summary>
    public class RenderObject
    {
        public RenderObject(SDF.ShapeSphere shape, Material.MaterialBase material)
        {
            _shape = shape;
            _material = material;
        }

        public SDF.AABB AABB
        {
            get {
                switch (Shape)
                {
                case SDF.ShapeSphere sphere:
                {
                    return SDF.AABB.From(sphere);
                }
                default:
                {
                    throw new UnreachableException();
                }
                }
            }
        }

        public static bool CompareMinX(RenderObject lhs, RenderObject rhs)
        {
            return lhs.AABB.MinPosition.X < rhs.AABB.MinPosition.X;
        }

        public static bool CompareMinY(RenderObject lhs, RenderObject rhs)
        {
            return lhs.AABB.MinPosition.Y < rhs.AABB.MinPosition.Y;
        }

        public static bool CompareMinZ(RenderObject lhs, RenderObject rhs)
        {
            return lhs.AABB.MinPosition.Z < rhs.AABB.MinPosition.Z;
        }

        public class AscendingMinX : IComparer<RenderObject>
        {
            public int Compare(RenderObject? lhs, RenderObject? rhs)
            {
                // -だと<、0だと=、+だと>になる。
                if (lhs == null && rhs == null)
                {
                    return 0;
                }
                if (lhs != null && rhs == null)
                {
                    return -1;
                }
                if (lhs == null && rhs != null)
                {
                    return 1;
                }

                var lv = lhs!.AABB.MinPosition.X;
                var rv = rhs!.AABB.MinPosition.X;
                if (lv - rv < 0)
                {
                    return -1;
                }
                if (lv - rv > 0)
                {
                    return 1;
                }
                return 0;
            }
        }

        public class AscendingMinY : IComparer<RenderObject>
        {
            public int Compare(RenderObject? lhs, RenderObject? rhs)
            {
                // -だと<、0だと=、+だと>になる。
                if (lhs == null && rhs == null)
                {
                    return 0;
                }
                if (lhs != null && rhs == null)
                {
                    return -1;
                }
                if (lhs == null && rhs != null)
                {
                    return 1;
                }

                var lv = lhs!.AABB.MinPosition.Y;
                var rv = rhs!.AABB.MinPosition.Y;
                if (lv - rv < 0)
                {
                    return -1;
                }
                if (lv - rv > 0)
                {
                    return 1;
                }
                return 0;
            }
        }

        public class AscendingMinZ : IComparer<RenderObject>
        {
            public int Compare(RenderObject? lhs, RenderObject? rhs)
            {
                // -だと<、0だと=、+だと>になる。
                if (lhs == null && rhs == null)
                {
                    return 0;
                }
                if (lhs != null && rhs == null)
                {
                    return -1;
                }
                if (lhs == null && rhs != null)
                {
                    return 1;
                }

                var lv = lhs!.AABB.MinPosition.Z;
                var rv = rhs!.AABB.MinPosition.Z;
                if (lv - rv < 0)
                {
                    return -1;
                }
                if (lv - rv > 0)
                {
                    return 1;
                }
                return 0;
            }
        }

        public object Shape => _shape;
        public Material.MaterialBase Material => _material;

        private object _shape;
        private Material.MaterialBase _material;
    }

    /// <summary>
    /// 子BVHノードまたはRenderObjectが持てるBVHノード
    /// </summary>
    public class BVHNode
    {
        public BVHNode(IEnumerable<RenderObject> renderObjects)
        {
            var rangeCount = renderObjects.Count();
            if (rangeCount == 0)
            {
                // 何もせずに終わり。
                throw new InvalidDataException();
            }

            SDF.AABB? aabb = null;
            switch (rangeCount)
            {
            case 1:
            {
                // 一つしかないのでLeftとRightにそのまま適用する。
                var renderObject = renderObjects.First();
                _leftNode = renderObject;
                _rightNode = renderObject;
                aabb = renderObject.AABB;
            }
            break;
            case 2:
            {
                // 2個しかないなら、分離する。
                var leftObject = renderObjects.First();
                var rightObject = renderObjects.Last();
                aabb = leftObject.AABB.Union(rightObject.AABB);

                // AABBからどれが一番長いかを判定して、その長い軸から切る。
                // つまりX軸がながければYZ平面に切る…など。
                var longestElemI = aabb.Lengths.MaxElementI;
                _separateAxisI = longestElemI;

                var comparisonMethod = RenderObject.CompareMinX;
                switch (longestElemI)
                {
                case 0: // X
                {
                    comparisonMethod = RenderObject.CompareMinX;
                }
                break;
                case 1: // Y
                {
                    comparisonMethod = RenderObject.CompareMinY;
                }
                break;
                case 2: // Z
                {
                    comparisonMethod = RenderObject.CompareMinZ;
                }
                break;
                default:
                {
                    throw new UnreachableException();
                }
                }

                if (comparisonMethod(leftObject, rightObject))
                {
                    _leftNode = leftObject;
                    _rightNode = rightObject;
                }
                else
                {
                    _leftNode = rightObject;
                    _rightNode = leftObject;
                }
            }
            break;
            default:
            {
                // ソートして2つに割る。
                var sortedObjects = renderObjects.ToArray();
                foreach (var renderObject in sortedObjects)
                {
                    if (aabb == null)
                    {
                        aabb = renderObject.AABB;
                    }
                    else
                    {
                        // 拡張する。
                        aabb = aabb.Union(renderObject.AABB);
                    }
                }

                // AABBからどれが一番長いかを判定して、その長い軸から切る。
                // つまりX軸がながければYZ平面に切る…など。
                var longestElemI = aabb!.Lengths.MaxElementI;
                _separateAxisI = longestElemI;

                // https://www.hanachiru-blog.com/entry/2020/04/03/120000
                // https://learn.microsoft.com/ja-jp/dotnet/standard/collections/comparisons-and-sorts-within-collections
                IComparer<RenderObject>? comparison = null;
                switch (longestElemI)
                {
                case 0: // X
                {
                    comparison = new RenderObject.AscendingMinX();
                }
                break;
                case 1: // Y
                {
                    comparison = new RenderObject.AscendingMinY();
                }
                break;
                case 2: // Z
                {
                    comparison = new RenderObject.AscendingMinZ();
                }
                break;
                default:
                {
                    throw new UnreachableException();
                }
                }
                Array.Sort(sortedObjects, comparison!);

                // sortedObjectsから前半分のIEnumerableを取得する。
                var leftCount = sortedObjects.Count() >> 1;
                _leftNode = new BVHNode(sortedObjects.Take(leftCount));
                _rightNode = new BVHNode(sortedObjects.Skip(leftCount));
            }
            break;
            }

            _aabb = aabb;
        }

        public bool CanHit(RayMath.Ray ray, double rayTMin, double rayTMax) 
            => _aabb.CanHit(ray, rayTMin, rayTMax);

        /// <summary>
        /// 一旦全部objectにしよ。。。BVHParentNodeか、RenderObjectか。
        /// </summary>
        private object _leftNode;
        /// <summary>
        /// 一旦全部objectにしよ。。。
        /// </summary>
        private object _rightNode;
        private SDF.AABB _aabb;
        private int _separateAxisI = -1;
    }
}
