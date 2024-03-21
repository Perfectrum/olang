using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class MethodCall: LexerTest
{
    [Fact]
    public void IntegerMethod()
    {
        AssertEqual(
            [ Int(2), Dot, Ident("Add"), LP, Int(2), RP ],
            lexer.Feed(Text("2.Add(2)"))
        );
    }

    [Fact]
    public void RealMethod()
    {
        AssertEqual(
            [ Real(2.0), Dot, Ident("Add"), LP, Real(2.0), RP ],
            lexer.Feed(Text("2.0.Add(2.0)"))
        );
    }

    [Fact]
    public void StringMethod()
    {
        AssertEqual(
            [ StringLiteral("Hello "), Dot, Ident("Add"), LP, StringLiteral("world"), RP ],
            lexer.Feed(Text("\"Hello \".Add(\"world\")"))
        );
    }

    [Fact]
    public void NoArguments()
    {
        AssertEqual(
            [ Ident("Someone"), Dot, Ident("DoSomething"), LP, RP ],
            lexer.Feed(Text("Someone.DoSomething()"))
        );
    }

    [Fact]
    public void SingleIdentifierArgument()
    {
        AssertEqual(
            [ Ident("Someone"), Dot, Ident("DoSomething"), LP, Ident("argument"), RP ],
            lexer.Feed(Text("Someone.DoSomething(argument)"))
        );
    }

    [Fact]
    public void SingleIntegerArgument()
    {
        AssertEqual(
            [ Ident("Someone"), Dot, Ident("DoSomething"), LP, Int(42), RP ],
            lexer.Feed(Text("Someone.DoSomething(42)"))
        );
    }

    [Fact]
    public void SingleRealArgument()
    {
        AssertEqual(
            [ Ident("Someone"), Dot, Ident("DoSomething"), LP, Real(3.14), RP ],
            lexer.Feed(Text("Someone.DoSomething(3.14)"))
        );
    }

    [Fact]
    public void SingleStringArgument()
    {
        AssertEqual(
            [ Ident("Someone"), Dot, Ident("DoSomething"), LP, StringLiteral("argument"), RP ],
            lexer.Feed(Text("Someone.DoSomething(\"argument\")"))
        );
    }

    [Fact]
    public void MultipleArguments()
    {
        AssertEqual(
            [ Ident("Someone"), Dot, Ident("DoSomething"), LP, StringLiteral("many different arguments!"), CM, Int(42), CM, Real(3.14), CM, StringLiteral("wow!"), CM, Ident("Identifier"), RP ],
            lexer.Feed(Text("Someone.DoSomething(\"many different arguments!\", 42, 3.14, \"wow!\", Identifier )"))
        );
    }
}