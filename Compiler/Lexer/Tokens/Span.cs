namespace ObjectLanguage.Compiler.Lexer.Tokens;

public record struct Span(
    long Line,
    int BeginPosition,
    int EndPosition
);
