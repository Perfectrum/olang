
using OLang.Compiler.Lexer.Tokens;

namespace OLang.Compiler.Lexer;

public interface ILexer
{
    public IEnumerable<Token> Feed(Stream sourceFile);

    // А что? Могу себе позволить
    public static ILexer GetLexer() {
        return new SuperLexer();
    }
}
