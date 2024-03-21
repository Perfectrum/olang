using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class IntegerNode(
    Position position,
    int value
) : Expression(position)
{
    public int Value { get; } = value;

    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}