using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.CLI
{
    /// <summary>
    /// コマンド引数のパーシング結果物
    /// </summary>
    public struct ParseResult
    {
        public ParseResult() { } 

        public string ExplicitOutputPath = "";
        public bool UseDefaultOutputPath = false;
        public bool UseDefaultWorld = false;
        public bool IsDebugMode = false;
        public bool UseParallel = false;
        public int ImageWidth = 720;
        public int ImageHeight = 480;
        public int SampleLevel = 1;
    }

    internal static class CommandParser
    {
        private enum ECommandArgParseState
        {
            None,
            OutputPath,
            SampleLevel,
            ImageWidth,
            ImageHeight,
        }

        public static bool TryParse(out ParseResult outResult, Span<string> args)
        {
            var parseState = ECommandArgParseState.None;
            var canOutput = false;
            var isWorldSpecified = false;
            outResult = new ParseResult();

            if (args.IsEmpty)
            {
                return false;
            }

            foreach (var arg in args)
            {
                switch (parseState)
                {
                case ECommandArgParseState.None:
                {
                    if (arg.Equals("-o") || arg.Equals("--output"))
                    {
                        parseState = ECommandArgParseState.OutputPath;
                    }
                    else if (arg.Equals("--output_default_path"))
                    {
                        outResult.UseDefaultOutputPath = true;
                        canOutput = true;
                    }
                    else if (arg.Equals("--use_default_world"))
                    {
                        outResult.UseDefaultWorld = true;
                        isWorldSpecified = true;
                    }
                    else if (arg.Equals("-d") || arg.Equals("--debug"))
                    {
                        outResult.IsDebugMode = true;
                    }
                    else if (arg.Equals("--parallel"))
                    {
                        outResult.UseParallel = true;
                    }
                    else if (arg.Equals("--sample_level"))
                    {
                        parseState = ECommandArgParseState.SampleLevel;
                    }
                    else if (arg.Equals("--image_width"))
                    {
                        parseState = ECommandArgParseState.ImageWidth;
                    }
                    else if (arg.Equals("--image_height"))
                    {
                        parseState = ECommandArgParseState.ImageHeight;
                    }
                }
                break;
                case ECommandArgParseState.OutputPath:
                {
                    try
                    {
                        // Validation.
                        // 生成して失敗したら例外が投げられる。
                        // ここではvar _は使えない。IDisposableが実行されない。
                        using (new System.IO.StreamWriter(arg!))
                        {

                        }

                        outResult.ExplicitOutputPath = arg;
                        parseState = ECommandArgParseState.None;
                        canOutput = true;
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                break;
                case ECommandArgParseState.SampleLevel:
                {
                    int level;
                    if (!int.TryParse(arg, out level))
                    {
                        return false;
                    }
                    if (level < 0)
                    {
                        System.Console.Error.WriteLine("--sample_level value must be 0 or positive.");
                        return false;
                    }

                    outResult.SampleLevel = level;
                    parseState = ECommandArgParseState.None;
                }
                break;
                case ECommandArgParseState.ImageWidth:
                {
                    int width;
                    if (!int.TryParse(arg, out width))
                    {
                        return false;
                    }
                    if (width <= 0)
                    {
                        System.Console.Error.WriteLine("--image_width value must be positive.");
                        return false;
                    }

                    outResult.ImageWidth = width;
                    parseState = ECommandArgParseState.None;
                }
                break;
                case ECommandArgParseState.ImageHeight:
                {
                    int height;
                    if (!int.TryParse(arg, out height))
                    {
                        return false;
                    }
                    if (height <= 0)
                    {
                        System.Console.Error.WriteLine("--image_height value must be positive.");
                        return false;
                    }

                    outResult.ImageHeight = height;
                    parseState = ECommandArgParseState.None;
                }
                break;
                }
            }

            if (!canOutput || !isWorldSpecified)
            {
                return false;
            }
            // ピクセル数を超えると失敗処理する。
            if ((ulong)outResult.ImageHeight * (ulong)outResult.ImageHeight > ((ulong)2 << 30 - 1))
            {
                System.Console.Error.WriteLine("Rendered pixel count must not be more than 2147483647.");
                return false;
            }

            return parseState == ECommandArgParseState.None;
        }
    }
}
