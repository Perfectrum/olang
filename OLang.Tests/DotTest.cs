using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class DotTest: LexerTest
{
    [Fact]
    public void SimpleField()
    {
        AssertEqual(
            [Ident("Something"), Dot, Ident("Field")],
            lexer.Feed(Text("Something.Field"))
        );
    }

    [Fact]
    public void IntegerField()
    {
        AssertEqual(
            [Int(2), Dot, Ident("Sign")],
            lexer.Feed(Text("2.Sign"))
        );
    }

    [Fact]
    public void RealField()
    {
        AssertEqual(
            [Real(2.0), Dot, Ident("Sign")],
            lexer.Feed(Text("2.0.Sign"))
        );
    }

    [Fact]
    public void StringField()
    {
        AssertEqual(
            [StringLiteral("Hello, world!"), Dot, Ident("Length")],
            lexer.Feed(Text("\"Hello, world!\".Length"))
        );
    }

    [Fact]
    public void KeywordAsField()
    {
        Array keywords = Enum.GetValues(typeof(KeywordType));
        foreach (KeywordType keyword in keywords)
        {
            AssertEqual(
                [Ident("Something"), Dot, Ident(keyword.ToString())],
                lexer.Feed(Text("Something." + keyword.ToString()))
            );
        }
    }

    [Fact]
    public void Chain()
    {
        AssertEqual([
                Ident("Something"), Dot,
                Ident("Field"), Dot,
                Ident("Method"), LP, RP, Dot,
                Ident("Field"), Dot,
                Ident("Again"), LP, Ident("arg"), RP
            ], lexer.Feed(Text("Something.Field.Method().Field.Again(arg)"))
        );
    }
}