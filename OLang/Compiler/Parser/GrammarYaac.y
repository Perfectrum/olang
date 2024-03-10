/* For some reason the constructor does not generate itself */
%{
    public Parser(AbstractScanner<Node, LexLocation> scanner) : base(scanner) { }
%}

%output = Parser.Generated.cs
%visibility internal
%namespace OLang.Compiler.Parser
%partial

%parsertype Parser
%scanbasetype BaseScanner
%tokentype TokenType

%using OLang.Compiler.Lexer.Tokens

%YYSTYPE Node

%start Program

%token CLASS
%token EXTENDS
%token VAR
%token THIS
%token METHOD
%token IS
%token END
%token RETURN
%token WHILE
%token LOOP
%token IF
%token THEN
%token ELSE

%token COMMA
%token COLON
%token SEMICOLON
%token LPARENT
%token RPARENT
%token LBRACKET
%token RBRACKET
%token DOT
%token ASAN

%token FIELD
%token STATIC
%token FUNCTION
%token LET
%token SUPER

%token INTEGER
%token REAL
%token BOOLEAN
%token IDENTIFIER
%token STRING

%%

Program
    : ClassDeclarations { $$ = CreateProgram($1); }
    ;

ClassDeclarations
    : { $$ = CreateNewList(); }
    | ClassDeclarations ClassDeclaration { AddToList($$, $2); $$ = $1; }
    ;

ClassDeclaration
    : CLASS ClassName Extends IS MemberDeclarations END SEMICOLON { $$ = CreateClass($2, $3, $5); }
    ;

Extends
    : { $$ = null; }
    | EXTENDS ClassName { $$ = $2; }
    ;

MemberDeclarations
    : { $$ = CreateNewList(); }
    | MemberDeclarations MemberDeclaration SEMICOLON { AddToList($$, $2); $$ = $1; }
    ;

ClassName
    : IDENTIFIER GenericParameter { $$ = CreateClassName($1, $2); }
    ;
    
GenericParameter
    : { $$ = null; }
    | LBRACKET ClassName RBRACKET { $$ = $2; }
    ;

MemberDeclaration
    : FieldDeclaration
    | StaticDeclaration
    | ConstructorDeclaration
    | MethodDeclaration
    | FunctionDeclaration
    ;

FieldDeclaration
    : FIELD IDENTIFIER COLON ClassName IS Expression { $$ = CreateField($2, $4, $6); }
    ;

StaticDeclaration
    : STATIC IDENTIFIER COLON IDENTIFIER IS Expression { $$ = CreateStaticField($2, $4, $6); }
    ;

ConstructorDeclaration
    : THIS LPARENT Parameters RPARENT IS Body END { $$ = CreateConstructor($3, $6); }
    ;

MethodDeclaration
    : METHOD IDENTIFIER LPARENT Parameters RPARENT SpecifyingType IS Body END { $$ = CreateMethod($2, $4, $6, $8); }
    ;

FunctionDeclaration
    : FUNCTION IDENTIFIER LPARENT Parameters RPARENT SpecifyingType IS Body END { $$ = CreateFunction($2, $4, $6, $8); }
    ;

Parameters
    : { $$ = CreateNewList(); }
    | Parameters COMMA ParameterDeclaration { AddToList($$, $2); $$ = $1; }
    ;

ParameterDeclaration
    : IDENTIFIER COLON ClassName { $$ = CreateParameter($1, $3); }
    ;

Body
    : Statements
    ;

VariableDeclaration
    : LET IDENTIFIER SpecifyingType ASAN Expression SEMICOLON
    ;

SpecifyingType
    : { $$ = null; }
    | COLON ClassName { $$ = $2; }
    ;

Statements
    : { $$ = CreateNewList(); }
    | Statements Statement SEMICOLON { AddToList($$, $2); $$ = $1; }
    ;

Statement
    : VariableDeclaration
    | Assignment
    | WhileLoop
    | IfStatement
    | ReturnStatement
    | Expression
    ;

Assignment
    : IDENTIFIER ASAN Expression
    ;

WhileLoop
    : WHILE Expression LOOP Body END
    ;

IfStatement
    : IF Expression THEN Body ElseTail END { $$ = CreateIfStatement($2, $4, $5); }
    ;

ElseTail
    : { $$ = null; }
    | ELSE Body { $$ = $2; }
    ;

// TODO: мейби вынести, как на паре показывали (но хочется посмотреть самому на конфликты)
ReturnStatement
    : RETURN
    | RETURN Expression
    ;

Expression
    : ConstructorInvocation
    | MethodCall
    | ValueGetting
    | Primary { $$ = Scanner.yylval; }
    ;

ConstructorInvocation
    : IDENTIFIER LPARENT Arguments RPARENT
    ;

MethodCall
    : Primary DOT IDENTIFIER LPARENT Arguments RPARENT
    ;

ValueGetting
    : Primary DOT IDENTIFIER 
    ;

Arguments
    : 
    | Arguments COMMA Expression
    ;

Primary
    : INTEGER
    | REAL
    | BOOLEAN
    | STRING
    | THIS
    | ClassName
    | SUPER
    ;

%%
