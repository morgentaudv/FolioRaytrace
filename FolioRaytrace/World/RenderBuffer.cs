using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.World
{
    /// <summary>
    /// 画像出力に使うバッファークラス
    /// </summary>
    public class RenderBuffer
    {
        public int BufferSize => _colorBuffer.Length;

        public RayMath.Vector3 this[int i]
        {
            get => _colorBuffer[i];
            set => _colorBuffer[i] = value;
        }

        public void UpdateAt(int i, RayMath.Vector3 v)
        {
            _colorBuffer[i] = v;
        }

        public RenderBuffer(int initSize)
        {
            _colorBuffer = new RayMath.Vector3[initSize];
        }

        /// <summary>
        /// バッファー内容を書く
        /// </summary>
        public void WritePPM(TextWriter writer)
        {
            foreach (var color in _colorBuffer)
            {
                writer.WriteLine($"{Utility.To255Color(color)}");
            }
        }

        private RayMath.Vector3[] _colorBuffer;
    }
}
