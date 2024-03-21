using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class ExtendedLexer: LexerTest
{
    [Fact]
    public void Easy()
    {
        AssertEqual(
            [
                While, True, Loop,
                Ident("Smth"), Dot, Ident("Test"), LP, RP, SC,
                Ident("Smth"), Dot, Ident("MoreTest"), LP, RP, SC,
                End, SC
            ],
            superLexer.Feed(
                Text(
                    """
                    while true loop
                        Smth.Test()
                        Smth.MoreTest()
                    end
                    """
                )
            )
        );
    }

    [Fact]
    public void ClassesNMethods()
    {
        AssertEqual(
            [
                Cls, Ident("A"), Is,
                Method, Ident("X"), LP, RP, Is, End, SC,
                Method, Ident("X"), LP, RP, Is, End, SC,
                End, SC,
                Cls, Ident("B"), Is,
                End, SC,
                Cls, Ident("C"), Is,
                End, SC
            ],
            superLexer.Feed(
                Text(
                    """
                    class A is
                        method X() is end
                        method X() is 
                        end
                    end
                    class B is end
                    class C is
                    end
                    """
                )
            )
        );
    }

    [Fact]
    public void HardStatement()
    {
        AssertEqual(
            [
                While, True, Loop,
                Ident("Console"), Dot, Ident("WriteLine"),
                LP,
                Ident("X"), Dot, Ident("Add"),
                LP,
                Ident("Y"), CM, Ident("X"),
                RP,
                RP,
                SC,
                Ident("I"), Dot, Ident("Am"), SC,
                End, SC
            ],
            superLexer.Feed(
                Text(
                    """
                    while
                                true
                         loop
                      Console
                        .
                            WriteLine
                                ( 
                                    X
                      .
                            Add 
                              (  

                                Y

                     ,
                                          X
                              )
                                )

                     I.Am
                                            end
                    """
                )
            )
        );
    }
}
