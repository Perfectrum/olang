using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Statements;

namespace OLang.Compiler.Parser.Structure.Members.Implementations;

public class Constructor(
    Position position,
    List<Parameter> parameters,
    List<Statement> body
) : MemberDeclaration(position)
{
    public List<Parameter> Parameters { get; } = parameters;
    public List<Statement> Body { get; } = body;
    
    public override void Accept(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}