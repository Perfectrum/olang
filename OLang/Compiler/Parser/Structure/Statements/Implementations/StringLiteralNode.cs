using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class StringLiteralNode(
    Position position,
    string value
) : Expression(position)
{
    public string Value { get; } = value;
        
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}