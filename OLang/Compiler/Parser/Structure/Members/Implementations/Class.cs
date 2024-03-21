using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser.Structure.Members.Implementations;

public class Class(
    Position position,
    ClassName name,
    ClassName? extends,
    List<MemberDeclaration> members
) : Node(position)
{
    public ClassName Name { get; set; } = name;
    public ClassName? Extends { get; set; } = extends;
    public List<MemberDeclaration> Members { get; set; } = members;
}