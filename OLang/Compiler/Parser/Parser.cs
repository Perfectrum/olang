using System.Collections.ObjectModel;

namespace OLang.Compiler.Parser;

internal partial class Parser
{
    public INode EndNode => CurrentSemanticValue;

    #region Lists

    private TemporaryListNode CreateNewList(INode firstElement) =>
        new([firstElement]);

    private TemporaryListNode CreateNewList() =>
        new([]);

    private void AddToList(INode list, INode element) =>
        ((TemporaryListNode)list).Nodes.Add(element);

    private static ReadOnlyCollection<T> ListNodeToReadOnlyArray<T>(INode node) =>
        ((TemporaryListNode)node).Collect<T>();

    private static T GetValueFromNode<T>(INode node) =>
        ((ValueNode<T>)node).Value;

    #endregion

    private Program CreateProgram(INode classes) =>
        new(ListNodeToReadOnlyArray<Class>(classes));

    private Class CreateClass(INode className, INode? extends, INode members) =>
        new((ClassName)className, (ClassName?)extends, ListNodeToReadOnlyArray<MemberDeclaration>(members));

    private ClassName CreateClassName(INode name, INode? genericArgument) =>
        new(((ValueNode<string>)name).Value, (ClassName?)genericArgument);
    
    private Parameter CreateParameter(INode name, INode type) =>
        new(((ValueNode<string>)name).Value, (ClassName)type);

    private Field CreateField(INode name, INode? type, INode expression) =>
        new(false, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private Field CreateStaticField(INode name, INode? type, INode expression) =>
        new(true, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private Constructor CreateConstructor(INode parameters, INode body) =>
        new(ListNodeToReadOnlyArray<Parameter>(parameters), ListNodeToReadOnlyArray<Statement>(body));

    private Method CreateMethod(INode name, INode parameters, INode? type, INode body) =>
        new(false, GetValueFromNode<string>(name), ListNodeToReadOnlyArray<Parameter>(parameters),
            (ClassName?)type, ListNodeToReadOnlyArray<Statement>(body));
    
    private Method CreateFunction(INode name, INode parameters, INode? type, INode body) =>
        new(true, GetValueFromNode<string>(name), ListNodeToReadOnlyArray<Parameter>(parameters),
            (ClassName?)type, ListNodeToReadOnlyArray<Statement>(body));

    private Variable CreateVariable(INode name, INode? type, INode expression) =>
        new(GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private Assigment CreateAssigment(INode variableName, INode expression) =>
        new(GetValueFromNode<string>(variableName), (Expression)expression);

    private WhileLoop CreateWhileLoop(INode condition, INode body) =>
        new((Expression)condition, ListNodeToReadOnlyArray<Statement>(body));

    private ReturnStatement CreateReturn(INode? expression) =>
        new((Expression?)expression);
    
    private IfStatement CreateIfStatement(INode expression, INode body, INode? elseBody) =>
        new((Expression)expression, ListNodeToReadOnlyArray<Statement>(body),
            ((TemporaryListNode?)elseBody)?.Collect<Statement>());

    private ConstructorInvocation CreateConstructorInvocation(INode className, INode arguments) =>
        new((ClassName)className, ListNodeToReadOnlyArray<Expression>(arguments));

    private MethodCall CreateMethodCall(INode expression, INode methodName, INode arguments) =>
        new((Expression)expression, GetValueFromNode<string>(methodName),
            ListNodeToReadOnlyArray<Expression>(arguments));
    
    private ValueGetting CreateValueGetting(INode expression, INode fieldName) =>
        new((Expression)expression, GetValueFromNode<string>(fieldName));
}