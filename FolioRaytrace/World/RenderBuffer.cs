using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public RenderBuffer(int width, int height)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width * height);
            _width = width;
            _height = height;  
            _colorBuffer = new RayMath.Vector3[_width * _height];
            _debugTextItems = new List<DebugTextItem>();
        }

        // --------------------------------------------------------------------
        // 文字書き込み
        // --------------------------------------------------------------------

        private struct DebugTextItem
        {
            public string String;
            public RayMath.Vector3 Color;
            public int X;
            public int Y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str">出力する文字列。ASCII文字しか描画しないことに注意</param>
        /// <param name="color">文字列の色</param>
        /// <param name="x">文字列の最初出力横位置</param>
        /// <param name="y">最初出力縦位置</param>
        public void WriteDebugText(string str, RayMath.Vector3 color, int x, int y)
        {
            if (str.Count() == 0)
            { return; }

            var item = new DebugTextItem();
            item.String = str;
            item.Color = color;
            item.X = x;
            item.Y = y;

            _debugTextItems.Add(item);
        }

        // --------------------------------------------------------------------
        // IO関連
        // --------------------------------------------------------------------

        /// <summary>
        /// バッファー内容を書く
        /// </summary>
        public void ExportPPM(TextWriter writer)
        {
            // _debugTextItemsを_colorBufferに上書きする。
            // 各文字は基本3x5にしたい。
            foreach (var item in _debugTextItems)
            {
                const int k_SPACE = 1;
                const int k_MUL = 2;

                var xCursor = item.X;

                foreach (var chr in item.String)
                {
                    if (xCursor >= _width)
                    { break; }
                    if (!Utility.IsCharAscii(chr))
                    { continue; }

                    // 文字情報を取得する。
                    // ただし空白などは特殊扱いしておく。
                    if (chr == ' ' || chr == '\t')
                    {
                        xCursor += (3 + k_SPACE) * k_MUL;
                    }

                    // もし文字指定がなければ描画できない。
                    var textInfo = DebugTextInfo.s_ASCIIs[(byte)chr];
                    if (textInfo == null)
                    { continue; }

                    // 描画する。(4x4 => 8x8)
                    var w = textInfo.W;
                    var yCursor = item.Y;
                    for (int y = 0; y < DebugTextInfo.k_HEIGHT * k_MUL; ++y)
                    {
                        var itemYI = y / k_MUL;
                        var yI = y + yCursor;
                        if (yI < 0 || yI >= _height)
                        { continue; } // 領域外

                        for (int x = 0; x < w * k_MUL; ++x)
                        {
                            var itemXI = x / k_MUL;
                            var xI = x + xCursor;
                            if (xI < 0 || xI >= _width)
                            { continue; } // 領域外

                            var flag = textInfo.Infos[(itemYI * w) + itemXI];
                            if (flag >= 1)
                            {
                                var bufferI = (yI * _width) + xI;
                                _colorBuffer[bufferI] = item.Color;
                            }
                        }
                    }

                    xCursor += (w + k_SPACE) * k_MUL;
                }
            }


            foreach (var color in _colorBuffer)
            {
                writer.WriteLine($"{Utility.To255Color(color)}");
            }
        }

        private int _width;
        private int _height;
        private RayMath.Vector3[] _colorBuffer;

        private List<DebugTextItem> _debugTextItems;
    }
}
