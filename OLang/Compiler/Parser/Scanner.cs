using OLang.Compiler.Lexer.Tokens;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser;

internal class Scanner(IEnumerable<Token> tokens, TextWriter _errorStream) : BaseScanner
{
    private readonly IEnumerator<Token> _tokens = tokens.GetEnumerator();

    public override void yyerror(string format, params object[] args)
    {
        _errorStream.Write(format, args);
        _errorStream.WriteLine($": {yylloc}");
    }

    public override int yylex()
    {
        if (_tokens.MoveNext() == false)
            return (int) TokenType.EOF;

        var token = _tokens.Current;

        yylval = token switch
        {
            BooleanLiteral booleanLiteral => new ValueNode<bool>(token.Position, booleanLiteral.Value),
            Identifier identifier => new ValueNode<string>(token.Position, identifier.Name),
            Integer integer => new ValueNode<int>(token.Position, integer.Value),
            Real real => new ValueNode<double>(token.Position, real.Value),
            StringLiteral stringLiteral => new ValueNode<string>(token.Position, stringLiteral.Text),
            _ => null
        };

        yylloc = token.Position;
        
        return (int)(token switch
        {
            BooleanLiteral => TokenType.BOOLEAN,
            Identifier => TokenType.IDENTIFIER,
            Integer => TokenType.INTEGER,
            Real => TokenType.REAL,
            StringLiteral => TokenType.STRING,
            Symbol symbol => symbol.Type switch
            {
                SymbolType.Comma => TokenType.COMMA,
                SymbolType.Colon => TokenType.COLON,
                SymbolType.Semicolon => TokenType.SEMICOLON,
                SymbolType.LP => TokenType.LPARENT,
                SymbolType.RP => TokenType.RPARENT,
                SymbolType.LB => TokenType.LBRACKET,
                SymbolType.RB => TokenType.RBRACKET,
                SymbolType.Dot => TokenType.DOT,
                SymbolType.Asan => TokenType.ASAN,
                _ => throw new ArgumentOutOfRangeException()
            },
            Keyword keyword => keyword.Type switch
            {
                KeywordType.Class => TokenType.CLASS,
                KeywordType.Extends => TokenType.EXTENDS,
                KeywordType.Var => TokenType.LET,
                KeywordType.This => TokenType.THIS,
                KeywordType.Method => TokenType.METHOD,
                KeywordType.Is => TokenType.IS,
                KeywordType.End => TokenType.END,
                KeywordType.Return => TokenType.RETURN,
                KeywordType.While => TokenType.WHILE,
                KeywordType.Loop => TokenType.LOOP,
                KeywordType.If => TokenType.IF,
                KeywordType.Then => TokenType.THEN,
                KeywordType.Else => TokenType.ELSE,
                KeywordType.Super => TokenType.SUPER,
                KeywordType.Static => TokenType.STATIC,
                KeywordType.Field => TokenType.FIELD,
                KeywordType.Function => TokenType.FUNCTION,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        });
    }
}