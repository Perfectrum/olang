using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class BooleanNode(
    Position position,
    bool value
) : Expression(position)
{
    public bool Value { get; } = value;
        
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}