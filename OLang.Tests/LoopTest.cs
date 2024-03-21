using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class Loop: LexerTest
{
    [Fact]
    public void EmptyInfiniteLoop()
    {
        AssertEqual(
            [While, True, Loop, End],
            lexer.Feed(Text("while true loop end"))
        );
    }
}