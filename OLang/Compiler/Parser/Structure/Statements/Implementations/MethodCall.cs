using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class MethodCall(
    Position position,
    Expression expression,
    string methodName,
    List<Expression> arguments
) : Expression(position)
{
    public Expression Expression { get; set; } = expression;
    public string MethodName { get; set; } = methodName;
    public List<Expression> Arguments { get; set; } = arguments;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}