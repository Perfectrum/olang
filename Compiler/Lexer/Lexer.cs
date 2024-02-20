using System.Collections.ObjectModel;
using ObjectLanguage.Compiler.Lexer.Tokens;

namespace ObjectLanguage.Compiler.Lexer;

public class Lexer
{
    private static readonly ReadOnlyDictionary<string, KeywordType> _keywordTypes = new Dictionary<string, KeywordType>
    {
        ["class"] = KeywordType.Class,
        ["extends"] = KeywordType.Extends,
        ["var"] = KeywordType.Var,
        ["this"] = KeywordType.This,
        ["method"] = KeywordType.Method,
        ["is"] = KeywordType.Is,
        ["end"] = KeywordType.End,
        ["return"] = KeywordType.Return,
        ["while"] = KeywordType.While,
        ["loop"] = KeywordType.Loop,
        ["true"] = KeywordType.True,
        ["false"] = KeywordType.False,
        ["if"] = KeywordType.If,
        ["then"] = KeywordType.Then,
        ["else"] = KeywordType.Else,
    }.AsReadOnly();
    
    private readonly Stream _sourceFile;

    public Lexer(Stream sourceFile) => _sourceFile = sourceFile;

    public IEnumerable<BaseToken> Analyze()
    {
        yield break;
    }
}
