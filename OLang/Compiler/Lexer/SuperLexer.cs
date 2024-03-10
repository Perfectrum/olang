
using OLang.Compiler.Lexer.Tokens;

namespace OLang.Compiler.Lexer;

internal class SuperLexer : ILexer
{

    private static Span MakeSpanAfter(Span? span)
    {
        if (span.HasValue)
        {
            var s = span.Value;
            return new(s.LineNumber, s.EndPosition, s.EndPosition);
        }
        return new(0, 0, 0);
    }

    private static bool ShouldNotAddSemicolonAfterToken(Token? token)
    {
        if (token is null) return true;
        return token switch
        {
            Keyword { Type: KeywordType.Class } => true,
            Keyword { Type: KeywordType.Extends } => true,
            Keyword { Type: KeywordType.Is } => true,
            Keyword { Type: KeywordType.Var } => true,
            Keyword { Type: KeywordType.Method } => true,
            Keyword { Type: KeywordType.While } => true,
            Keyword { Type: KeywordType.Loop } => true,
            Keyword { Type: KeywordType.If } => true,
            Keyword { Type: KeywordType.Then } => true,
            Keyword { Type: KeywordType.Else } => true,
            Keyword { Type: KeywordType.Static } => true,
            Keyword { Type: KeywordType.Field } => true,
            Keyword { Type: KeywordType.Function } => true,
            Symbol { Type: SymbolType.Semicolon } => true,
            Symbol { Type: SymbolType.Comma } => true,
            Symbol { Type: SymbolType.Colon } => true,
            Symbol { Type: SymbolType.LP } => true,
            Symbol { Type: SymbolType.LB } => true,
            Symbol { Type: SymbolType.Dot } => true,
            Symbol { Type: SymbolType.Asan } => true,
            _ => false
        };
    }

    private static bool ShouldNotAddSemicolonBeforeToken(Token? token)
    {
        if (token is null) return false;
        return token switch
        {
            Keyword { Type: KeywordType.Extends } => true,
            Keyword { Type: KeywordType.Is } => true,
            Keyword { Type: KeywordType.Then } => true,
            Keyword { Type: KeywordType.Loop } => true,
            Symbol { Type: SymbolType.Comma } => true,
            Symbol { Type: SymbolType.LP } => true,
            Symbol { Type: SymbolType.RP } => true,
            Symbol { Type: SymbolType.RB } => true,
            Symbol { Type: SymbolType.Semicolon } => true,
            Symbol { Type: SymbolType.Dot } => true,
            Symbol { Type: SymbolType.Asan } => true,
            Symbol { Type: SymbolType.Colon } => true,
            _ => false
        };
    }

    private readonly BasicLexer _basic = new();
    public IEnumerable<Token> Feed(Stream sourceFile)
    {
        Token? prev = null;

        var iter = _basic.Feed(sourceFile).GetEnumerator();
        bool hadNewLine = false;
        while (iter.MoveNext())
        {
            var token = iter.Current;
            if (token is NewLine)
            {
                hadNewLine = true;
                continue;
            }
            if (hadNewLine)
            {
                if (!(ShouldNotAddSemicolonAfterToken(prev) || ShouldNotAddSemicolonBeforeToken(token)))
                {
                    yield return new Symbol(";", SymbolType.Semicolon, MakeSpanAfter(prev?.Span)); ;
                }
                hadNewLine = false;
            }
            yield return token;
            prev = token;
        }

        if (!ShouldNotAddSemicolonAfterToken(prev))
        {
            yield return new Symbol(";", SymbolType.Semicolon, MakeSpanAfter(prev?.Span));
        }
    }
}
