using OLang.Compiler.Lexer.Tokens;
using QUT.Gppg;

namespace OLang.Compiler.Parser;

internal class Scanner(IEnumerable<Token> tokens, StreamWriter errorStream) : BaseScanner
{
    private readonly IEnumerator<Token> _tokens = tokens.GetEnumerator();
    private readonly StreamWriter _errorStream = errorStream;

    public override void yyerror(string format, params object[] args)
    {
        _errorStream.Write(format, args);
        _errorStream.WriteLine($": {yylloc.StartLine}-{yylloc.EndLine}:{yylloc.StartColumn}-{yylloc.EndColumn}");
    }

    public override int yylex()
    {
        if (_tokens.MoveNext() == false)
            return (int) TokenType.EOF;

        var token = _tokens.Current;

        yylval = token switch
        {
            BooleanLiteral booleanLiteral => new ValueNode<bool>(booleanLiteral.Value),
            Identifier identifier => new ValueNode<string>(identifier.Name),
            Integer integer => new ValueNode<int>(integer.Value),
            Real real => new ValueNode<double>(real.Value),
            StringLiteral stringLiteral => new ValueNode<string>(stringLiteral.Text),
            _ => null
        };

        // TODO: мейби легче подсунуть наш Span и реализовать просто IMerge?
        yylloc = new LexLocation(
            (int)token.Span.LineNumber, (int)token.Span.LineNumber,
            token.Span.BeginPosition, token.Span.EndPosition
        );
        
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
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        });
    }
}