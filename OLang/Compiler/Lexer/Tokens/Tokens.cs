using OLang.Compiler.Overall;

namespace OLang.Compiler.Lexer.Tokens;

public abstract record Token(Position Position);

public record NewLine(Position Position) : Token(Position);

public record Identifier(string Name, Position Position) : Token(Position);

public record StringLiteral(string Text, Position Position) : Token(Position);

public record BooleanLiteral(bool Value, Position Position) : Token(Position);

public record Integer(int Value, Position Position) : Token(Position);

public record Real(double Value, Position Position) : Token(Position);

public record Keyword(KeywordType Type, Position Position) : Token(Position);

public enum KeywordType
{
    Class,
    Extends,

    Var,
    This,
    Super,

    Method,
    Is,
    End,
    Return,

    While,
    Loop,

    If,
    Then,
    Else,

    Static,
    Field,
    Function
}

public record Symbol(string Text, SymbolType Type, Position Position) : Token(Position);

public enum SymbolType
{
    Comma,
    Colon,
    Semicolon,
    LP,
    RP,
    LB,
    RB,

    Dot,
    Asan
}

