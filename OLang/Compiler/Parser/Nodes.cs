using System.Collections.ObjectModel;

namespace OLang.Compiler.Parser;

using Parameters = ReadOnlyCollection<Parameter>;
using Body = ReadOnlyCollection<Statement>;
using Members = ReadOnlyCollection<MemberDeclaration>;
using Arguments = ReadOnlyCollection<Expression>;

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

internal record TemporaryListNode(List<INode> Nodes) : INode
{
    // TODO: мейби можно придумать без вызова .Cast<T>().ToArray(), чтобы память лишнюю не выделять, но пока так
    public ReadOnlyCollection<T> Collect<T>() => Nodes.Cast<T>().ToArray().AsReadOnly();
}

public interface INode;

public record Program(ReadOnlyCollection<Class> Classes) : INode;

public abstract record MemberDeclaration : INode
{
    public abstract void Visit(IMemberDeclarationVisitor visitor);
}

public record Parameter(
    string Name,
    ClassName Type
) : INode;

public record Field(
    // TODO: не уверен, что будет удобно
    bool IsStatic,
    string Name,
    ClassName? Type,
    Expression Expression
) : MemberDeclaration
{
    public override void Visit(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}

public record Constructor(
    Parameters Parameters,
    Body Body
) : MemberDeclaration
{
    public override void Visit(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}

public record Method(
    // TODO: тоже не уверен, что будет удобно
    bool IsStatic,
    string Name,
    Parameters Parameters,
    ClassName? ReturnType,
    Body Body
) : MemberDeclaration
{
    public override void Visit(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}

public record Class(
    ClassName Name,
    ClassName? Extends,
    Members Members
) : INode;

public abstract record Statement : INode
{
    public abstract void Visit(IStatementVisitor visitor);
}

public record Assigment(
    string VariableName,
    Expression Expression
) : Statement
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record WhileLoop(
    Expression Condition,
    Body Body
) : Statement
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record IfStatement(
    Expression Condition,
    Body TrueBlock,
    Body? FalseBlock
) : Expression
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record ReturnStatement(
    Expression? Expression
) : Statement
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record Variable(
    string Name,
    ClassName? Type,
    Expression Expression
) : Statement
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public abstract record Expression : Statement;

public record ConstructorInvocation(
    ClassName ClassName,
    Arguments Arguments
) : Expression
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record MethodCall(
    Expression Expression,
    string MethodName,
    Arguments Arguments
) : Expression
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record ValueGetting(
    Expression Expression,
    string FieldName
) : Expression
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

// TODO: мейби можно переделать на явные int, string и т. д.
public record ValueNode<T>(T Value) : Expression
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}

public record ClassName(
    string Name,
    ClassName? GenericArgument
) : Expression
{
    public override void Visit(IStatementVisitor visitor) => visitor.Visit(this);
}
