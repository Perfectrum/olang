using Xunit;
using System;
using System.Text;
using OLang.Compiler.Lexer.Tokens;
using System.Globalization;
using OLang.Compiler.Lexer;


public class LexerTest
{
    public ILexer lexer => new BasicLexer();
    public ILexer superLexer => new SuperLexer();
    
    public static Span EmptySpan => new(0, 0, 0);

    public static Token NewLine => new NewLine(EmptySpan);

    public static Token Ident(string str) => new Identifier(str, EmptySpan);

    public static Token Cls => new Keyword(KeywordType.Class, EmptySpan);
    public static Token This => new Keyword(KeywordType.This, EmptySpan);
    public static Token Var => new Keyword(KeywordType.Var, EmptySpan);
    public static Token Method => new Keyword(KeywordType.Method, EmptySpan);
    public static Token Is => new Keyword(KeywordType.Is, EmptySpan);
    public static Token End => new Keyword(KeywordType.End, EmptySpan);
    
    public static Token While => new Keyword(KeywordType.While, EmptySpan);

    public static Token Loop => new Keyword(KeywordType.Loop, EmptySpan);

    public static Token If => new Keyword(KeywordType.If, EmptySpan);
    public static Token Then => new Keyword(KeywordType.Then, EmptySpan);
    public static Token Else => new Keyword(KeywordType.Else, EmptySpan);

    public static Token True => new BooleanLiteral(true, EmptySpan);
    public static Token False => new BooleanLiteral(false, EmptySpan);

    public static Token Return => new Keyword(KeywordType.Return, EmptySpan);

    public static Token SC => new Symbol(";", SymbolType.Semicolon, EmptySpan);
    public static Token CL => new Symbol(":", SymbolType.Colon, EmptySpan);
    public static Token CM => new Symbol(",", SymbolType.Comma, EmptySpan);
    public static Token Dot => new Symbol(".", SymbolType.Dot, EmptySpan);
    public static Token LP => new Symbol("(", SymbolType.LP, EmptySpan);
    public static Token RP => new Symbol(")", SymbolType.RP, EmptySpan);

    public static Token Int(int value) => new Integer(value, EmptySpan);
    public static Token Real(double value) => new Real(value, EmptySpan);
    public static Token StringLiteral(string value) => new StringLiteral(value, EmptySpan);

    public static Stream Text(string str)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(byteArray);
    }
    
    public static void AssertEqual(Token[] expected, IEnumerable<Token> actual)
    {
        actual.Zip(expected, (a, b) => new { Expected = b, Actual = a with { Span = b.Span } })
              .ToList()
              .ForEach(pair => Assert.Equal(pair.Expected, pair.Actual));
    }

    public static void AssertEqual(Token[][] expected, IEnumerable<Token> actual)
    {
        actual.Zip(expected.SelectMany(x => x), (a, b) => new { Expected = b, Actual = a with { Span = b.Span } })
              .ToList()
              .ForEach(pair => Assert.Equal(pair.Expected, pair.Actual));
    }
}