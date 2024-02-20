using ObjectLanguage.Compiler.Lexer.Tokens;

namespace ObjectLanguage.Compiler.Lexer;

public class Lexer
{
    private readonly Stream _sourceFile;

    public Lexer(Stream sourceFile) => _sourceFile = sourceFile;

    public IEnumerable<BaseToken> Analyze()
    {
        yield break;
    }
}
