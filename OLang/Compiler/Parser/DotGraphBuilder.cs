using System;
using System.Text;
using DotNetGraph.Attributes;
using DotNetGraph.Core;
using DotNetGraph.Extensions;
using DotNetGraph.Compilation;
using OLang.Compiler.Parser.Structure.Members;
using OLang.Compiler.Parser.Structure.Members.Implementations;
using OLang.Compiler.Parser.Structure.Statements;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser;

class DotGraphBuilder
{
    public DotGraph graph = new DotGraph().WithIdentifier("program graph").Directed();

    public void AddProgram(Structure.Program program)
    {
        var node = new DotNode().WithIdentifier("Program").WithLabel("Program");
        graph.Add(node);

        foreach (var cls in program.Classes)
        {
            var classNode = AddClass(cls);
            graph.Add(classNode);

            var edge = new DotEdge().From(node).To(classNode);
            graph.Add(edge);
        }
    }

    public DotNode AddClass(Class cls)
    {
        var node = new DotNode()
                    .WithIdentifier($"{cls.GetHashCode()}class")
                    .WithLabel($"Class");
        graph.Add(node);

        var nameNode = AddClassName(cls.Name);
        graph.Add(nameNode);

        graph.Add(new DotEdge().From(node).To(nameNode));

        if (cls.Extends != null)
        {
            var extend = AddClassName(cls.Extends!);
            graph.Add(extend);
            graph.Add(new DotEdge().From(node).To(extend).WithLabel("Extends"));
        }

        foreach (var member in cls.Members)
        {
            var memberNode = AddMember(member);
            graph.Add(memberNode);

            var edge = new DotEdge().From(node).To(memberNode);
            graph.Add(edge);
        }

        return node;
    }

    public DotNode AddMember(MemberDeclaration member)
    {
        var node = member switch
        {
            Constructor c => AddConstructor(c),
            Field f => AddField(f),
            Method m => AddMethod(m),
            _ => throw new Exception($"Undefined member")
        };

        graph.Add(node);
        return node;
    }

    public DotNode AddParameter(Parameter param)
    {
        var node = new DotNode()
                    .WithIdentifier(param.GetHashCode().ToString())
                    .WithLabel($"Parameter {param.Name}");

        var typ = AddClassName(param.Type);
        graph.Add(new DotEdge().From(node).To(typ).WithLabel("Type"));

        graph.Add(node);
        return node;
    }

    public DotNode AddField(Field field)
    {

        StringBuilder label = new StringBuilder();

        if (field.IsStatic)
        {
            label.Append("Static ");
        }
        label.Append("Field ");
        label.Append(field.Name);

        var node = new DotNode()
                        .WithIdentifier(field.GetHashCode().ToString())
                        .WithLabel(label.ToString());
        graph.Add(node);

        if (field.Type != null)
        {
            var typ = AddClassName(field.Type!);
            graph.Add(new DotEdge().From(node).To(typ).WithLabel("Type"));
        }
        
        var exprNode = AddExpression(field.Expression);
        graph.Add(exprNode);

        var edge = new DotEdge().From(node).To(exprNode);
        graph.Add(edge);

        return node;
    }

    public DotNode AddMethod(Method method)
    {

        StringBuilder label = new StringBuilder();

        if (method.IsStatic)
        {
            label.Append("Static ");
        }
        label.Append("Method ");
        label.Append(method.Name);

        var node = new DotNode()
                        .WithIdentifier(method.GetHashCode().ToString())
                        .WithLabel(method.ToString());
        graph.Add(node);

        foreach(var statement in method.Body)
        {
            var statementNode = AddStatement(statement);
            graph.Add(statementNode);

            var edge = new DotEdge()
                .From(node)
                .To(statementNode);

            graph.Add(edge);
        }

        return node;
    }

    public DotNode AddConstructor(Constructor constructor)
    {
        var node = new DotNode()
                    .WithIdentifier($"{constructor.GetHashCode()}while")
                    .WithLabel($"Constructor");
        
        foreach(var statement in constructor.Body)
        {
            var statementNode = AddStatement(statement);

            var edge = new DotEdge()
                .From(node)
                .To(statementNode);

            graph.Add(edge);
        }
        
        return node;
    }

    public DotNode AddStatement(Statement statement)
    {
        var node = statement switch
        {
            Variable v => AddVariable(v),
            Assigment a => AddAssigment(a),
            WhileLoop w => new DotNode()
                                .WithIdentifier($"{statement.GetHashCode()}while")
                                .WithLabel($"While"),
            IfStatement i => AddIfStatement(i),
            ReturnStatement r => AddReturn(r),
            Expression e => AddExpression(e),
            _ => throw new Exception($"Undefined statement")
        };

        graph.Add(node);
        return node;
    }

    public DotNode AddExpression(Expression expr)
    {
        var node = expr switch
        {
            ConstructorInvocation ci => AddConstructorInvocation(ci),
            MethodCall mc => AddMethodCall(mc),
            ValueGetting vg => AddValueGetting(vg),
            ClassName cn => AddClassName(cn),
            Expression e => new DotNode().WithIdentifier($"{e.GetHashCode()}").WithLabel("???") // throw new Exception($"Undefined expression {expr}")
        };

        graph.Add(node);
        return node;
    }

    public DotNode AddConstructorInvocation(ConstructorInvocation expr)
    {
        var classNameNode = AddClassName(expr.ClassName);
        graph.Add(classNameNode);

        var node = new DotNode()
                        .WithIdentifier($"{expr.GetHashCode()}")
                        .WithLabel("Constructor Invocation");
        graph.Add(node);

        var edge = new DotEdge().From(node).To(classNameNode);
        graph.Add(edge);

        return node;
    }

    public DotNode AddMethodCall(MethodCall expr)
    {
        var node = new DotNode()
                        .WithIdentifier($"Method {expr.MethodName} call")
                        .WithLabel($"Method {expr.MethodName} call");
        graph.Add(node);
        return node;
    }

    public DotNode AddValueGetting(ValueGetting expr)
    {
        var node = new DotNode()
                        .WithIdentifier($"Field {expr.FieldName} getting")
                        .WithLabel($"Field {expr.FieldName} getting");
        graph.Add(node);
        return node;
    }

    public DotNode AddClassName(ClassName expr)
    {
        var node = new DotNode()
                        .WithIdentifier($"Class Name {expr.Name}")
                        .WithLabel($"Class Name {expr.Name}");
        graph.Add(node);
        return node;
    }

    public DotNode AddIfStatement(IfStatement ifst)
    {
        var node = new DotNode()
                        .WithIdentifier($"{ifst.GetHashCode()}if")
                        .WithLabel("if");
        graph.Add(node);

        var cond = AddExpression(ifst.Condition);
        graph.Add(cond);

        graph.Add(new DotEdge().From(node).To(cond));

        var thn = new DotNode()
                    .WithIdentifier($"{ifst.GetHashCode()}then")
                    .WithLabel("then");
        graph.Add(thn);
        
        graph.Add(new DotEdge().From(node).To(thn));

        foreach (var sttmnt in ifst.TrueBlock) {
            var sttmntNode = AddStatement(sttmnt);
            graph.Add(sttmntNode);
            graph.Add(new DotEdge().From(thn).To(sttmntNode));
        }

        if (ifst.FalseBlock != null)
        {
            var els = new DotNode()
                    .WithIdentifier($"{ifst.GetHashCode()}else")
                    .WithLabel("else");
            graph.Add(els);
            graph.Add(new DotEdge().From(node).To(els));

            foreach (var sttmnt in ifst.FalseBlock!) {
                var sttmntNode = AddStatement(sttmnt);
                graph.Add(sttmntNode);
                graph.Add(new DotEdge().From(els).To(sttmntNode));
            }
        }

        return node;
    }

    public DotNode AddReturn(ReturnStatement r)
    {
        var node = new DotNode()
                        .WithIdentifier($"{r.GetHashCode()}")
                        .WithLabel("return");
        graph.Add(node);
        
        if (r.Expression != null)
        {
            var expr = AddExpression(r.Expression!);
            graph.Add(new DotEdge().From(node).To(expr));
        }        
        
        return node;
    }

    public DotNode AddVariable(Variable v)
    {
        var node = new DotNode()
                        .WithIdentifier(v.Name)
                        .WithLabel(v.Name);
        graph.Add(node);

        var expr = AddExpression(v.Expression);
        graph.Add(new DotEdge().From(node).To(expr));

        return node;
    }

    public DotNode AddAssigment(Assigment a)
    {
        var node = new DotNode()
                        .WithIdentifier($"{a.GetHashCode()}")
                        .WithLabel($"{a.VariableName} =");
        graph.Add(node);

        var expr = AddExpression(a.Expression);
        graph.Add(new DotEdge().From(node).To(expr));

        return node;
    }
}
