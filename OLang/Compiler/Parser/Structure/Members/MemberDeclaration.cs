using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Members;

public abstract class MemberDeclaration(Position position) : Node(position)
{
    public abstract void Accept(IMemberDeclarationVisitor visitor);
}