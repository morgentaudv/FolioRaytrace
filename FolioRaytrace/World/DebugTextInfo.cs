using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.World
{
    internal sealed class DebugTextInfo
    {
        public sealed class InfoItem
        {
            public InfoItem(byte[] infos) { 
                Infos = infos;
                W = infos.Length / 5;
            }
            /// <summary>
            /// 今は4x4固定で行く。
            /// </summary>
            public readonly byte[] Infos;
            public readonly int W;
        }

        public const int k_HEIGHT = 5;

        public static readonly InfoItem[] s_ASCIIs;

        static DebugTextInfo()
        {
            s_ASCIIs = new InfoItem[128];

            s_ASCIIs[(byte)'!'] = new InfoItem([ 1, 1, 1, 0, 1, ]);
            s_ASCIIs[(byte)'"'] = new InfoItem( [ 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, ]);
            s_ASCIIs[(byte)'#'] = new InfoItem(
                [
                    0, 1, 0, 1, 0,
                    1, 1, 1, 1, 1,
                    0, 1, 0, 1, 0,
                    1, 1, 1, 1, 1,
                    0, 1, 0, 1, 0,
                ]);
            s_ASCIIs[(byte)'$'] = new InfoItem(
                [
                    0, 1, 1, 1, 1,
                    1, 0, 1, 0, 0,
                    0, 1, 1, 1, 0,
                    0, 0, 1, 0, 1,
                    1, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'%'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    0, 0, 0, 1, 0,
                    0, 0, 1, 0, 0,
                    0, 1, 0, 0, 0,
                    1, 0, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'\''] = new InfoItem( [ 1, 1, 0, 0, 0, ]);
            s_ASCIIs[(byte)'('] = new InfoItem( [ 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, ]);
            s_ASCIIs[(byte)')'] = new InfoItem( [ 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, ]);
            s_ASCIIs[(byte)'*'] = new InfoItem( [ 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, ]);
            s_ASCIIs[(byte)'+'] = new InfoItem( [ 0, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, ]);
            s_ASCIIs[(byte)','] = new InfoItem( [ 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, ]);
            s_ASCIIs[(byte)'-'] = new InfoItem( [ 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, ]);
            s_ASCIIs[(byte)'.'] = new InfoItem( [ 0, 0, 0, 0, 1, ]);
            s_ASCIIs[(byte)'/'] = new InfoItem( [ 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, ]);

            s_ASCIIs[(byte)'0'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 1, 1,
                    1, 0, 1, 0, 1,
                    1, 1, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'1'] = new InfoItem(
                [
                    0, 1, 
                    1, 1, 
                    0, 1, 
                    0, 1, 
                    0, 1, 
                ]);
            s_ASCIIs[(byte)'2'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 0,
                    1, 1, 1, 1, 1,
                ]);
            s_ASCIIs[(byte)'3'] = new InfoItem(
                [
                    1, 1, 1, 1, 0,
                    0, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                    0, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'4'] = new InfoItem(
                [
                    0, 0, 0, 1, 0,
                    0, 0, 1, 1, 0,
                    0, 1, 0, 1, 0,
                    1, 1, 1, 1, 1,
                    0, 0, 0, 1, 0,
                ]);
            s_ASCIIs[(byte)'5'] = new InfoItem(
                [
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 1, 1, 1, 0,
                    0, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'6'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 0,
                    1, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'7'] = new InfoItem(
                [
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 1,
                    0, 0, 0, 1, 0,
                    0, 0, 1, 0, 0,
                    0, 1, 0, 0, 0,
                ]);
            s_ASCIIs[(byte)'8'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'9'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 1,
                    0, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);

            s_ASCIIs[(byte)'"'] = new InfoItem( [ 0, 1, 0, 1, 0, ]);
            s_ASCIIs[(byte)'"'] = new InfoItem( [ 0, 1, 0, 1, 1, ]);

            s_ASCIIs[(byte)'<'] = new InfoItem( [ 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, ]);
            s_ASCIIs[(byte)'>'] = new InfoItem( [ 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, ]);
            s_ASCIIs[(byte)'='] = new InfoItem( [ 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, ]);
            s_ASCIIs[(byte)'?'] = new InfoItem(
                [
                    1, 1, 1, 0,
                    0, 0, 0, 1,
                    0, 1, 1, 0,
                    0, 0, 0, 0,
                    0, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'@'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 1, 0, 1,
                    1, 1, 0, 1, 1,
                    1, 0, 1, 1, 1,
                    0, 1, 1, 1, 0,
                ]);

            s_ASCIIs[(byte)'A'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'B'] = new InfoItem(
                [
                    1, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'C'] = new InfoItem(
                [
                    0, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 0, 0, 0, 0,
                    1, 0, 0, 0, 0,
                    0, 1, 1, 1, 1,
                ]);
            s_ASCIIs[(byte)'D'] = new InfoItem(
                [
                    1, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'E'] = new InfoItem(
                [
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 1, 1, 1, 1,
                ]);
            s_ASCIIs[(byte)'F'] = new InfoItem(
                [
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 0, 0, 0, 0,
                ]);
            s_ASCIIs[(byte)'G'] = new InfoItem(
                [
                    0, 1, 1, 1, 1,
                    1, 0, 0, 0, 0,
                    1, 0, 1, 1, 1,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'H'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'I'] = new InfoItem(
                [
                    1, 1, 1, 
                    0, 1, 0, 
                    0, 1, 0, 
                    0, 1, 0, 
                    1, 1, 1, 
                ]);
            s_ASCIIs[(byte)'J'] = new InfoItem(
                [
                    0, 1, 1, 
                    0, 0, 1, 
                    0, 0, 1, 
                    1, 0, 1, 
                    0, 1, 0, 
                ]);
            s_ASCIIs[(byte)'K'] = new InfoItem(
                [
                    1, 0, 0, 1,
                    1, 0, 1, 0,
                    1, 1, 0, 0,
                    1, 0, 1, 0,
                    1, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'L'] = new InfoItem(
                [
                    1, 0, 0, 
                    1, 0, 0, 
                    1, 0, 0, 
                    1, 0, 0, 
                    1, 1, 1, 
                ]);
            s_ASCIIs[(byte)'M'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    1, 1, 0, 1, 1,
                    1, 0, 1, 0, 1,
                    1, 0, 1, 0, 1,
                    1, 0, 1, 0, 1,
                ]);
            s_ASCIIs[(byte)'N'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    1, 1, 0, 0, 1,
                    1, 0, 1, 0, 1,
                    1, 0, 0, 1, 1,
                    1, 0, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'O'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'P'] = new InfoItem(
                [
                    1, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                    1, 0, 0, 0, 0,
                    1, 0, 0, 0, 0,
                ]);
            s_ASCIIs[(byte)'Q'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 0, 1, 0, 1,
                    1, 0, 0, 1, 0,
                    0, 1, 1, 0, 1,
                ]);
            s_ASCIIs[(byte)'R'] = new InfoItem(
                [
                    1, 1, 1, 1, 0,
                    1, 0, 0, 0, 1,
                    1, 1, 1, 1, 0,
                    1, 0, 0, 1, 0,
                    1, 0, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'S'] = new InfoItem(
                [
                    0, 1, 1, 1, 0,
                    1, 0, 0, 0, 0,
                    0, 1, 1, 1, 0,
                    0, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'T'] = new InfoItem(
                [
                    1, 1, 1, 1, 1,
                    0, 0, 1, 0, 0,
                    0, 0, 1, 0, 0,
                    0, 0, 1, 0, 0,
                    0, 0, 1, 0, 0,
                ]);
            s_ASCIIs[(byte)'U'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    0, 1, 1, 1, 0,
                ]);
            s_ASCIIs[(byte)'V'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    1, 0, 0, 0, 1,
                    0, 1, 0, 1, 0,
                    0, 0, 1, 0, 0,
                ]);
            s_ASCIIs[(byte)'W'] = new InfoItem(
                [
                    1, 0, 1, 0, 1,
                    1, 0, 1, 0, 1,
                    1, 0, 1, 0, 1,
                    1, 0, 1, 0, 1,
                    0, 1, 0, 1, 0,
                ]);
            s_ASCIIs[(byte)'X'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    0, 1, 0, 1, 0,
                    0, 0, 1, 0, 0,
                    0, 1, 0, 1, 0,
                    1, 0, 0, 0, 1,
                ]);
            s_ASCIIs[(byte)'Y'] = new InfoItem(
                [
                    1, 0, 0, 0, 1,
                    0, 1, 0, 1, 0,
                    0, 0, 1, 0, 0,
                    0, 0, 1, 0, 0,
                    0, 0, 1, 0, 0,
                ]);
            s_ASCIIs[(byte)'Z'] = new InfoItem(
                [
                    1, 1, 1, 1, 1,
                    0, 0, 0, 1, 0,
                    0, 0, 1, 0, 0,
                    0, 1, 0, 0, 0,
                    1, 1, 1, 1, 1,
                ]);

            s_ASCIIs[(byte)'['] = new InfoItem( [ 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, ]);
            s_ASCIIs[(byte)']'] = new InfoItem( [ 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, ]);
            s_ASCIIs[(byte)'\\'] = new InfoItem( [ 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, ]);
            s_ASCIIs[(byte)'^'] = new InfoItem( [ 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, ]);
            s_ASCIIs[(byte)'_'] = new InfoItem( [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, ]);
            s_ASCIIs[(byte)'`'] = new InfoItem( [ 1, 0,  0, 1,  0, 0,  0, 0,  0, 0,  ]);
            s_ASCIIs[(byte)'{'] = new InfoItem( [ 0, 1, 0, 1, 1, 0, 0, 1, 0, 1, ]);
            s_ASCIIs[(byte)'|'] = new InfoItem([ 1, 1, 0, 1, 1, ]);
            s_ASCIIs[(byte)'}'] = new InfoItem( [ 1, 0, 1, 0, 0, 1, 1, 0, 1, 0, ]);
            s_ASCIIs[(byte)'~'] = new InfoItem( [ 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, ]);

            s_ASCIIs[(byte)'a'] = s_ASCIIs[(byte)'A'];
            s_ASCIIs[(byte)'b'] = s_ASCIIs[(byte)'B'];
            s_ASCIIs[(byte)'c'] = s_ASCIIs[(byte)'C'];
            s_ASCIIs[(byte)'d'] = s_ASCIIs[(byte)'D'];
            s_ASCIIs[(byte)'e'] = s_ASCIIs[(byte)'E'];
            s_ASCIIs[(byte)'f'] = s_ASCIIs[(byte)'F'];
            s_ASCIIs[(byte)'g'] = s_ASCIIs[(byte)'G'];

            s_ASCIIs[(byte)'h'] = s_ASCIIs[(byte)'H'];
            s_ASCIIs[(byte)'i'] = s_ASCIIs[(byte)'I'];
            s_ASCIIs[(byte)'j'] = s_ASCIIs[(byte)'J'];
            s_ASCIIs[(byte)'k'] = s_ASCIIs[(byte)'K'];
            s_ASCIIs[(byte)'l'] = s_ASCIIs[(byte)'L'];
            s_ASCIIs[(byte)'m'] = s_ASCIIs[(byte)'M'];
            s_ASCIIs[(byte)'n'] = s_ASCIIs[(byte)'N'];
            s_ASCIIs[(byte)'o'] = s_ASCIIs[(byte)'O'];
            s_ASCIIs[(byte)'p'] = s_ASCIIs[(byte)'P'];

            s_ASCIIs[(byte)'q'] = s_ASCIIs[(byte)'Q'];
            s_ASCIIs[(byte)'r'] = s_ASCIIs[(byte)'R'];
            s_ASCIIs[(byte)'s'] = s_ASCIIs[(byte)'S'];
            s_ASCIIs[(byte)'t'] = s_ASCIIs[(byte)'T'];
            s_ASCIIs[(byte)'u'] = s_ASCIIs[(byte)'U'];
            s_ASCIIs[(byte)'v'] = s_ASCIIs[(byte)'V'];
            s_ASCIIs[(byte)'w'] = s_ASCIIs[(byte)'W'];
            s_ASCIIs[(byte)'x'] = s_ASCIIs[(byte)'X'];
            s_ASCIIs[(byte)'y'] = s_ASCIIs[(byte)'Y'];
            s_ASCIIs[(byte)'z'] = s_ASCIIs[(byte)'Z'];

            // 0, 0, 0, 0, 0,
        }
    }}
