using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure;

internal class TemporaryListNode(Position position, List<Node> nodes) : Node(position)
{
    public List<Node> Nodes { get; } = nodes;
}