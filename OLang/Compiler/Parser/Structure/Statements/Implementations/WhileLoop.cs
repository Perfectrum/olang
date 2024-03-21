using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class WhileLoop(
    Position position,
    Expression condition,
    List<Statement> body
) : Statement(position)
{
    public Expression Condition { get; set; } = condition;
    public List<Statement> Body { get; set; } = body;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}