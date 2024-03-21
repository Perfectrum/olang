using OLang.Compiler.Lexer.Tokens;
using OLang.Compiler.Parser.Structure;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser;

internal class Scanner(IEnumerable<Token> tokens, TextWriter errorStream) : BaseScanner
{
    private readonly IEnumerator<Token> _tokens = tokens.GetEnumerator();

    public override void yyerror(string format, params object[] args)
    {
        errorStream.Write(format, args);
        errorStream.WriteLine($": {yylloc.Filename}:{yylloc.BeginLine}:{yylloc.BeginColumn}-{yylloc.EndColumn}");
    }

    public override int yylex()
    {
        if (_tokens.MoveNext() == false)
            return (int) TokenType.EOF;

        var token = _tokens.Current;

        yylval = token switch
        {
            BooleanLiteral booleanLiteral => new BooleanNode(token.Position, booleanLiteral.Value),
            Identifier identifier => new IdentifierNode(token.Position, identifier.Name),
            Integer integer => new IntegerNode(token.Position, integer.Value),
            Real real => new RealNode(token.Position, real.Value),
            StringLiteral stringLiteral => new StringLiteralNode(token.Position, stringLiteral.Text),
            Keyword keyword => keyword.Type switch
            {
                KeywordType.This => new KeywordWrapperNode(
                    token.Position, KeywordType.This),
                KeywordType.Base => new KeywordWrapperNode(
                    token.Position, KeywordType.Base),
                _ => null
            },
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
                KeywordType.Var => TokenType.VAR,
                KeywordType.This => TokenType.THIS,
                KeywordType.Base => TokenType.BASE,
                KeywordType.Using => TokenType.USING,
                KeywordType.Method => TokenType.METHOD,
                KeywordType.Is => TokenType.IS,
                KeywordType.End => TokenType.END,
                KeywordType.Return => TokenType.RETURN,
                KeywordType.While => TokenType.WHILE,
                KeywordType.Loop => TokenType.LOOP,
                KeywordType.If => TokenType.IF,
                KeywordType.Then => TokenType.THEN,
                KeywordType.Else => TokenType.ELSE,
                KeywordType.Static => TokenType.STATIC,
                KeywordType.Field => TokenType.FIELD,
                KeywordType.Function => TokenType.FUNCTION,
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new ArgumentOutOfRangeException()
        });
    }
}