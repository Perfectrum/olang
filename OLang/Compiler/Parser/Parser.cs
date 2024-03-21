using OLang.Compiler.Lexer.Tokens;
using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure;
using OLang.Compiler.Parser.Structure.Members;
using OLang.Compiler.Parser.Structure.Members.Implementations;
using OLang.Compiler.Parser.Structure.Statements;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser;

internal partial class Parser
{
    public Structure.Program EndNode => (Structure.Program) CurrentSemanticValue;

    #region Utils

    #region Lists

    private static TemporaryListNode CreateNewList(Position position, Node firstElement) =>
        new(position, [firstElement]);

    private static TemporaryListNode CreateNewList(Position position) =>
        new(position, []);

    private static void AddToList(Position position, Node list, Node element)
    {
        var listNode = ((TemporaryListNode)list);
        listNode.Position = position;
        ((TemporaryListNode)list).Nodes.Add(element);
    }

    private static List<T> ListNodeToList<T>(Node node) =>
        ((TemporaryListNode)node).Nodes.Cast<T>().ToList();

    #endregion

    private static LocalConstructorIdentifierType GetLocalConstructorIdentifierType(Node node) =>
        ((KeywordWrapperNode)node).Value switch
        {
            KeywordType.Base => LocalConstructorIdentifierType.Base,
            KeywordType.This => LocalConstructorIdentifierType.This,
            _ => throw new ArgumentOutOfRangeException()
        };
    #endregion

    private static Structure.Program CreateProgram(Position position, Node classes) =>
        new(position, ListNodeToList<Class>(classes));

    private static Class CreateClass(Position position, Node className, Node? extends, Node members) =>
        new(position, (ClassName)className, (ClassName?)extends, ListNodeToList<MemberDeclaration>(members));

    private static ClassName CreateClassName(Position position, Node name, Node? genericArgument) =>
        new(position, ((IdentifierNode)name).Value, (ClassName?)genericArgument);
    
    private static Parameter CreateParameter(Position position, Node name, Node type) =>
        new(position, ((IdentifierNode)name).Value, (ClassName)type);

    private static Field CreateField(Position position, Node name, Node? type, Node expression) =>
        new(position, false, ((IdentifierNode)name).Value, (ClassName?)type, (Expression)expression);

    private static Field CreateStaticField(Position position, Node name, Node? type, Node expression) =>
        new(position, true, ((IdentifierNode)name).Value, (ClassName?)type, (Expression)expression);

    private static Constructor CreateConstructor(Position position, Node parameters, Node localConstructorInvocationNode, Node body) =>
        new(position, ListNodeToList<Parameter>(parameters), (LocalConstructorInvocationNode)localConstructorInvocationNode, ListNodeToList<Statement>(body));

    private static Method CreateMethod(Position position, Node name, Node parameters, Node? type, Node body) =>
        new(position, false, ((IdentifierNode)name).Value, ListNodeToList<Parameter>(parameters),
            (ClassName?)type, ListNodeToList<Statement>(body));
    
    private static Method CreateFunction(Position position, Node name, Node parameters, Node? type, Node body) =>
        new(position, true, ((IdentifierNode)name).Value, ListNodeToList<Parameter>(parameters),
            (ClassName?)type, ListNodeToList<Statement>(body));

    private static Variable CreateVariable(Position position, Node name, Node? type, Node expression) =>
        new(position, ((IdentifierNode)name).Value, (ClassName?)type, (Expression)expression);

    private static Assigment CreateAssigment(Position position, Node assignmentValue, Node expression) =>
        new(position, (Expression)assignmentValue, (Expression)expression);

    private static WhileLoop CreateWhileLoop(Position position, Node condition, Node body) =>
        new(position, (Expression)condition, ListNodeToList<Statement>(body));

    private static ReturnStatement CreateReturn(Position position, Node? expression) =>
        new(position, (Expression?)expression);
    
    private static IfStatement CreateIfStatement(Position position, Node expression, Node body, Node? elseBody) =>
        new(position, (Expression)expression, ListNodeToList<Statement>(body),
            elseBody == null ? null : ListNodeToList<Statement>(elseBody));

    private static ConstructorInvocation CreateConstructorInvocation(Position position, Node className, Node arguments) =>
        new(position, (ClassName)className, ListNodeToList<Expression>(arguments));
    
    private static LocalConstructorInvocationNode CreateLocalConstructorInvocation(Position position, Node localConstructorType, Node arguments) =>
        new(position, GetLocalConstructorIdentifierType(localConstructorType), ListNodeToList<Expression>(arguments));

    private static MethodCall CreateMethodCall(Position position, Node expression, Node methodName, Node arguments) =>
        new(position, (Expression)expression, ((IdentifierNode)methodName).Value,
            ListNodeToList<Expression>(arguments));
    
    private static ValueGetting CreateValueGetting(Position position, Node expression, Node fieldName) =>
        new(position, (Expression)expression, ((IdentifierNode)fieldName).Value);

    private static ThisNode CreateThisNode(Position position, Node keywordWrapperNode) =>
        ((KeywordWrapperNode)keywordWrapperNode).Value switch
        {
            KeywordType.This => new(position),
            _ => throw new ArgumentOutOfRangeException()
        };
}