using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;

namespace OLang.Tests;

public class IntegrTest
{
    private Lexer lexer = new Lexer();

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

    [Fact (Skip = "Unary minus is not implemented yet")]
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

        for(int i = 0; i < 10_000; i++) {
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

        for(int i = 0; i < 1000; i++) {

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

            Assert.Equal(length*2-1, tokens.Count);

            for (int index = 0; index < tokens.Count; index++)
            {
                if (index % 2 == 0)
                    Assert.Equal(source[index/2], ((Integer)tokens[index]).Value);
                else
                    Assert.Equal(SymbolType.Comma, ((Symbol)tokens[index]).Type);
            }
        }
    }
}

public class RealTest
{
    private Lexer lexer = new Lexer();

    [Fact]
    public void Zero()
    {
        string str = "0.0";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

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

    [Fact (Skip = "Unary minus is not implemented yet")]
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
        string str = double.Epsilon.ToString("F500");
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

        for(int i = 0; i < 10_000; i++) {
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

        for(int i = 0; i < 100; i++) {

            int length = random.Next(1, 101);
            string str = "";

            double[] source = new double[length];
            for (int index = 0; index < length; index++)
            {
                // TODO: Когда (если) заработает унарный минус, нижней границей должно быть double.MinValue
                double x = random.NextDouble() * double.MaxValue;
                source[index] = x;
                str+= x.ToString("F", CultureInfo.InvariantCulture) + ",";
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(str[..^1]);
            MemoryStream stream = new MemoryStream(byteArray);

            var tokens = lexer.Feed(stream).ToList();

            Assert.Equal(length*2-1, tokens.Count);

            for (int index = 0; index < tokens.Count; index++)
            {
                if (index % 2 == 0)
                    Assert.Equal(source[index/2], ((Real)tokens[index]).Value);

                else
                    Assert.Equal(SymbolType.Comma, ((Symbol)tokens[index]).Type);
            }
        }
    }
}

public class MethodCall
{
    private Lexer lexer = new Lexer();

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
    private Lexer lexer = new Lexer();

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
        foreach(KeywordType keyword in keywords)
        {
            string str = "Something." + keyword.ToString().ToLower();
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream stream = new MemoryStream(byteArray);

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
    private Lexer lexer = new Lexer();

    [Fact]
    public void SingleBranch()
    {
        string str = "if true then return";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(4, tokens.Count);

        Assert.Equal(KeywordType.If, ((Keyword)tokens[0]).Type);
        Assert.Equal(KeywordType.True, ((Keyword)tokens[1]).Type);
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
        Assert.Equal(KeywordType.True, ((Keyword)tokens[1]).Type);
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

        Assert.Equal(16, tokens.Count);

        Assert.Equal(KeywordType.If, ((Keyword)tokens[0]).Type);
        Assert.Equal(KeywordType.True, ((Keyword)tokens[1]).Type);
        
        Assert.Equal(KeywordType.Then, ((Keyword)tokens[2]).Type);
        Assert.Equal(KeywordType.If, ((Keyword)tokens[3]).Type);
        Assert.Equal(KeywordType.True, ((Keyword)tokens[4]).Type);

        Assert.Equal(KeywordType.Then, ((Keyword)tokens[5]).Type);
        Assert.Equal(KeywordType.If, ((Keyword)tokens[6]).Type);
        Assert.Equal(KeywordType.True, ((Keyword)tokens[7]).Type);

        Assert.Equal(KeywordType.Then, ((Keyword)tokens[8]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[9]).Type);
        
        Assert.Equal(KeywordType.Else, ((Keyword)tokens[10]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[11]).Type);

        Assert.Equal(KeywordType.Else, ((Keyword)tokens[12]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[13]).Type);

        Assert.Equal(KeywordType.Else, ((Keyword)tokens[14]).Type);
        Assert.Equal(KeywordType.Return, ((Keyword)tokens[15]).Type);
    }
}

public class Loop
{
    private Lexer lexer = new Lexer();

    [Fact]
    public void EmptyInfiniteLoop()
    {
        string str = "while true loop end";
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        MemoryStream stream = new MemoryStream(byteArray);

        var tokens = lexer.Feed(stream).ToList();

        Assert.Equal(4, tokens.Count);

        Assert.Equal(KeywordType.While, ((Keyword)tokens[0]).Type);
        Assert.Equal(KeywordType.True, ((Keyword)tokens[1]).Type);
        Assert.Equal(KeywordType.Loop, ((Keyword)tokens[2]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[3]).Type);
    }
}

public class Class
{
    private Lexer lexer = new Lexer();

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

        Assert.Equal(25, tokens.Count);

        Assert.Equal(KeywordType.Class, ((Keyword)tokens[0]).Type);
        Assert.Equal("Human", ((Identifier)tokens[1]).Name);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[2]).Type);

        Assert.Equal(KeywordType.This, ((Keyword)tokens[3]).Type);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[4]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[6]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[7]).Type);

        Assert.Equal("Eat", ((Identifier)tokens[8]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[9]).Type);
        Assert.Equal("food", ((Identifier)tokens[10]).Name);
        Assert.Equal(SymbolType.Colon, ((Symbol)tokens[11]).Type);
        Assert.Equal("Food", ((Identifier)tokens[12]).Name);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[13]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[14]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[15]).Type);

        Assert.Equal("Sleep", ((Identifier)tokens[16]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[17]).Type);
        Assert.Equal("hours", ((Identifier)tokens[18]).Name);
        Assert.Equal(SymbolType.Colon, ((Symbol)tokens[19]).Type);
        Assert.Equal("Integer", ((Identifier)tokens[20]).Name);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[21]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[22]).Type);
        Assert.Equal(KeywordType.End, ((Keyword)tokens[23]).Type);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[24]).Type);
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

        Assert.Equal(20, tokens.Count);

        Assert.Equal(KeywordType.Class, ((Keyword)tokens[0]).Type);
        Assert.Equal("Test", ((Identifier)tokens[1]).Name);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[2]).Type);

        Assert.Equal(KeywordType.This, ((Keyword)tokens[3]).Type);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[4]).Type);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[5]).Type);
        Assert.Equal(KeywordType.Is, ((Keyword)tokens[6]).Type);

        Assert.Equal(KeywordType.Var, ((Keyword)tokens[7]).Type);
        Assert.Equal("x", ((Identifier)tokens[8]).Name);
        Assert.Equal(SymbolType.Colon, ((Symbol)tokens[9]).Type);

        Assert.Equal(3, ((Integer)tokens[10]).Value);
        Assert.Equal(SymbolType.Dot, ((Symbol)tokens[11]).Type);

        Assert.Equal("Add", ((Identifier)tokens[12]).Name);
        Assert.Equal(SymbolType.LP, ((Symbol)tokens[13]).Type);
        Assert.Equal(5.8, ((Real)tokens[14]).Value);
        Assert.Equal(SymbolType.Comma, ((Symbol)tokens[15]).Type);
        Assert.Equal(0, ((Integer)tokens[16]).Value);
        Assert.Equal(SymbolType.RP, ((Symbol)tokens[17]).Type);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[18]).Type);

        Assert.Equal(KeywordType.End, ((Keyword)tokens[19]).Type);
    }
}