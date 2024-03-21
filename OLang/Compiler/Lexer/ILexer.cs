
using OLang.Compiler.Lexer.Tokens;

namespace OLang.Compiler.Lexer;

public interface ILexer
{
    public IEnumerable<Token> Feed(Stream stream, string pathToSourceFile = "");
}
