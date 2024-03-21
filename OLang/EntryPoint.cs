using Newtonsoft.Json;
using OLang.Compiler.Lexer;
using OLang.Compiler.Parser;
using DotNetGraph.Compilation;

if (args.Length == 0) 
{
    Console.WriteLine("Enter absolute path to the test .olang file");
    return;
}

var pathToSourceCode = args[0];

if (File.Exists(pathToSourceCode) == false)
{
    Console.WriteLine("This file was not found");
    return;
}

using var sourceFile = File.OpenRead(pathToSourceCode);
var lex = new SuperLexer();
var tokensStream = lex.Feed(sourceFile);

using var errorStream = new StreamWriter(Console.OpenStandardError());
using var outputStream = new StreamWriter(Console.OpenStandardOutput());

var parser = new Parser(new Scanner(tokensStream, errorStream));
if (parser.Parse() == false)
{
    errorStream.WriteLine("Error Parsing");
}
else
{
    outputStream.WriteLine("C style:");
    Printer.Print(parser.EndNode, outputStream);
    
    outputStream.WriteLine();
    outputStream.WriteLine("Json:");
    var json = JsonConvert.SerializeObject(parser.EndNode, Formatting.Indented);
    outputStream.WriteLine(json);


    var graphBuilder = new DotGraphBuilder();

    graphBuilder.AddProgram(parser.EndNode);

    await using var writer = new StringWriter();
    var context = new CompilationContext(writer, new CompilationOptions());
    await graphBuilder.graph.CompileAsync(context);

    var result = writer.GetStringBuilder().ToString();
    File.WriteAllText("graph.dot", result);
}

