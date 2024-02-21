using System.Collections.ObjectModel;
using System.Text;
using ObjectLanguage.Compiler.Lexer.Tokens;

class Lexer2
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

    private void Switch(Mode mode, bool delay = true, bool push = true)
    {
        _mode = mode;
        _delayCurrentSymbol = delay;
        if (push)
        {
            _locations.Push(new Location(_column, _line, _offset));
        }
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

    private bool IsCurrentNewLine()
    {
        return Current == '\n';
    }

    private bool IsCurrentDigit()
    {
        return char.IsDigit(Current);
    }

    private bool IsCurrentLetter()
    {
        return char.IsLetter(Current);
    }

    private bool IsCurrentAlphaNum()
    {
        return IsCurrentDigit() || IsCurrentLetter();
    }

    private bool IsCurrentSaveIdentFirstLetter()
    {
        return IsCurrentLetter() || Current == '_';
    }

    private bool IsCurrentSaveIdentLetter()
    {
        return IsCurrentAlphaNum() || Current == '_';
    }

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

            switch (_mode)
            {
                case Mode.BASIC:
                    BasicCase();
                    break;
                case Mode.INTEGER:
                    IntegerCase();
                    break;
                case Mode.DOT:
                    DotCase();
                    break;
                case Mode.REAL:
                    RealCase();
                    break;
                case Mode.STRING:
                    StringCase();
                    break;
                case Mode.WORD:
                    WordCase();
                    break;
            }
        }

        End();
        while (_shouldBeYielded.Count > 0)
        {
            yield return _shouldBeYielded.Dequeue();
        }
    }

    ///////////////////////////////////////////////////////////
    // Actions

    private void BasicCase()
    {
        if (IsCurrentNewLine())
        {
            NewLine();
            return;
        }

        if (IsCurrentDigit())
        {
            Switch(Mode.INTEGER);
            return;
        }
        if (IsCurrentSaveIdentFirstLetter())
        {
            Switch(Mode.WORD);
            return;
        }

        if (Current == '"')
        {
            Switch(Mode.STRING, false);
            return;
        }

        if (Current == ',')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.Comma, CurrentSpan));
            return;
        }

        if (Current == '.')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.Dot, CurrentSpan));
            return;
        }

        if (Current == ';')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.Semicolon, CurrentSpan));
            return;
        }

        if (Current == ':')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.Colon, CurrentSpan));
            return;
        }

        if (Current == '(')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.LP, CurrentSpan));
            return;
        }

        if (Current == ')')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.RP, CurrentSpan));
            return;
        }

        if (Current == '[')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.LB, CurrentSpan));
            return;
        }

        if (Current == ']')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.RB, CurrentSpan));
            return;
        }

        if (Current == '=')
        {
            Yield(new Symbol(Current.ToString(), SymbolType.Asan, CurrentSpan));
            return;
        }
    }

    private void IntegerCase()
    {
        if (IsCurrentDigit())
        {
            Store();
            return;
        }

        if (Current == '.')
        {
            Store();
            Switch(Mode.DOT, false, false);
            return;
        }

        var word = GetSavedWord();
        // Потому что в буффере все цифры, поэтому без проверки
        var number = int.Parse(word);

        Yield(new Integer(number, GetSpan()));
        Switch(Mode.BASIC);
    }

    private void DotCase()
    {
        if (IsCurrentDigit())
        {
            Switch(Mode.REAL, true, false);
            return;
        }

        var word = GetSavedWord();
        var number = int.Parse(word[..^1]);

        Yield(new Integer(number, GetSpan(-1)));

        var dotSpan = CurrentSpan;
        dotSpan.BeginPosition -= 1;
        dotSpan.EndPosition -= 1;

        Yield(new Symbol(".", SymbolType.Dot, dotSpan));
        Switch(Mode.BASIC);
    }

    private void RealCase()
    {
        if (IsCurrentDigit())
        {
            Store();
            return;
        }

        var word = GetSavedWord();
        var number = double.Parse(word);

        Yield(new Real(number, GetSpan()));
        Switch(Mode.BASIC);
    }

    private void StringCase()
    {
        if (Current == '\n')
        {
            //TODO: можно ошибку выкидывать в теории
            NewLine();
            Store();
            return;
        }
        if (Current == '"')
        {
            Yield(new StringLiteral(GetSavedWord(), GetSpan()));
            Switch(Mode.BASIC, false);
            return;
        }

        Store();
    }

    private void YieldWord()
    {
        var word = GetSavedWord();
        if (word.Length > 0)
        {
            if (_keywordTypes.TryGetValue(word, out var type))
            {
                Yield(new Keyword(type, GetSpan()));
            }
            else
            {
                Yield(new Identifier(word, GetSpan()));
            }
        }
        Switch(Mode.BASIC);
    }

    private void WordCase()
    {
        if (IsCurrentSaveIdentLetter())
        {
            Store();
            return;
        }

        YieldWord();
    }

    private void End()
    {
        var word = GetSavedWord();
        if (word.Length > 0)
        {
            if (_mode == Mode.INTEGER)
            {
                Yield(new Integer(int.Parse(word), GetSpan()));
                return;
            }
            if (_mode == Mode.REAL)
            {
                Yield(new Real(double.Parse(word), GetSpan()));
            }

            if (_mode == Mode.WORD)
            {
                YieldWord();
            }
        }
    }

}
