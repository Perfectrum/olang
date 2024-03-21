using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class ReturnStatement(
    Position position,
    Expression? expression
) : Statement(position)
{
    public Expression? Expression { get; set; } = expression;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}