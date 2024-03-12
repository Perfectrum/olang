using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

string yaccPath = Path.Combine("Compiler", "Parser", "GrammarYaac.y");
string docPath = Path.Combine("..", "docs", "grammar.md");

Dictionary<string, string> tokens = new()
{
    ["CLASS"] = "class",
    ["EXTENDS"] = "extends",
    ["LET"] = "let",
    ["THIS"] = "this",
    ["METHOD"] = "method",
    ["IS"] = "is",
    ["END"] = "end",
    ["RETURN"] = "return",
    ["WHILE"] = "while",
    ["LOOP"] = "loop",
    ["IF"] = "if",
    ["THEN"] = "then",
    ["ELSE"] = "else",
    ["COMMA"] = ",",
    ["COLON"] = ":",
    ["SEMICOLON"] = ";",
    ["LPARENT"] = "(",
    ["RPARENT"] = ")",
    ["LBRACKET"] = "[",
    ["RBRACKET"] = "]",
    ["DOT"] = ".",
    ["ASAN"] = "=",
    ["FIELD"] = "field",
    ["STATIC"] = "static",
    ["FUNCTION"] = "function",
    ["SUPER"] = "super"
};

if (File.Exists(yaccPath))
{
    if (!File.Exists(docPath))
    {
        File.Create(docPath).Close();
    }

    using StreamWriter writer = new(docPath);
    using StreamReader reader = new(yaccPath);

    bool inside = false;

    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine();
        
        if (line == "%%")
        {
            inside = !inside;
        }
        else if (inside)
        {
            line = Regex.Replace(line, @"\{.*?\}", "");

            foreach (var pair in tokens)
            {
                line = Regex.Replace(line, $@"\b{pair.Key}\b", $"\"{pair.Value}\"");
            }

            writer.WriteLine(line);
        }
    }
}
else
{
    Console.WriteLine($"Yacc file {yaccPath} with grammar description was not found");
}
