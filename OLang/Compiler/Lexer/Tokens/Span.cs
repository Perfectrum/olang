namespace OLang.Compiler.Lexer.Tokens;

public record struct Span(
    long LineNumber,
    int BeginPosition,
    int EndPosition
);