using OLang.Compiler.Overall;

namespace OLang.Compiler.Parser.Structure.Statements;

public abstract class Expression(
    Position position
) : Statement(position);