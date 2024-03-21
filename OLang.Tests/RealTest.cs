using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class RealTest: LexerTest
{
    [Fact]
    public void Zero()
    {
        var zero = lexer.Feed(Text("0.0"));
        AssertEqual([Real(0)], zero);
    }

    [Fact]
    public void NegativeZero()
    {
        var minusZero = lexer.Feed(Text("-0.0"));
        AssertEqual([Real(0)], minusZero);
    }

    [Fact]
    public void MaxReal()
    {
        var maxReal = lexer.Feed(Text(double.MaxValue.ToString("F", CultureInfo.InvariantCulture)));
        var expectedToken = Real(double.MaxValue);
        AssertEqual([expectedToken], maxReal);
    }

    [Fact(Skip = "Unary minus is not implemented yet")]
    public void MinReal()
    {
        var str = double.MinValue.ToString("F", CultureInfo.InvariantCulture);
        var minReal = lexer.Feed(Text(str));
        var expectedToken = Real(double.MinValue);
        AssertEqual([expectedToken], minReal);
    }

    [Fact]
    public void Epsilon()
    {
        string str = double.Epsilon.ToString("F500");
        var epsilon = lexer.Feed(Text(str));
        var expectedToken = Real(double.Epsilon);
        AssertEqual([expectedToken], epsilon);
    }

    [Fact]
    public void SingleReal()
    {
        Random random = new Random();

        for (int i = 0; i < 10_000; i++)
        {
            // TODO: Когда (если) заработает унарный минус, нижней границей должно быть double.MinValue
            var x = random.NextDouble() * random.NextDouble();
            
            var str = x.ToString("F", CultureInfo.InvariantCulture);
            var actualToken = lexer.Feed(Text(str));
            
            var expectedToken = Real(Convert.ToDouble(str));

            AssertEqual([expectedToken], actualToken);
        }
    }
}
