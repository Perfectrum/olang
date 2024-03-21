using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser.Structure.Statements;

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