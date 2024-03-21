using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Statements;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser.Structure.Members.Implementations;

public class Method(
    Position position,
    // TODO: тоже не уверен, что будет удобно
    bool isStatic,
    string name,
    List<Parameter> parameters,
    ClassName? returnType,
    List<Statement> body
) : MemberDeclaration(position)
{
    public bool IsStatic { get; set; } = isStatic;
    public string Name { get; set; } = name;
    public List<Parameter> Parameters { get; } = parameters;
    public ClassName? ReturnType { get; set; } = returnType;
    public List<Statement> Body { get; } = body;

    public override void Accept(IMemberDeclarationVisitor visitor) => visitor.Visit(this);
}