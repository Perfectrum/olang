using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class RealNode(
    Position position,
    double value
) : Expression(position)
{
    public double Value { get; } = value;

    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}