using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class Variable(
    Position position,
    string name,
    ClassName? type,
    Expression expression
) : Statement(position)
{
    public string Name { get; set; } = name;
    public ClassName? Type { get; set; } = type;
    public Expression Expression { get; set; } = expression;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}