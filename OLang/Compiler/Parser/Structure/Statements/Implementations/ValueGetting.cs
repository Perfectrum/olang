using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class ValueGetting(
    Position position,
    Expression expression,
    string fieldName
) : Expression(position)
{
    public Expression Expression { get; set; } = expression;
    public string FieldName { get; set; } = fieldName;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}