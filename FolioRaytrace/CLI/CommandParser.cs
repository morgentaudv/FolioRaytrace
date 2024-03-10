using System;
using System.Collections.Generic;
using System.Linq;
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
        public string OutputPath;
    }

    internal static class CommandParser
    {
        public static bool TryParse(out ParseResult outResult, Span<string> args)
        {
            var parseState = ECommandArgParseState.None;
            outResult = new ParseResult();

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
                        outResult.OutputPath = arg;
                        parseState = ECommandArgParseState.None;
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                break;
                }
            }

            return parseState == ECommandArgParseState.None;
        }
    } 
}
