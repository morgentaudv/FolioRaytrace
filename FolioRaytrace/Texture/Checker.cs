using FolioRaytrace.RayMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Texture
{
    public class Checker : ITextureBase
    {
        public Checker(double scale, ITextureBase even, ITextureBase odd)
        {
            // If scale is not positive, throw exception.
            if (scale <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _scale = scale;
            _even = even;
            _odd = odd;
        }

        public Vector3 Value(ITextureBase.ValueSetting setting)
        {
            var invScale = 1.0 / _scale;
            int xInteger = (int)Math.Floor(invScale * setting.Point.X);
            int yInteger = (int)Math.Floor(invScale * setting.Point.Y);
            int zInteger = (int)Math.Floor(invScale * setting.Point.Z);
            bool isEven = ((xInteger + yInteger + zInteger) % 2) == 0;

            if (isEven)
            {
                return _even.Value(setting);
            }
            else
            {
                return _odd.Value(setting);
            }
        }

        private double _scale;
        private ITextureBase _even;
        private ITextureBase _odd;
    }
}
