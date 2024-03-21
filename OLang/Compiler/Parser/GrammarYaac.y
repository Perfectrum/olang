/* For some reason the constructor does not generate itself */
%{
    public Parser(AbstractScanner<Node, Position> scanner) : base(scanner) { }
%}

%output = Parser.Generated.cs
%visibility internal
%namespace OLang.Compiler.Parser
%partial

%parsertype Parser
%scanbasetype BaseScanner
%tokentype TokenType

%using OLang.Compiler.Overall
%using OLang.Compiler.Parser.Structure

%YYSTYPE Node
%YYLTYPE Position

%start Program

%token CLASS
%token EXTENDS
%token VAR
%token THIS
%token BASE
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
%token USING

%token FIELD
%token STATIC
%token FUNCTION

%token INTEGER
%token REAL
%token BOOLEAN
%token IDENTIFIER
%token STRING

%%

Program
    : ClassDeclarations { $$ = CreateProgram(@$, $1); }
    ;

ClassDeclarations
    : { $$ = CreateNewList(@$); }
    | ClassDeclarations ClassDeclaration { AddToList(@$, $$, $2); $$ = $1; }
    ;

ClassDeclaration
    : CLASS ClassName Extends IS MemberDeclarations END SEMICOLON { $$ = CreateClass(@$, $2, $3, $5); }
    ;

Extends
    : { $$ = null; }
    | EXTENDS ClassName { $$ = $2; }
    ;

MemberDeclarations
    : { $$ = CreateNewList(@$); }
    | MemberDeclarations MemberDeclaration SEMICOLON { AddToList(@$, $$, $2); $$ = $1; }
    ;

ClassName
    : IDENTIFIER PossibleGenericParameter { $$ = CreateClassName(@$, $1, $2); }
    ;
    
PossibleGenericParameter
    : { $$ = null; }
    | GenericParameter { $$ = $1; }
    ;
    
GenericParameter
    : LBRACKET ClassName RBRACKET { $$ = $2; }
    ;

MemberDeclaration
    : FieldDeclaration
    | StaticDeclaration
    | ConstructorDeclaration
    | MethodDeclaration
    | FunctionDeclaration
    ;

FieldDeclaration
    : FIELD IDENTIFIER COLON ClassName IS Expression { $$ = CreateField(@$, $2, $4, $6); }
    ;

StaticDeclaration
    : STATIC IDENTIFIER COLON IDENTIFIER IS Expression { $$ = CreateStaticField(@$, $2, $4, $6); }
    ;

ConstructorDeclaration
    : THIS LPARENT Parameters RPARENT LocalConstructorInvocation IS Body END { $$ = CreateConstructor(@$, $3, $5, $7); }
    ;

LocalConstructorInvocation
    : { $$ = null; }
    | USING LocalConstructorIdentifier LPARENT Arguments RPARENT { $$ = CreateLocalConstructorInvocation(@$, $2, $4); }
    ;

LocalConstructorIdentifier
    : THIS
    | BASE
    ;

MethodDeclaration
    : METHOD IDENTIFIER LPARENT Parameters RPARENT SpecifyingType IS Body END { $$ = CreateMethod(@$, $2, $4, $6, $8); }
    ;

FunctionDeclaration
    : FUNCTION IDENTIFIER LPARENT Parameters RPARENT SpecifyingType IS Body END { $$ = CreateFunction(@$, $2, $4, $6, $8); }
    ;

Parameters
    : { $$ = CreateNewList(@$); }
    | ParameterDeclaration { $$ = CreateNewList(@$, $1); }
    | Parameters COMMA ParameterDeclaration { AddToList(@$, $$, $3); $$ = $1; }
    ;

ParameterDeclaration
    : IDENTIFIER COLON ClassName { $$ = CreateParameter(@$, $1, $3); }
    ;

Body
    : Statements
    ;

VariableDeclaration
    : VAR IDENTIFIER SpecifyingType ASAN Expression { $$ = CreateVariable(@$, $2, $3, $5); }
    ;

SpecifyingType
    : { $$ = null; }
    | COLON ClassName { $$ = $2; }
    ;

Statements
    : { $$ = CreateNewList(@$); }
    | Statements Statement SEMICOLON { AddToList(@$, $$, $2); $$ = $1; }
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
    : AssigmentPrefix ASAN Expression { $$ = CreateAssigment(@$, $1, $3); }
    ;

AssigmentPrefix
    : ValueGetting
    | IDENTIFIER
    ;

WhileLoop
    : WHILE Expression LOOP Body END { $$ = CreateWhileLoop(@$, $2, $4); }
    ;

IfStatement
    : IF Expression THEN Body ElseTail END { $$ = CreateIfStatement(@$, $2, $4, $5); }
    ;

ElseTail
    : { $$ = null; }
    | ELSE Body { $$ = $2; }
    ;

ReturnStatement
    : RETURN { $$ = CreateReturn(@$, null); }
    | RETURN Expression { $$ = CreateReturn(@$, $2); }
    ;

Expression
    : ConstructorInvocation
    | MethodCall
    | ValueGetting
    | Primary
    ;

ConstructorInvocation
    : ClassName LPARENT Arguments RPARENT { $$ = CreateConstructorInvocation(@$, $1, $3); }
    ;

MethodCall
    : Expression DOT IDENTIFIER LPARENT Arguments RPARENT { $$ = CreateMethodCall(@$, $1, $3, $5); }
    ;

ValueGetting
    : Expression DOT IDENTIFIER { $$ = CreateValueGetting(@$, $1, $3); }
    ;

Arguments
    : { $$ = CreateNewList(@$); }
    | Expression { $$ = CreateNewList(@$, $1); }
    | Arguments COMMA Expression { AddToList(@$, $$, $3); $$ = $1; }
    ;

Primary
    : INTEGER
    | REAL
    | BOOLEAN
    | STRING
    | ThisStatement
    | ClassOrVariableExpression
    ;

ThisStatement
    : THIS { $$ = CreateThisNode(@$, $1); }
    ;

ClassOrVariableExpression
    : IDENTIFIER { $$ = $1; }
    | IDENTIFIER GenericParameter { $$ = CreateClassName(@$, $1, $2); }
    ;

%%
