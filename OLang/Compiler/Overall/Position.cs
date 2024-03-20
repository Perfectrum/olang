using QUT.Gppg;

namespace OLang.Compiler.Overall;

public record struct Position(
    long BeginLine,
    long EndLine,
    int BeginColumn,
    int EndColumn
) : IMerge<Position>
{
    public Position Merge(Position last) =>
        new Position(BeginLine, last.EndLine, BeginColumn, last.EndColumn);

    public override string ToString() =>
        $"{BeginLine}-{EndLine}:{BeginLine}-{EndColumn}";
};