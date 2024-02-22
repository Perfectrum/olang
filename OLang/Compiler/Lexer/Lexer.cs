using System.Collections.ObjectModel;
using System.Text;
using OLang.Compiler.Lexer.Tokens;

internal class Lexer
{
    ///////////////////////////////////////////////////////////
    // Keywords

    private static readonly ReadOnlyDictionary<string, KeywordType> _keywordTypes = new(new Dictionary<string, KeywordType>
    {
        ["class"] = KeywordType.Class,
        ["extends"] = KeywordType.Extends,
        ["var"] = KeywordType.Var,
        ["this"] = KeywordType.This,
        ["method"] = KeywordType.Method,
        ["is"] = KeywordType.Is,
        ["end"] = KeywordType.End,
        ["return"] = KeywordType.Return,
        ["while"] = KeywordType.While,
        ["loop"] = KeywordType.Loop,
        ["true"] = KeywordType.True,
        ["false"] = KeywordType.False,
        ["if"] = KeywordType.If,
        ["then"] = KeywordType.Then,
        ["else"] = KeywordType.Else,
    });

    ///////////////////////////////////////////////////////////
    // Types

    private enum Mode
    {
        BASIC,
        INTEGER,
        REAL,
        DOT,
        STRING,
        WORD
    }

    private record Location(int Column, int Line, int Offset);
    private record struct SwitchParameters(Mode Mode, bool Delay = true, bool Push = true);

    ///////////////////////////////////////////////////////////
    // Fields

    private Mode _mode = Mode.BASIC;
    private char _currentChar = (char)0;
    private bool _delayCurrentSymbol = false;
    private readonly Queue<Token> _shouldBeYielded = new();
    private readonly StringBuilder _buffer = new();

    private readonly Stack<Location> _locations = new();
    private int _column = 1;
    private int _line = 1;
    private int _offset = 0;

    ///////////////////////////////////////////////////////////
    // Helpers

    private char Current => _currentChar;

    private Span CurrentSpan => new(_line, _column, _column + 1);

    private void Switch(SwitchParameters parameters)
    {
        _mode = parameters.Mode;
        _delayCurrentSymbol = parameters.Delay;
        if (parameters.Push)
            _locations.Push(new Location(_column, _line, _offset));
    }

    private void Yield(Token token)
    {
        _shouldBeYielded.Enqueue(token);
    }

    private void NewLine()
    {
        _column = 0;
        ++_line;
    }

    private Location GetLastLocation()
    {
        return _locations.Pop();
    }

    private void Store()
    {
        _buffer.Append(Current);
    }

    private string GetSavedWord()
    {
        var word = _buffer.ToString();
        _buffer.Clear();
        return word;
    }

    private Span GetSpan(int endFix = 0, int startFix = 0)
    {

        var loc = GetLastLocation();
        return new Span(_line, (loc?.Column ?? 0) + startFix, _column + endFix);
    }

    ///////////////////////////////////////////////////////////
    // Char helpers

    private bool IsCurrentNewLine() => Current == '\n';

    private bool IsCurrentDigit() => char.IsDigit(Current);

    private bool IsCurrentLetter() => char.IsLetter(Current);

    private bool IsCurrentAlphaNum() => IsCurrentDigit() || IsCurrentLetter();

    private bool IsCurrentSaveIdentFirstLetter() => IsCurrentLetter() || Current == '_';

    private bool IsCurrentSaveIdentLetter() => IsCurrentAlphaNum() || Current == '_';

    ///////////////////////////////////////////////////////////
    // EntryPoint

    public IEnumerable<Token> Feed(Stream sourceFile)
    {
        TextReader reader = new StreamReader(sourceFile);

        int ch = reader.Read();
        _delayCurrentSymbol = true;
        _currentChar = (char)ch;
        while (ch != -1)
        {
            while (_shouldBeYielded.Count > 0)
            {
                yield return _shouldBeYielded.Dequeue();
            }

            if (!_delayCurrentSymbol)
            {
                ch = reader.Read();
                _currentChar = (char)ch;
                ++_column;
                ++_offset;
            }
            _delayCurrentSymbol = false;

            var switchParameters = _mode switch
            {
                Mode.BASIC => BasicCase(),
                Mode.INTEGER => IntegerCase(),
                Mode.REAL => DotCase(),
                Mode.DOT => RealCase(),
                Mode.STRING => StringCase(),
                Mode.WORD => WordCase(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (switchParameters.HasValue)
                Switch(switchParameters.Value);
        }

        End();
        while (_shouldBeYielded.Count > 0)
            yield return _shouldBeYielded.Dequeue();
    }

    ///////////////////////////////////////////////////////////
    // Actions

    private SwitchParameters? BasicCase()
    {
        if (IsCurrentNewLine())
        {
            NewLine();
            return null;
        }

        SwitchParameters? mode = null;
        if (IsCurrentDigit())
            mode = new SwitchParameters(Mode.INTEGER);
        else if (IsCurrentSaveIdentFirstLetter())
            mode = new SwitchParameters(Mode.WORD);
        else if (Current == '"')
            mode = new SwitchParameters(Mode.STRING, false);

        if (mode.HasValue)
            return mode;

        SymbolType? symbolType = Current switch
        {
            ',' => SymbolType.Comma,
            '.' => SymbolType.Dot,
            ';' => SymbolType.Semicolon,
            ':' => SymbolType.Colon,
            '(' => SymbolType.LP,
            ')' => SymbolType.RP,
            '[' => SymbolType.LB,
            ']' => SymbolType.RB,
            '=' => SymbolType.Asan,
            _ => null
        };

        if (symbolType.HasValue)
            Yield(new Symbol(Current.ToString(), symbolType.Value, CurrentSpan));

        return null;
    }

    private SwitchParameters? IntegerCase()
    {
        if (IsCurrentDigit())
        {
            Store();
            return null;
        }

        if (Current == '.')
        {
            Store();
            return new SwitchParameters(Mode.DOT, false, false);
        }

        var word = GetSavedWord();
        // Потому что в буффере все цифры, поэтому без проверки
        var number = int.Parse(word);

        Yield(new Integer(number, GetSpan()));
        return new SwitchParameters(Mode.BASIC);
    }

    private SwitchParameters? DotCase()
    {
        if (IsCurrentDigit())
            return new SwitchParameters(Mode.REAL, true, false);

        var word = GetSavedWord();
        var number = int.Parse(word[..^1]);

        Yield(new Integer(number, GetSpan(-1)));

        var dotSpan = CurrentSpan;
        dotSpan.BeginPosition -= 1;
        dotSpan.EndPosition -= 1;

        Yield(new Symbol(".", SymbolType.Dot, dotSpan));
        return new SwitchParameters(Mode.BASIC);
    }

    private SwitchParameters? RealCase()
    {
        if (IsCurrentDigit())
        {
            Store();
            return null;
        }

        var word = GetSavedWord();
        var number = double.Parse(word);

        Yield(new Real(number, GetSpan()));
        return new SwitchParameters(Mode.BASIC);
    }

    private SwitchParameters? StringCase()
    {
        if (IsCurrentNewLine())
        {
            //TODO: можно ошибку выкидывать в теории
            NewLine();
            Store();
            return null;
        }

        if (Current == '"')
        {
            Yield(new StringLiteral(GetSavedWord(), GetSpan()));
            return new SwitchParameters(Mode.BASIC, false);
        }

        Store();
        return null;
    }

    private SwitchParameters? YieldWord()
    {
        var word = GetSavedWord();
        if (word.Length > 0)
        {
            if (_keywordTypes.TryGetValue(word, out var type))
                Yield(new Keyword(type, GetSpan()));
            else
                Yield(new Identifier(word, GetSpan()));
        }

        return new SwitchParameters(Mode.BASIC);
    }

    private SwitchParameters? WordCase()
    {
        if (IsCurrentSaveIdentLetter())
        {
            Store();
            return null;
        }

        return YieldWord();
    }

    private void End()
    {
        var word = GetSavedWord();
        if (word.Length == 0)
            return;

        switch (_mode)
        {
            case Mode.INTEGER:
            Console.WriteLine("######################################");
                Console.WriteLine(word);
                Yield(new Integer(int.Parse(word), GetSpan()));
                return;
            case Mode.REAL:
                Yield(new Real(double.Parse(word), GetSpan()));
                break;
            case Mode.WORD:
                YieldWord();
                break;
        }
    }
}