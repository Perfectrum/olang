using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser;

internal partial class Parser
{
    public Program EndNode => (Program) CurrentSemanticValue;

    #region Lists

    private TemporaryListNode CreateNewList(Position position, Node firstElement) =>
        new(position, [firstElement]);

    private TemporaryListNode CreateNewList(Position position) =>
        new(position, []);

    private void AddToList(Position position, Node list, Node element)
    {
        var listNode = ((TemporaryListNode)list);
        listNode.Position = position;
        ((TemporaryListNode)list).Nodes.Add(element);
    }

    private static List<T> ListNodeToList<T>(Node node) =>
        ((TemporaryListNode)node).Nodes.Cast<T>().ToList();

    private static T GetValueFromNode<T>(Node node) =>
        ((ValueNode<T>)node).Value;

    #endregion

    private static Program CreateProgram(Position position, Node classes) =>
        new(position, ListNodeToList<Class>(classes));

    private static Class CreateClass(Position position, Node className, Node? extends, Node members) =>
        new(position, (ClassName)className, (ClassName?)extends, ListNodeToList<MemberDeclaration>(members));

    private static ClassName CreateClassName(Position position, Node name, Node? genericArgument) =>
        new(position, ((ValueNode<string>)name).Value, (ClassName?)genericArgument);
    
    private static Parameter CreateParameter(Position position, Node name, Node type) =>
        new(position, ((ValueNode<string>)name).Value, (ClassName)type);

    private static Field CreateField(Position position, Node name, Node? type, Node expression) =>
        new(position, false, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private static Field CreateStaticField(Position position, Node name, Node? type, Node expression) =>
        new(position, true, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private static Constructor CreateConstructor(Position position, Node parameters, Node body) =>
        new(position, ListNodeToList<Parameter>(parameters), ListNodeToList<Statement>(body));

    private static Method CreateMethod(Position position, Node name, Node parameters, Node? type, Node body) =>
        new(position, false, GetValueFromNode<string>(name), ListNodeToList<Parameter>(parameters),
            (ClassName?)type, ListNodeToList<Statement>(body));
    
    private static Method CreateFunction(Position position, Node name, Node parameters, Node? type, Node body) =>
        new(position, true, GetValueFromNode<string>(name), ListNodeToList<Parameter>(parameters),
            (ClassName?)type, ListNodeToList<Statement>(body));

    private static Variable CreateVariable(Position position, Node name, Node? type, Node expression) =>
        new(position, GetValueFromNode<string>(name), (ClassName?)type, (Expression)expression);

    private static Assigment CreateAssigment(Position position, Node variableName, Node expression) =>
        new(position, GetValueFromNode<string>(variableName), (Expression)expression);

    private static WhileLoop CreateWhileLoop(Position position, Node condition, Node body) =>
        new(position, (Expression)condition, ListNodeToList<Statement>(body));

    private static ReturnStatement CreateReturn(Position position, Node? expression) =>
        new(position, (Expression?)expression);
    
    private static IfStatement CreateIfStatement(Position position, Node expression, Node body, Node? elseBody) =>
        new(position, (Expression)expression, ListNodeToList<Statement>(body),
            elseBody == null ? null : ListNodeToList<Statement>(elseBody));

    private static ConstructorInvocation CreateConstructorInvocation(Position position, Node className, Node arguments) =>
        new(position, (ClassName)className, ListNodeToList<Expression>(arguments));

    private static MethodCall CreateMethodCall(Position position, Node expression, Node methodName, Node arguments) =>
        new(position, (Expression)expression, GetValueFromNode<string>(methodName),
            ListNodeToList<Expression>(arguments));
    
    private static ValueGetting CreateValueGetting(Position position, Node expression, Node fieldName) =>
        new(position, (Expression)expression, GetValueFromNode<string>(fieldName));
}