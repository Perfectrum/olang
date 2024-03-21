using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class IntegerTest: LexerTest
{
    [Fact]
    public void Zero()
    {
        var zero = lexer.Feed(Text("0"));
        AssertEqual([Int(0)], zero);
    }

    [Fact]
    public void NegativeZero()
    {
        var minusZero = lexer.Feed(Text("-0"));
        AssertEqual([Int(0)], minusZero);
    }

    [Fact]
    public void MaxInteger()
    {
        var maxInt = lexer.Feed(Text($"{int.MaxValue}"));
        var expectedToken = Int(int.MaxValue);
        AssertEqual([expectedToken], maxInt);
    }

    [Fact(Skip = "Unary minus is not implemented yet")]
    public void MinInteger()
    {
        var minInt = lexer.Feed(Text($"{int.MinValue}"));
        var expectedToken = Int(int.MinValue);
        AssertEqual([expectedToken], minInt);
    }

    [Fact]
    public void SingleInteger()
    {
        Random random = new Random();

        for (int i = 0; i < 10_000; i++)
        {
            // TODO: Когда (если) заработает унарный минус, нижней границей должно быть int.MinValue
            int x = random.Next(0, int.MaxValue);

            var actualToken = lexer.Feed(Text($"{x}"));
            var expectedToken = Int(x);

            AssertEqual([expectedToken], actualToken);
        }
    }

    [Fact]
    public void IntegersList()
    {
        Random random = new Random();

        for (int i = 0; i < 1000; i++)
        {
            int length = random.Next(1, 101);

            // TODO: Когда (если) заработает унарный минус, нижней границей должно быть int.MinValue
            var source = Enumerable.Range(0, length).Select(_ => random.Next(0, int.MaxValue)).ToArray();
            
            var str = string.Join(",", source);
            var actualTokens = lexer.Feed(Text(str));
            
            var expectedTokens = source.SelectMany(x => new [] { Int(x), CM }).ToArray();

            AssertEqual(expectedTokens, actualTokens);
        }
    }
}
