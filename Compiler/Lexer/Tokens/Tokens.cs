namespace ObjectLanguage.Compiler.Lexer.Tokens;

public abstract record BaseToken(Span Span);

public record Identifier(string Name, Span Span) : BaseToken(Span);

public record Integer(int Value, Span Span) : BaseToken(Span);

public record Real(double Value, Span Span) : BaseToken(Span);

public record Keyword(KeywordType Type, Span Span) : BaseToken(Span);

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
    
    True,
    False,
    
    If,
    Then,
    Else,
}
