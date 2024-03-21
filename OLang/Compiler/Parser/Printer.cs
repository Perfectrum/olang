using System.Text;
using OLang.Compiler.Parser.Structure;
using OLang.Compiler.Parser.Structure.Members;
using OLang.Compiler.Parser.Structure.Members.Implementations;
using OLang.Compiler.Parser.Structure.Statements;
using OLang.Compiler.Parser.Structure.Statements.Implementations;

namespace OLang.Compiler.Parser;

public static class Printer
{
    private class ConverterAstToCStyleCode : IMemberDeclarationVisitor, IStatementVisitor
    {
        private readonly StringBuilder _builder = new();
        private int _tabIndex;

        public string GetProgram(Structure.Program program)
        {
            _builder.Clear();
            _tabIndex = 0;
            Visit(program);
            return _builder.ToString();
        }

        public void Visit(Structure.Program program)
        {
            Append("Program");
            AppendCollectionWithTab(program.Classes, Visit);
        }

        public void Visit(Parameter parameter)
        {
            Visit(parameter.Type);
            Append($" {parameter.Name}");
        }

        public void Visit(Field field)
        {
            Append($"{(field.IsStatic ? "static " : "")}");
            if (field.Type is null)
                Append("var ");
            else
                Visit(field.Type);
            
            Append($" {field.Name} = ");
            field.Expression.Accept(this);
        }

        public void Visit(Constructor constructor)
        {
            Append("this ");
            AppendCollectionWithoutTab(constructor.Parameters, Visit);
            if (constructor.LocalConstructorInvocation is not null)
            {
                var localConstructorIdentifier = constructor.LocalConstructorInvocation.ConstructorType switch
                {
                    LocalConstructorIdentifierType.This => "this",
                    LocalConstructorIdentifierType.Base => "base",
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                Append($" : {localConstructorIdentifier}");
                AppendCollectionWithoutTab(constructor.LocalConstructorInvocation.Arguments, node => node.Accept(this));
            }
            AppendCollectionWithTab(constructor.Body, node => node.Accept(this));
        }

        public void Visit(Method method)
        {
            Append($"{(method.IsStatic ? "static " : "")}");
            if (method.ReturnType != null)
                Visit(method.ReturnType);
            else
                Append("void");
            Append($" {method.Name}");
            AppendCollectionWithoutTab(method.Parameters, Visit);
            AppendCollectionWithTab(method.Body, node => node.Accept(this));
        }

        public void Visit(Class @class)
        {
            Append("class ");
            Visit(@class.Name);
            if (@class.Extends is not null)
            {
                Append(" : ");
                Visit(@class.Extends);
            }
            
            AppendCollectionWithTab(@class.Members, node => node.Accept(this));
        }

        public void Visit(ClassName className)
        {
            Append($"{className.Name}");
            if (className.GenericArgument is null)
                return;
            
            Append("[");
            Visit(className.GenericArgument);
            Append("]");
        }

        public void Visit(BooleanNode booleanNode) => Append(booleanNode.Value.ToString());

        public void Visit(StringLiteralNode stringLiteralNode) => Append($"\"{stringLiteralNode.Value}\"");

        public void Visit(RealNode realNode) => Append(realNode.Value.ToString());

        public void Visit(IntegerNode integerNode) => Append(integerNode.Value.ToString());

        public void Visit(IdentifierNode identifierNode) => Append(identifierNode.Value);

        public void Visit(IfStatement ifStatement)
        {
            Append("if (");
            ifStatement.Condition.Accept(this);
            Append(")");
            AppendCollectionWithTab(ifStatement.TrueBlock, node => node.Accept(this));
            if (ifStatement.FalseBlock is not null)
            {
                _builder.AppendLine();
                AppendWithTabs("else");
                AppendCollectionWithTab(ifStatement.FalseBlock, node => node.Accept(this));
            }
        }

        public void Visit(Variable variable)
        {
            if (variable.Type is null)
                Append("var");
            else
                Visit(variable.Type);
            
            Append($" {variable.Name} = ");
            variable.Expression.Accept(this);
        }

        public void Visit(ThisNode thisNode) => Append("this");

        public void Visit(Assigment assigment)
        {
            assigment.AssignmentValue.Accept(this);
            Append(" = ");
            assigment.Expression.Accept(this);
        }

        public void Visit(WhileLoop whileLoop)
        {
            Append("while ");
            whileLoop.Condition.Accept(this);
            AppendCollectionWithTab(whileLoop.Body, node => node.Accept(this));
        }

        public void Visit(ReturnStatement returnStatement)
        {
            Append("return");
            if (returnStatement.Expression is null)
                return;
            
            Append(" ");
            returnStatement.Expression.Accept(this);
        }

        public void Visit(ConstructorInvocation constructorInvocation)
        {
            Append("new ");
            Visit(constructorInvocation.ClassName);
            AppendCollectionWithoutTab(constructorInvocation.Arguments, node => node.Accept(this));
        }

        public void Visit(MethodCall methodCall)
        {
            methodCall.Expression.Accept(this);
            Append($".{methodCall.MethodName}");
            AppendCollectionWithoutTab(methodCall.Arguments, node => node.Accept(this));
        }

        public void Visit(ValueGetting valueGetting)
        {
            valueGetting.Expression.Accept(this);
            Append($".{valueGetting.FieldName}");
        }

        private void AppendCollectionWithTab<T>(
            IEnumerable<T> collection, Action<T> actionOnNode) where T : Node
        {
            Append(" {");
            _builder.AppendLine();
            ++_tabIndex;
            foreach (var node in collection)
            {
                AppendTabs();
                actionOnNode(node);
                _builder.AppendLine();
            }
            --_tabIndex;
            AppendWithTabs("}");
        }
        
        private void AppendCollectionWithoutTab<T>(
            IEnumerable<T> collection, Action<T> actionOnNode) where T : Node
        {
            Append("(");
            ++_tabIndex;
            foreach (var node in collection)
            {
                actionOnNode(node);
                Append(", ");
            }
            if (collection.Any())
                _builder.Remove(_builder.Length - 2, 2);
            --_tabIndex;
            Append(")");
        }

        private void AppendTabs() => _builder.Append(' ', _tabIndex * 4);
        
        private void Append(string? str) => _builder.Append(str);
        private void AppendWithTabs(string str)
        {
            AppendTabs();
            _builder.Append(str);
        }
    }
    
    public static void PrintCStyle(Structure.Program program, StreamWriter stream) =>
        stream.WriteLine(new ConverterAstToCStyleCode().GetProgram(program));
}