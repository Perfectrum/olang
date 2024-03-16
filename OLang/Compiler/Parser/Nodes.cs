using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser;

using Classes = List<Class>;
using Parameters = List<Parameter>;
using Body = List<Statement>;
using Members = List<MemberDeclaration>;
using Arguments = List<Expression>;

public interface IStatementVisitor
{
    void Visit(IfStatement ifStatement);
    void Visit(Variable variable);
    void Visit(Assigment assigment);
    void Visit(WhileLoop whileLoop);
    void Visit(ReturnStatement returnStatement);
    void Visit(ConstructorInvocation constructorInvocation);
    void Visit(MethodCall methodCall);
    void Visit(ValueGetting valueGetting);
    void Visit<T>(ValueNode<T> valueNode);
    void Visit(ClassName className);
}

public interface IMemberDeclarationVisitor
{
    void Visit(Field field);
    void Visit(Constructor constructor);
    void Visit(Method method);
}

internal class TemporaryListNode(Position position, List<Node> nodes) : Node(position)
{
    public List<Node> Nodes { get; } = nodes;
}

public abstract class Node(Position position)
{
    public Position Position { get; set; } = position;
}

public class Program(Position position, Classes classes) : Node(position)
{
    public Classes Classes { get; } = classes;
}

public abstract class MemberDeclaration(Position position) : Node(position)
{
    public abstract void Accept(IMemberDeclarationVisitor visitor);
}

public class Parameter(
    Position position,
    string name,
    ClassName type
) : Node(position)
{
    public string Name { get; set; } = name;
    public ClassName Type { get; set; } = type;
}

public class Field(
    Position position,
    // TODO: не уверен, что будет удобно
    bool isStatic,
    string name,
    ClassName? type,
    Expression expression
) : MemberDeclaration(position)
{
    public bool IsStatic { get; set; } = isStatic;
    public string Name { get; set; } = name;
    public ClassName? Type { get; set; } = type;
    public Expression Expression { get; set; } = expression;
    
    public override void Accept(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}

public class Constructor(
    Position position,
    Parameters parameters,
    Body body
) : MemberDeclaration(position)
{
    public Parameters Parameters { get; } = parameters;
    public Body Body { get; } = body;
    
    public override void Accept(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}

public class Method(
    Position position,
    // TODO: тоже не уверен, что будет удобно
    bool isStatic,
    string name,
    Parameters parameters,
    ClassName? returnType,
    Body body
) : MemberDeclaration(position)
{
    public bool IsStatic { get; set; } = isStatic;
    public string Name { get; set; } = name;
    public Parameters Parameters { get; } = parameters;
    public ClassName? ReturnType { get; set; } = returnType;
    public Body Body { get; } = body;

    public override void Accept(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}

public class Class(
    Position position,
    ClassName name,
    ClassName? extends,
    Members members
) : Node(position)
{
    public ClassName Name { get; set; } = name;
    public ClassName? Extends { get; set; } = extends;
    public Members Members { get; set; } = members;
}

public abstract class Statement(Position position) : Node(position)
{
    public abstract void Accept(IStatementVisitor visitor);
}

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

public class WhileLoop(
    Position position,
    Expression condition,
    Body body
) : Statement(position)
{
    public Expression Condition { get; set; } = condition;
    public Body Body { get; set; } = body;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}

public class IfStatement(
    Position position,
    Expression condition,
    Body trueBlock,
    Body? falseBlock
) : Expression(position)
{
    public Expression Condition { get; set; } = condition;
    public Body TrueBlock { get; set; } = trueBlock;
    public Body? FalseBlock { get; set; } = falseBlock;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}

public class ReturnStatement(
    Position position,
    Expression? expression
) : Statement(position)
{
    public Expression? Expression { get; set; } = expression;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}

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

public abstract class Expression(
    Position position
) : Statement(position);

public class ConstructorInvocation(
    Position position,
    ClassName className,
    Arguments arguments
) : Expression(position)
{
    public ClassName ClassName { get; set; } = className;
    public Arguments Arguments { get; set; } = arguments;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}

public class MethodCall(
    Position position,
    Expression expression,
    string methodName,
    Arguments arguments
) : Expression(position)
{
    public Expression Expression { get; set; } = expression;
    public string MethodName { get; set; } = methodName;
    public Arguments Arguments { get; set; } = arguments;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}

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

// TODO: мейби можно переделать на явные int, string, Identifier и т. д.
public class ValueNode<T>(
    Position position,
    T value
) : Expression(position)
{
    public T Value { get; } = value;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}


public class ClassName(
    Position position,
    string name,
    ClassName? genericArgument
) : Expression(position)
{
    public string Name { get; set; } = name;
    public ClassName? GenericArgument { get; set; } = genericArgument;
    
    public override void Accept(IStatementVisitor visitor) => visitor.Visit(this);
}
