```
Program : { ClassDeclaration }

ClassDeclaration :
    'class' ClassName [ extends ClassName ] 'is'
        { MemberDeclaration ';' }
    'end'

ClassName : Identifier [ '[' ClassName ']' ]

MemberDeclaration :
      FieldDeclaration
    | MethodDeclaration
    | ConstructorDeclaration
    | FunctionDeclaration
    | StaticDeclaration

FieldDeclaration :
    'field' Identifier ':' Identifier 'is' Expression

StaticDeclaration :
    'static' Identifier ':' Identifier 'is' Expression

MethodDeclaration :
    'method' Identifier '(' Parameters ')' [ ':' Identifier ]
    'is' Body 'end'

FunctionDeclaration :
    'function' Identifier '(' Parameters ')' [ ':' Identifier ]
    'is' Body 'end'

ConstructorDeclaration : this '(' Parameters ')' 'is' Body 'end'

Parameters : [ ParameterDeclaration { ',' ParameterDeclaration } ]

ParameterDeclaration : Identifier ':' ClassName

Body : { Statement ';' }

VariableDeclaration : 'let' Identifier [ ':' Identifier ] '=' Expression ';'

Statement :
      VariableDeclaration
    | Assignment
    | WhileLoop
    | IfStatement
    | ReturnStatement
    | Expression

Assignment : Identifier '=' Expression

WhileLoop : 'while' Expression 'loop' Body 'end'

IfStatement : 'if' Expression 'then' Body [ else Body ] 'end'

ReturnStatement : 'return' [ Expression ]

Expression : ConstructorInvocation
           | MethodCall
           | ValueGetting

ConstructorInvocation : Identifier '(' Arguments ')'

MethodCall : Primary '.' Identifier '(' Arguments ')'

ValueGetting : Primary '.' Identifier

Arguments : [ Expression { ',' Expression } ]

Primary :
      IntegerLiteral
    | RealLiteral
    | BooleanLiteral
    | this
    | ClassName
    | super
```
