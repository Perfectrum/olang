using System.Collections.ObjectModel;

namespace OLang.Compiler.Parser;

using Parameters = ReadOnlyCollection<Parameter>;
using Body = ReadOnlyCollection<Statement>;
using Members = ReadOnlyCollection<MemberDeclaration>;

internal record TemporaryListNode(List<Node> Nodes) : Node
{
    // TODO: мейби можно придумать без вызова .Cast<T>().ToArray(), чтобы память лишнюю не выделять, но пока так
    public ReadOnlyCollection<T> Collect<T>() => Nodes.Cast<T>().ToArray().AsReadOnly();
}

internal record ValueNode<T>(T Value) : Node;

public abstract record Node
{
    // public abstract void Visit(IVisitorNodes visitor);
}

public record Program(ReadOnlyCollection<Class> Classes) : Node;

public abstract record MemberDeclaration : Node;

public record Parameter(string Name, ClassName Type) : Node;

public record Field(
    // TODO: не уверен, что будет удобно
    bool IsStatic,
    string Name,
    ClassName? Type,
    Expression Expression
) : MemberDeclaration;

public record Constructor(
    Parameters Parameters,
    Body Body
) : MemberDeclaration;

public record Method(
    // TODO: тоже не уверен, что будет удобно
    bool IsStatic,
    string Name,
    Parameters Parameters,
    ClassName? ReturnType,
    Body Body
) : MemberDeclaration;

public record Class(
    ClassName Name,
    ClassName? Extends,
    Members Members
) : Node;

public record ClassName(string Name, ClassName? GenericArgument) : Node;

public record Statement : Node;

public record Expression : Statement;

public record If(
    Expression Condition,
    Body TrueBlock,
    Body? FalseBlock
) : Expression;
