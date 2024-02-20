using System.Collections.ObjectModel;
using System.Text;
using ObjectLanguage.Compiler.Lexer.Tokens;

namespace ObjectLanguage.Compiler.Lexer;

public static class Lexer
{
    private static readonly ReadOnlyDictionary<string, KeywordType> _keywordTypes = new Dictionary<string, KeywordType>
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
    }.AsReadOnly();

    // TODO: сделать точки + двоеточия
    public static IEnumerable<BaseToken> Analyze(Stream sourceFile)
    {
        StringBuilder wordBuffer = new();
        int lineNumber = 0;
        int indexOnLine = 0;
        
        TextReader reader = new StreamReader(sourceFile);
        for (int readFromBuffer = reader.Read();
             readFromBuffer != -1;
             readFromBuffer = reader.Read(), ++indexOnLine)
        {
            char currentChar = (char)readFromBuffer;

            // TODO: не очень красиво, но умнее нужно думать
            var indexOfInNewLine = Environment.NewLine.IndexOf(currentChar);
            if (indexOfInNewLine != -1)
            {
                if (indexOfInNewLine == Environment.NewLine.Length - 1)
                    ++lineNumber;
                indexOnLine = 0;
                continue;
            }

            if (currentChar != ' ')
            {
                wordBuffer.Append(currentChar);
                continue;
            }
            
            if (wordBuffer.Length == 0)
                continue;

            var word = wordBuffer.ToString();
            var span = new Span(lineNumber, indexOnLine - word.Length, indexOnLine);
            
            if (_keywordTypes.TryGetValue(word, out KeywordType keywordType))
                yield return new Keyword(keywordType, span);
            else if (int.TryParse(word, out var intNumber))
                yield return new Integer(intNumber, span);
            // TODO: наверняка не учёл культуру
            else if (double.TryParse(word, out var doubleNumber))
                yield return new Real(doubleNumber, span);
            else
                yield return new Identifier(word, span);
            
            wordBuffer.Clear();
        }
    }
}