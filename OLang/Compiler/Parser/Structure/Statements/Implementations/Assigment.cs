using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class Assigment(
    Position position,
    Expression assignmentValue,
    Expression expression
) : Statement(position)
{
    public Expression AssignmentValue { get; set; } = assignmentValue;
    public Expression Expression { get; set; } = expression;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}