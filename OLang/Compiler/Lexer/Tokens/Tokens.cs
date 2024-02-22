namespace OLang.Compiler.Lexer.Tokens;

public abstract record Token(Span Span);

public record Identifier(string Name, Span Span) : Token(Span);

public record StringLiteral(string Text, Span Span) : Token(Span);

public record BooleanLiteral(bool Value, Span Span) : Token(Span);

public record Integer(int Value, Span Span) : Token(Span);

public record Real(double Value, Span Span) : Token(Span);

public record Keyword(KeywordType Type, Span Span) : Token(Span);

public enum KeywordType
{
    Class,
    Extends,
    
    Var,
    This,
    
    Method,
    Is,
    End,
    Return,
    
    While,
    Loop,
    
    If,
    Then,
    Else,
}

public record Symbol(string Text, SymbolType Type, Span Span) : Token(Span);

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

