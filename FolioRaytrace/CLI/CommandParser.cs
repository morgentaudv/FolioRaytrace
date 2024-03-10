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
        public int SampleLevel = 1;
        public bool UseDefaultOutputPath = false;
        public bool IsDebugMode = false;
        public bool UseParallel = false;
    }

    internal static class CommandParser
    {
        private enum ECommandArgParseState
        {
            None,
            OutputPath,
            SampleLevel,
        }

        public static bool TryParse(out ParseResult outResult, Span<string> args)
        {
            var parseState = ECommandArgParseState.None;
            var canOutput = false;
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
                    int level = 0;
                    if (!int.TryParse(arg, out level))
                    {
                        return false;
                    }

                    outResult.SampleLevel = level;
                    parseState = ECommandArgParseState.None;
                }
                break;
                }
            }

            if (!canOutput)
            {
                return false;
            }

            return parseState == ECommandArgParseState.None;
        }
    }
}
