using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;
using OLang.Compiler.Overall;

namespace OLang.Tests;

public static class Comparator
{
    public static void Compare(Token[][] expected, IEnumerable<Token> actual)
    {
        var iter = actual.GetEnumerator();
        foreach (var line in expected)
        {
            foreach (var token in line)
            {
                Assert.True(iter.MoveNext());
                var b = iter.Current;
                var a = token with { Position = b.Position };
                Assert.Equal(a, b);
            }
        }
    }
}

public static class R
{

    private static Position ZeroPosition => new(0, 0, 0, 0, "");

    public static Token[][] P(params Token[][] xs) => xs;
    public static Token[] L(params Token[] xs) => xs;

    public static Stream Text(string str)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(byteArray);
    }

    public static Token Ident(string str) => new Identifier(str, ZeroPosition);

    public static Token Cls => new Keyword(KeywordType.Class, ZeroPosition);
    public static Token Method => new Keyword(KeywordType.Method, ZeroPosition);
    public static Token While => new Keyword(KeywordType.While, ZeroPosition);
    public static Token Loop => new Keyword(KeywordType.Loop, ZeroPosition);
    public static Token End => new Keyword(KeywordType.End, ZeroPosition);
    public static Token Is => new Keyword(KeywordType.Is, ZeroPosition);

    public static Token True => new BooleanLiteral(true, ZeroPosition);
    public static Token False => new BooleanLiteral(false, ZeroPosition);

    public static Token SC => new Symbol(";", SymbolType.Semicolon, ZeroPosition);
    public static Token CM => new Symbol(",", SymbolType.Comma, ZeroPosition);
    public static Token Dot => new Symbol(".", SymbolType.Dot, ZeroPosition);
    public static Token LP => new Symbol("(", SymbolType.LP, ZeroPosition);
    public static Token RP => new Symbol(")", SymbolType.RP, ZeroPosition);

}

public class IntegerTest
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void Zero()
    {
        string str = "0";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(0, ((Integer)tokens[0]).Value);
    }

    [Fact]
    public void NegativeZero()
    {
        string str = "-0";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(0, ((Integer)tokens[0]).Value);
    }

    [Fact]
    public void MaxInteger()
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(int.MaxValue.ToString());
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(int.MaxValue, ((Integer)tokens[0]).Value);
    }

    [Fact(Skip = "Unary minus is not implemented yet")]
    public void MinInteger()
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(int.MinValue.ToString());
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(int.MinValue, ((Integer)tokens[0]).Value);
    }

    [Fact]
    public void SingleInteger()
    {
        Random random = new Random();

        for (int i = 0; i < 10_000; i++)
        {
            // TODO: Когда (если) заработает унарный минус, нижней границей должно быть int.MinValue
            int x = random.Next(0, int.MaxValue);

            byte[] byteArray = Encoding.UTF8.GetBytes(x.ToString());
            MemoryStream stream = new MemoryStream(byteArray);

            var tokens = lexer.Feed(stream).ToList();

            Assert.Single(tokens);
            Assert.Equal(x, ((Integer)tokens[0]).Value);
        }
    }

    [Fact]
    public void IntegersList()
    {
        Random random = new Random();

        for (int i = 0; i < 1000; i++)
        {

            int length = random.Next(1, 101);

            int[] source = new int[length];
            for (int index = 0; index < length; index++)
            {
                // TODO: Когда (если) заработает унарный минус, нижней границей должно быть int.MinValue
                source[index] = random.Next(0, int.MaxValue);
            }

            string str = string.Join(",", source);
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);

            var tokens = lexer.Feed(stream).ToList();

            Assert.Equal(length * 2 - 1, tokens.Count);

            for (int index = 0; index < tokens.Count; index++)
            {
                if (index % 2 == 0)
                    Assert.Equal(source[index / 2], ((Integer)tokens[index]).Value);
                else
                    Assert.Equal(SymbolType.Comma, ((Symbol)tokens[index]).Type);
            }
        }
    }
}

public class RealTest
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void Zero()
    {
        string str = "0.0";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(0.0, ((Real)tokens[0]).Value);
    }

    [Fact]
    public void NegativeZero()
    {
        string str = "-0.0";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(0.0, ((Real)tokens[0]).Value);
    }

    [Fact]
    public void MaxReal()
    {
        string str = double.MaxValue.ToString("F", CultureInfo.InvariantCulture);
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(double.MaxValue, ((Real)tokens[0]).Value);
    }

    [Fact(Skip = "Unary minus is not implemented yet")]
    public void MinReal()
    {
        string str = double.MinValue.ToString("F", CultureInfo.InvariantCulture);
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(double.MinValue, ((Real)tokens[0]).Value);
    }

    [Fact]
    public void Epsilon()
    {
        string str = double.Epsilon.ToString("F500", CultureInfo.InvariantCulture);
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Single(tokens);
        Assert.Equal(double.Epsilon, ((Real)tokens[0]).Value);
    }

    [Fact]
    public void SingleReal()
    {
        Random random = new Random();

        for (int i = 0; i < 10_000; i++)
        {
            // TODO: Когда (если) заработает унарный минус, нижней границей должно быть double.MinValue
            double x = random.NextDouble() * double.MaxValue;

            byte[] byteArray = Encoding.UTF8.GetBytes(x.ToString("F", CultureInfo.InvariantCulture));
            MemoryStream stream = new MemoryStream(byteArray);

            var tokens = lexer.Feed(stream).ToList();

            Assert.Single(tokens);
            Assert.Equal(x, ((Real)tokens[0]).Value);
        }
    }

    [Fact]
    public void RealsList()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {

            int length = random.Next(1, 101);
            string str = "";

            double[] source = new double[length];
            for (int index = 0; index < length; index++)
            {
                // TODO: Когда (если) заработает унарный минус, нижней границей должно быть double.MinValue
                double x = random.NextDouble() * double.MaxValue;
                source[index] = x;
                str += x.ToString("F", CultureInfo.InvariantCulture) + ",";
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(str[..^1]);
            MemoryStream stream = new MemoryStream(byteArray);

            var tokens = lexer.Feed(stream).ToList();

            Assert.Equal(length * 2 - 1, tokens.Count);

            for (int index = 0; index < tokens.Count; index++)
            {
                if (index % 2 == 0)
                    Assert.Equal(source[index / 2], ((Real)tokens[index]).Value);

                else
                    Assert.Equal(SymbolType.Comma, ((Symbol)tokens[index]).Type);
            }
        }
    }
}

public class MethodCall
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void IntegerMethod()
    {
        string str = "2.Add(2)";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal(2, ((Integer)tokens[0]).Value);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Add", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal(2, ((Integer)tokens[4]).Value);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void RealMethod()
    {
        string str = "2.0.Add(2.0)";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal(2.0, ((Real)tokens[0]).Value);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Add", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal(2.0, ((Real)tokens[4]).Value);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void StringMethod()
    {
        string str = "\"Hello \".Add(\"world\")";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal("Hello ", ((StringLiteral)tokens[0]).Text);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Add", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal("world", ((StringLiteral)tokens[4]).Text);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void NoArguments()
    {
        string str = "Someone.DoSomething()";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(5, tokens.Count);

        Assert.Equal("Someone", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("DoSomething", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[4]).Type);
    }

    [Fact]
    public void SingleIdentifierArgument()
    {
        string str = "Someone.DoSomething(argument)";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal("Someone", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("DoSomething", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal("argument", ((Identifier)tokens[4]).Name);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void SingleIntegerArgument()
    {
        string str = "Someone.DoSomething(42)";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal("Someone", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("DoSomething", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal(42, ((Integer)tokens[4]).Value);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void SingleRealArgument()
    {
        string str = "Someone.DoSomething(3.14)";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal("Someone", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("DoSomething", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal(3.14, ((Real)tokens[4]).Value);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void SingleStringArgument()
    {
        string str = "Someone.DoSomething(\"argument\")";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal("Someone", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("DoSomething", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);
        Assert.Equal("argument", ((StringLiteral)tokens[4]).Text);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
    }

    [Fact]
    public void MultipleArguments()
    {
        string str = "Someone.DoSomething(\"many different arguments!\", 42, 3.14, \"wow!\", Identifier )";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(14, tokens.Count);

        Assert.Equal("Someone", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("DoSomething", ((Identifier)tokens[2]).Name);

        Assert.Equal(SymbolType.LP, ((Symbol)tokens[3]).Type);

        Assert.Equal("many different arguments!", ((StringLiteral)tokens[4]).Text);
        Assert.Equal(SymbolType.Comma, ((Symbol)tokens[5]).Type);

        Assert.Equal(42, ((Integer)tokens[6]).Value);
        Assert.Equal(SymbolType.Comma, ((Symbol)tokens[7]).Type);

        Assert.Equal(3.14, ((Real)tokens[8]).Value);
        Assert.Equal(SymbolType.Comma, ((Symbol)tokens[9]).Type);

        Assert.Equal("wow!", ((StringLiteral)tokens[10]).Text);
        Assert.Equal(SymbolType.Comma, ((Symbol)tokens[11]).Type);

        Assert.Equal("Identifier", ((Identifier)tokens[12]).Name);

        Assert.Equal(SymbolType.RP, ((Symbol)tokens[13]).Type);
    }
}

public class DotCall
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void SimpleField()
    {
        string str = "Something.Field";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(3, tokens.Count);

        Assert.Equal("Something", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Field", ((Identifier)tokens[2]).Name);
    }

    [Fact]
    public void IntegerField()
    {
        string str = "2.Sign";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(3, tokens.Count);

        Assert.Equal(2, ((Integer)tokens[0]).Value);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Sign", ((Identifier)tokens[2]).Name);
    }

    [Fact]
    public void RealField()
    {
        string str = "2.0.Sign";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(3, tokens.Count);

        Assert.Equal(2.0, ((Real)tokens[0]).Value);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Sign", ((Identifier)tokens[2]).Name);
    }

    [Fact]
    public void StringField()
    {
        string str = "\"Hello, world!\".Lenght";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(3, tokens.Count);

        Assert.Equal("Hello, world!", ((StringLiteral)tokens[0]).Text);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Lenght", ((Identifier)tokens[2]).Name);
    }

    [Fact]
    public void KeywordAsField()
    {
        Array keywords = Enum.GetValues(typeof(KeywordType));
        foreach (KeywordType keyword in keywords)
        {
            string str = "Something." + keyword.ToString().ToLower();
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new(byteArray);

            var tokens = lexer.Feed(stream).ToList();

            Assert.Equal(3, tokens.Count);

            Assert.Equal("Something", ((Identifier)tokens[0]).Name);
            Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
            Assert.Equal(keyword, ((Keyword)tokens[2]).Type);
        }
    }

    [Fact]
    public void Chain()
    {
        string str = "Something.Field.Method().Field.Again(arg)";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(14, tokens.Count);

        Assert.Equal("Something", ((Identifier)tokens[0]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[1]).Type);
        Assert.Equal("Field", ((Identifier)tokens[2]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[3]).Type);
        Assert.Equal("Method", ((Identifier)tokens[4]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[5]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[6]).Type);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[7]).Type);
        Assert.Equal("Field", ((Identifier)tokens[8]).Name);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[9]).Type);
        Assert.Equal("Again", ((Identifier)tokens[10]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[11]).Type);
        Assert.Equal("arg", ((Identifier)tokens[12]).Name);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[13]).Type);
    }
}

public class Condition
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void SingleBranch()
    {
        string str = "if true then return";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(4, tokens.Count);

        Assert.Equal(KeywordType.If, ((Keyword)tokens[0]).Type);
        Assert.True(((BooleanLiteral)tokens[1]).Value);
        Assert.Equal(KeywordType.Then, ((Keyword)tokens[2]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[3]).Type);
    }

    [Fact]
    public void IfElse()
    {
        string str = "if true then return else return";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(6, tokens.Count);

        Assert.Equal(KeywordType.If, ((Keyword)tokens[0]).Type);
        Assert.True(((BooleanLiteral)tokens[1]).Value);
        Assert.Equal(KeywordType.Then, ((Keyword)tokens[2]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[3]).Type);
        Assert.Equal(KeywordType.Else, ((Keyword)tokens[4]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[5]).Type);
    }


    [Fact]
    public void Deep()
    {
        string str = """
        if true
            then if true
                then if true 
                    then return
                else return
            else return
        else return
        """;
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(22, tokens.Count);

        Assert.Equal(KeywordType.If, ((Keyword)tokens[0]).Type);
        Assert.True(((BooleanLiteral)tokens[1]).Value);
        Assert.IsType<NewLine>(tokens[2]);

        Assert.Equal(KeywordType.Then, ((Keyword)tokens[3]).Type);
        Assert.Equal(KeywordType.If, ((Keyword)tokens[4]).Type);
        Assert.True(((BooleanLiteral)tokens[5]).Value);
        Assert.IsType<NewLine>(tokens[6]);

        Assert.Equal(KeywordType.Then, ((Keyword)tokens[7]).Type);
        Assert.Equal(KeywordType.If, ((Keyword)tokens[8]).Type);
        Assert.True(((BooleanLiteral)tokens[9]).Value);
        Assert.IsType<NewLine>(tokens[10]);

        Assert.Equal(KeywordType.Then, ((Keyword)tokens[11]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[12]).Type);
        Assert.IsType<NewLine>(tokens[13]);

        Assert.Equal(KeywordType.Else, ((Keyword)tokens[14]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[15]).Type);
        Assert.IsType<NewLine>(tokens[16]);

        Assert.Equal(KeywordType.Else, ((Keyword)tokens[17]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[18]).Type);
        Assert.IsType<NewLine>(tokens[19]);

        Assert.Equal(KeywordType.Else, ((Keyword)tokens[20]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[21]).Type);
    }
}

public class Loop
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void EmptyInfiniteLoop()
    {
        string str = "while true loop end";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(4, tokens.Count);

        Assert.Equal(KeywordType.While, ((Keyword)tokens[0]).Type);
        Assert.True(((BooleanLiteral)tokens[1]).Value);
        Assert.Equal(KeywordType.Loop, ((Keyword)tokens[2]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[3]).Type);
    }
}

public class Class
{
    private readonly BasicLexer lexer = new();

    [Fact]
    public void EmptyClass()
    {
        string str = "class Test is end";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(4, tokens.Count);

        Assert.Equal(KeywordType.Class, ((Keyword)tokens[0]).Type);
        Assert.Equal("Test", ((Identifier)tokens[1]).Name);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[2]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[3]).Type);
    }

    [Fact]
    public void EmptyConstructor()
    {
        string str = "class Test is this() is end end";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(9, tokens.Count);

        Assert.Equal(KeywordType.Class, ((Keyword)tokens[0]).Type);
        Assert.Equal("Test", ((Identifier)tokens[1]).Name);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[2]).Type);

        Assert.Equal(KeywordType.This, ((Keyword)tokens[3]).Type);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[4]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);

        Assert.Equal(KeywordType.Is, ((Keyword)tokens[6]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[7]).Type);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[8]).Type);
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
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(29, tokens.Count);

        Assert.Equal(KeywordType.Class, ((Keyword)tokens[0]).Type);
        Assert.Equal("Human", ((Identifier)tokens[1]).Name);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[2]).Type);
        Assert.IsType<NewLine>(tokens[3]);

        Assert.Equal(KeywordType.This, ((Keyword)tokens[4]).Type);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[5]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[6]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[7]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[8]).Type);
        Assert.IsType<NewLine>(tokens[9]);

        Assert.Equal("Eat", ((Identifier)tokens[10]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[11]).Type);
        Assert.Equal("food", ((Identifier)tokens[12]).Name);
        Assert.Equal(SymbolType.Colon, ((Symbol)tokens[13]).Type);
        Assert.Equal("Food", ((Identifier)tokens[14]).Name);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[15]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[16]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[17]).Type);
        Assert.IsType<NewLine>(tokens[18]);

        Assert.Equal("Sleep", ((Identifier)tokens[19]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[20]).Type);
        Assert.Equal("hours", ((Identifier)tokens[21]).Name);
        Assert.Equal(SymbolType.Colon, ((Symbol)tokens[22]).Type);
        Assert.Equal("Integer", ((Identifier)tokens[23]).Name);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[24]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[25]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[26]).Type);
        Assert.IsType<NewLine>(tokens[27]);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[28]).Type);
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
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(24, tokens.Count);

        Assert.Equal(KeywordType.Class, ((Keyword)tokens[0]).Type);
        Assert.Equal("Test", ((Identifier)tokens[1]).Name);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[2]).Type);
        Assert.IsType<NewLine>(tokens[3]);

        Assert.Equal(KeywordType.This, ((Keyword)tokens[4]).Type);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[5]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[6]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[7]).Type);
        Assert.IsType<NewLine>(tokens[8]);

        Assert.Equal(KeywordType.Var, ((Keyword)tokens[9]).Type);
        Assert.Equal("x", ((Identifier)tokens[10]).Name);
        Assert.Equal(SymbolType.Colon, ((Symbol)tokens[11]).Type);

        Assert.Equal(3, ((Integer)tokens[12]).Value);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[13]).Type);

        Assert.Equal("Add", ((Identifier)tokens[14]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[15]).Type);
        Assert.Equal(5.8, ((Real)tokens[16]).Value);
        Assert.Equal(SymbolType.Comma, ((Symbol)tokens[17]).Type);
        Assert.Equal(0, ((Integer)tokens[18]).Value);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[19]).Type);
        Assert.IsType<NewLine>(tokens[20]);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[21]).Type);
        Assert.IsType<NewLine>(tokens[22]);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[23]).Type);
    }
}

public class ExtendedLexer
{

    private static SuperLexer Instance() => new();

    [Fact]
    public void Easy()
    {
        Comparator.Compare(
            R.P(
                R.L(R.While, R.True, R.Loop),
                R.L(R.Ident("Smth"), R.Dot, R.Ident("Test"), R.LP, R.RP, R.SC),
                R.L(R.Ident("Smth"), R.Dot, R.Ident("MoreTest"), R.LP, R.RP, R.SC),
                R.L(R.End, R.SC)
            ),
            Instance().Feed(
                R.Text(
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
        Comparator.Compare(
            R.P(
                R.L(R.Cls, R.Ident("A"), R.Is),
                R.L(R.Method, R.Ident("X"), R.LP, R.RP, R.Is, R.End, R.SC),
                R.L(R.Method, R.Ident("X"), R.LP, R.RP, R.Is, R.End, R.SC),
                R.L(R.End, R.SC),

                R.L(R.Cls, R.Ident("B"), R.Is),
                R.L(R.End, R.SC),

                R.L(R.Cls, R.Ident("C"), R.Is),
                R.L(R.End, R.SC)
            ),
            Instance().Feed(
                R.Text(
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
        Comparator.Compare(
            R.P(
                R.L(R.While, R.True, R.Loop),
                R.L(R.Ident("Console"), R.Dot, R.Ident("WriteLine")),
                R.L(R.LP),
                R.L(R.Ident("X"), R.Dot, R.Ident("Add")),
                R.L(R.LP),
                R.L(R.Ident("Y"), R.CM, R.Ident("X")),
                R.L(R.RP),
                R.L(R.RP),
                R.L(R.SC),
                R.L(R.Ident("I"), R.Dot, R.Ident("Am"), R.SC),
                R.L(R.End, R.SC)
            ),
            Instance().Feed(
                R.Text(
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