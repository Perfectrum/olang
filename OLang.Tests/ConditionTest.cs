using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class Condition: LexerTest
{
    [Fact]
    public void SingleBranch()
    {
        AssertEqual(
            [If, True, Then, Return],
            lexer.Feed(Text("if true then return"))
        );
    }

    [Fact]
    public void IfElse()
    {
        AssertEqual(
            [If, True, Then, Return, Else, Return],
            lexer.Feed(Text("if true then return else return"))
        );
    }

    [Fact]
    public void Deep()
    {
        AssertEqual(
                [If, True, NewLine,
                    Then, If, True, NewLine,
                        Then, If, True, NewLine,
                            Then, Return, NewLine,
                        Else, Return, NewLine,
                    Else, Return, NewLine,
                Else, Return],
            lexer.Feed(Text("""
                if true
                    then if true
                        then if true 
                            then return
                        else return
                    else return
                else return
            """))
        );
    }
}