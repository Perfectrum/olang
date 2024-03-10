using OLang.Compiler.Parser;

Console.WriteLine("Enter absolute path to the test .olang file");
var pathToSourceCode = Console.ReadLine();

if (File.Exists(pathToSourceCode) == false)
{
    Console.WriteLine("This file was not found");
    return;
}

using var sourceFile = File.OpenRead(pathToSourceCode);
var lex = new Lexer();
var tokensStream = lex.Feed(sourceFile);

using var errorStream = new StreamWriter(Console.OpenStandardOutput());
using var outputStream = new StreamWriter(Console.OpenStandardOutput());

var parser = new Parser(new Scanner(tokensStream, errorStream));
if (parser.Parse() == false)
    errorStream.WriteLine("Error Parsing");
else
    outputStream.WriteLine(parser.EndNode);

