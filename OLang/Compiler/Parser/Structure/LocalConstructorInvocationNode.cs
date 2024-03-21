using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Statements;

namespace OLang.Compiler.Parser.Structure;

public enum LocalConstructorIdentifierType
{
    This,
    Base,
}

public class LocalConstructorInvocationNode(
    Position position,
    LocalConstructorIdentifierType constructorIdentifierType,
    List<Expression> arguments
) : Node(position)
{
    public LocalConstructorIdentifierType ConstructorType { get; set; } = constructorIdentifierType;
    public List<Expression> Arguments { get; set; } = arguments;
}