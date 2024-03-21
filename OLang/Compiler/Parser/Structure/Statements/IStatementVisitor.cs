using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser.Structure.Statements;

public interface IStatementVisitor
{
    void Visit(IfStatement ifStatement);
    void Visit(Variable variable);
    void Visit(ThisNode thisNode);
    void Visit(Assigment assigment);
    void Visit(WhileLoop whileLoop);
    void Visit(ReturnStatement returnStatement);
    void Visit(ConstructorInvocation constructorInvocation);
    void Visit(MethodCall methodCall);
    void Visit(ValueGetting valueGetting);
    void Visit(ClassName className);
    
    void Visit(BooleanNode booleanNode);
    void Visit(StringLiteralNode stringLiteralNode);
    void Visit(RealNode realNode);
    void Visit(IntegerNode integerNode);
    void Visit(IdentifierNode identifierNode);
}