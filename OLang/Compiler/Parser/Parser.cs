using System.Collections.ObjectModel;

namespace OLang.Compiler.Parser;

internal partial class Parser
{
    public Node EndNode => CurrentSemanticValue;

    #region Lists

    private TemporaryListNode CreateNewList(Node firstElement) =>
        new([firstElement]);

    private TemporaryListNode CreateNewList() =>
        new([]);

    private void AddToList(Node list, Node element) =>
        ((TemporaryListNode)list).Nodes.Add(element);

    private static ReadOnlyCollection<T> ListNodeToReadOnlyArray<T>(Node node) =>
        ((TemporaryListNode)node).Collect<T>();

    private static T GetValueFromNode<T>(Node node) =>
        ((ValueNode<T>)node).Value;

    #endregion

    private Program CreateProgram(Node classes) =>
        new(ListNodeToReadOnlyArray<Class>(classes));

    private Class CreateClass(Node className, Node? extends, Node members) =>
        new((ClassName)className, (ClassName?)extends, ListNodeToReadOnlyArray<MemberDeclaration>(members));

    private ClassName CreateClassName(Node name, Node? genericArgument) =>
        new(((ValueNode<string>)name).Value, (ClassName?)genericArgument);
    
    private Parameter CreateParameter(Node name, Node type) =>
        new(((ValueNode<string>)name).Value, (ClassName)type);

    private Field CreateField(Node name, Node? type, Node expression) =>
        new(false, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private Field CreateStaticField(Node name, Node? type, Node expression) =>
        new(true, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private Constructor CreateConstructor(Node parameters, Node body) =>
        new(ListNodeToReadOnlyArray<Parameter>(parameters), ListNodeToReadOnlyArray<Statement>(body));

    private Method CreateMethod(Node name, Node parameters, Node? type, Node body) =>
        new(false, GetValueFromNode<string>(name), ListNodeToReadOnlyArray<Parameter>(parameters),
            (ClassName?)type, ListNodeToReadOnlyArray<Statement>(body));
    
    private Method CreateFunction(Node name, Node parameters, Node? type, Node body) =>
        new(true, GetValueFromNode<string>(name), ListNodeToReadOnlyArray<Parameter>(parameters),
            (ClassName?)type, ListNodeToReadOnlyArray<Statement>(body));

    private If CreateIfStatement(Node expression, Node body, Node? elseBody) =>
        new((Expression)expression, ListNodeToReadOnlyArray<Statement>(body),
            ((TemporaryListNode?)elseBody)?.Collect<Statement>());
}