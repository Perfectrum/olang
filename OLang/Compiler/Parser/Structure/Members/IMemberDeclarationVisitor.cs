using OLang.Compiler.Parser.Structure.Members.Implementations;

namespace OLang.Compiler.Parser.Structure.Members;

public interface IMemberDeclarationVisitor
{
    void Visit(Field field);
    void Visit(Constructor constructor);
    void Visit(Method method);
}