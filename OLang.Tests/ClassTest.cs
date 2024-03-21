using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class Class: LexerTest
{
    [Fact]
    public void EmptyClass()
    {
        AssertEqual(
            [Cls, Ident("Test"), Is, End],
            lexer.Feed(Text("class Test is end"))
        );
    }

    [Fact]
    public void EmptyConstructor()
    {
        AssertEqual(
            [
                Cls, Ident("Test"), Is,
                This, LP, RP, Is, End,
                End
            ],
            lexer.Feed(Text("class Test is this() is end end"))
        );
    }

    [Fact]
    public void EmptyMethods()
    {
        string str = """
        class Human is
            this() is end
            Eat(food: Food) is end
            Sleep(hours: Integer) is end
        end
        """;

        AssertEqual(
            [
                Cls, Ident("Human"), Is, NewLine,
                    This, LP, RP, Is, End, NewLine,
                    Ident("Eat"), LP, Ident("food"), CL, Ident("Food"), RP,
                        Is, End, NewLine,
                    Ident("Sleep"), LP, Ident("hours"), CL, Ident("Integer"), RP,
                        Is, End, NewLine,
                End
            ],
            lexer.Feed(Text(str))
        );
    }

    [Fact]
    public void ComplicatedConstructor()
    {
        string str = """
        class Test is
            this() is
                var x : 3.Add(5.8, 0)
            end
        end
        """;
        AssertEqual(
            [
                Cls, Ident("Test"), Is, NewLine,
                    This, LP, RP, Is, NewLine,
                        Var, Ident("x"), CL, Int(3), Dot, Ident("Add"), LP, Real(5.8), CM, Int(0), RP, NewLine,
                    End, NewLine,
                End
            ],
            lexer.Feed(Text(str))
        );
    }
}