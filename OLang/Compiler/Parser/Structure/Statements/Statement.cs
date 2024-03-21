using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements;

public abstract class Statement(Position position) : Node(position)
{
    public abstract void Accept(IStatementVisitor visitor);
}