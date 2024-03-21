using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class ThisNode(
    Position position
) : Expression(position)
{
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}