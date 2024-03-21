
using OLang.Compiler.Lexer.Tokens;
using OLang.Compiler.Overall;

namespace OLang.Compiler.Lexer;

internal class SuperLexer : ILexer
{
    private static readonly HashSet<KeywordType> _afterKeywords = [
        KeywordType.Class,
        KeywordType.Extends,
        KeywordType.Is,
        KeywordType.Var,
        KeywordType.Method,
        KeywordType.While,
        KeywordType.Loop,
        KeywordType.If,
        KeywordType.Then,
        KeywordType.Else,
        KeywordType.Static,
        KeywordType.Field,
        KeywordType.Function
    ];
    private readonly static HashSet<SymbolType> _afterSymbols = [
        SymbolType.Semicolon,
        SymbolType.Comma,
        SymbolType.Colon,
        SymbolType.LP,
        SymbolType.LB,
        SymbolType.Dot,
        SymbolType.Asan
    ];

    private readonly static HashSet<KeywordType> _beforeKeywords = [
        KeywordType.Extends,
        KeywordType.Is,
        KeywordType.Then,
        KeywordType.Loop
    ];
    private readonly static HashSet<SymbolType> _beforeSymbols = [
        SymbolType.Comma,
        SymbolType.LP,
        SymbolType.RP,
        SymbolType.RB,
        SymbolType.Semicolon,
        SymbolType.Dot,
        SymbolType.Asan,
        SymbolType.Colon
    ];

    private static Position MakeSpanAfter(Position? span)
    {
        return span ?? new Position(0, 0, 0, 0, "");
    }

    private static bool ShouldNotAddSemicolonAfterToken(Token? token)
    {
        return token switch
        {
            null => true,
            Keyword k => _afterKeywords.Contains(k.Type),
            Symbol s => _afterSymbols.Contains(s.Type),
            _ => false
        };
    }

    private static bool ShouldNotAddSemicolonBeforeToken(Token? token)
    {
        return token switch
        {
            null => false,
            Keyword k => _beforeKeywords.Contains(k.Type),
            Symbol s => _beforeSymbols.Contains(s.Type),
            _ => false
        };
    }

    private readonly BasicLexer _basic = new();

    public IEnumerable<Token> Feed(Stream stream, string pathToSourceFile = "")
    {
        Token? prev = null;

        var iter = _basic.Feed(stream, pathToSourceFile).GetEnumerator();
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
                    yield return new Symbol(";", SymbolType.Semicolon, MakeSpanAfter(prev?.Position)); ;
                }
                hadNewLine = false;
            }
            yield return token;
            prev = token;
        }

        if (!ShouldNotAddSemicolonAfterToken(prev))
        {
            yield return new Symbol(";", SymbolType.Semicolon, MakeSpanAfter(prev?.Position));
        }
    }
}
