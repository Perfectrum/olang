using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class ClassName(
    Position position,
    string name,
    ClassName? genericArgument
) : Expression(position)
{
    public string Name { get; set; } = name;
    public ClassName? GenericArgument { get; set; } = genericArgument;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}