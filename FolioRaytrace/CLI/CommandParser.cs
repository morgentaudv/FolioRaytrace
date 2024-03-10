using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.CLI
{
    enum ECommandArgParseState
    {
        None,
        Output,
    }

    /// <summary>
    /// コマンド引数のパーシング結果物
    /// </summary>
    public struct ParseResult
    {
        public ParseResult() { } 

        public string ExplicitOutputPath = "";
        public bool UseDefaultOutputPath = false;
        public bool IsDebugMode = false;
        public bool UseParallel = false;
    }

    internal static class CommandParser
    {
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
                        parseState = ECommandArgParseState.Output;
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
                }
                break;
                case ECommandArgParseState.Output:
                {
                    // Validation.
                    try
                    {
                        // 生成だけする。
                        // 失敗したら例外が投げられる。
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
