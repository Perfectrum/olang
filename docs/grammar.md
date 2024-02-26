```
Program : { ClassDeclaration }

ClassDeclaration :
    class ClassName [ extends ClassName ] is
        { MemberDeclaration }
    end

ClassName : Identifier [ '[' ClassName ']' ]

MemberDeclaration :
      FieldDeclaration
    | MethodDeclaration
    | ConstructorDeclaration
    | FunctionDeclaration
    | StaticDeclaration

FieldDeclaration :
    field Identifier ':' Expression 'is' Expression

StaticDeclaration :
    static Identifier ':' Expression 'is' Expression

MethodDeclaration :
    method Identifier '(' Parameters ')' [ ':' Identifier ]
    is Body end

FunctionDeclaration :
    function Identifier '(' Parameters ')' [ ':' Identifier ]
    is Body end

Parameters : [ ParameterDeclaration { , ParameterDeclaration } ]

ParameterDeclaration : Identifier ':' ClassName

Body : { VariableDeclaration | Statement }

VariableDeclaration : let Identifier ':' Identifier '=' Expression

ConstructorDeclaration : this '(' [ Parameters ] ')' is Body end

Statement :
      Assignment
    | WhileLoop
    | IfStatement
    | ReturnStatement

Assignment : Identifier '=' Expression

WhileLoop : while Expression loop Body end

IfStatement : if Expression then Body [ else Body ] end

ReturnStatement : return [ Expression ]

Expression : Primary { '.' Identifier [ '(' Arguments ')' ] }

Arguments : Expression { ',' Expression }

Primary :
      IntegerLiteral
    | RealLiteral
    | BooleanLiteral
    | this
    | ClassName
    | super
```