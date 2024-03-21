using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class Assigment(
    Position position,
    string variableName,
    Expression expression
) : Statement(position)
{
    public string VariableName { get; set; } = variableName;
    public Expression Expression { get; set; } = expression;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}