﻿using OLang.Compiler.Lexer;

Console.WriteLine("Enter absolute path to the test .olang file");
var pathToSourceCode = Console.ReadLine();

if (File.Exists(pathToSourceCode) == false)
{
    Console.WriteLine("This file was not found");
    return;
}

using var sourceFile = File.OpenRead(pathToSourceCode);
var lex = new SuperLexer();
var tokensStream = lex.Feed(sourceFile);

foreach (var token in tokensStream)
    Console.WriteLine(token);