using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure;

public abstract class Node(Position position)
{
    public Position Position { get; set; } = position;
}