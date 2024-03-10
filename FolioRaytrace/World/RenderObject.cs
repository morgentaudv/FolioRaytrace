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


        public object Shape => _shape;
        public Material.MaterialBase Material => _material;

        private object _shape;
        private Material.MaterialBase _material;
    }
}
