using System;
using System.Collections.Generic;
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

        public object Shape => _shape;
        public Material.MaterialBase Material => _material;

        private object _shape;
        private Material.MaterialBase _material;
    }
}
