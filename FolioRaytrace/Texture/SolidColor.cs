using FolioRaytrace.RayMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Texture
{
    /// <summary>
    /// 色だけを返す。
    /// </summary>
    public sealed class SolidColor : ITextureBase
    {
        public SolidColor(RayMath.Vector3 color)
        {
            _color = color;
        }

        public Vector3 Value(ITextureBase.ValueSetting setting)
        {
            return _color;
        }

        RayMath.Vector3 _color;
    }
}
