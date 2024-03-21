using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class ValueNode<T>(
    Position position,
    T value
) : Expression(position)
{
    public T Value { get; } = value;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}