using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Statements;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser.Structure.Members.Implementations;

public class Field(
    Position position,
    // TODO: не уверен, что будет удобно
    bool isStatic,
    string name,
    ClassName? type,
    Expression expression
) : MemberDeclaration(position)
{
    public bool IsStatic { get; set; } = isStatic;
    public string Name { get; set; } = name;
    public ClassName? Type { get; set; } = type;
    public Expression Expression { get; set; } = expression;
    
    public override void Accept(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}