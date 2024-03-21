using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser.Structure.Members.Implementations;

public class Parameter(
    Position position,
    string name,
    ClassName type
) : Node(position)
{
    public string Name { get; set; } = name;
    public ClassName Type { get; set; } = type;
}