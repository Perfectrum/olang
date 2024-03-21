using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class IfStatement(
    Position position,
    Expression condition,
    List<Statement> trueBlock,
    List<Statement>? falseBlock
) : Expression(position)
{
    public Expression Condition { get; set; } = condition;
    public List<Statement> TrueBlock { get; set; } = trueBlock;
    public List<Statement>? FalseBlock { get; set; } = falseBlock;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}