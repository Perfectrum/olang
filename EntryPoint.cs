using ObjectLanguage.Compiler.Lexer;

Console.WriteLine("Enter path to the test .olang file");
var pathToSourceCode = Console.ReadLine();

if (Path.Exists(pathToSourceCode) == false)
{
    Console.WriteLine("This file was not found");
    return;
}

using var sourceFile = File.OpenRead(pathToSourceCode);
var lexer = new Lexer(sourceFile);
var tokensStream = lexer.Analyze();
