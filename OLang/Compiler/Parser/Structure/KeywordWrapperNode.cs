using OLang.Compiler.Lexer.Tokens;
using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure;

internal class KeywordWrapperNode(
    Position position,
    KeywordType value
) : Node(position)
{
    public KeywordType Value { get; set; } = value;
}