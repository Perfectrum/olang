using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements.Implementations;

public class ConstructorInvocation(
    Position position,
    ClassName className,
    List<Expression> arguments
) : Expression(position)
{
    public ClassName ClassName { get; set; } = className;
    public List<Expression> Arguments { get; set; } = arguments;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}