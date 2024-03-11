using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.Texture
{
    /// <summary>
    /// RenderObjectのテクスチャーのベースクラス
    /// </summary>
    public interface ITextureBase
    {
        /// <summary>
        /// Value計算の設定
        /// </summary>
        struct ValueSetting
        {
            public double U;
            public double V;
            public RayMath.Vector3 Point;
        }

        /// <summary>
        /// setting情報から適切な色の値を返す。
        /// </summary>
        public RayMath.Vector3 Value(ValueSetting setting);
    }
}
