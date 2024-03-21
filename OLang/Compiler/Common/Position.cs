using QUT.Gppg;

namespace OLang.Compiler.Overall;

public record struct Position(
    long BeginLine,
    long EndLine,
    int BeginColumn,
    int EndColumn,
    string Filename
) : IMerge<Position>
{
    public Position Merge(Position last) =>
        new Position(BeginLine, last.EndLine, BeginColumn, last.EndColumn, Filename);

    public override string ToString() =>
        $"{Filename}:{BeginLine}-{EndLine}:{BeginColumn}-{EndColumn}";
};