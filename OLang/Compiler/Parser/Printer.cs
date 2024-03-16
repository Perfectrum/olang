using System.Text;

namespace OLang.Compiler.Parser;

public static class Printer
{
    private class ConverterAstToCStyleCode : IMemberDeclarationVisitor, IStatementVisitor
    {
        private readonly StringBuilder _builder = new();
        private int _tabIndex;

        public string GetProgram(Program program)
        {
            _builder.Clear();
            _tabIndex = 0;
            Visit(program);
            return _builder.ToString();
        }

        public void Visit(Program program)
        {
            Append("Program");
            AppendCollectionWithTab(program.Classes, Visit);
        }

        public void Visit(Parameter parameter) =>
            Append($"{parameter.Type} {parameter.Name}");

        public void Visit(Field field)
        {
            Append($"{(field.IsStatic ? "static " : "")}");
            if (field.Type is null)
                Append("var ");
            else
                Visit(field.Type);
            
            Append($"{field.Name} = ");
            field.Expression.Accept(this);
        }

        public void Visit(Constructor constructor)
        {
            Append("this ");
            AppendCollectionWithoutTab(constructor.Parameters, Visit);
            AppendCollectionWithTab(constructor.Body, node => node.Accept(this));
        }

        public void Visit(Method method)
        {
            Append($"{(method.IsStatic ? "static" : "")} method {method.Name}, ");
            AppendCollectionWithTab(method.Body, node => node.Accept(this));
            AppendCollectionWithoutTab(method.Parameters, Visit);
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

        public void Visit(Assigment assigment)
        {
            Append($"{assigment.VariableName} = ");
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

        public void Visit<T>(ValueNode<T> valueNode) => Append(valueNode.Value!.ToString());

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
    
    public static void Print(Program program, StreamWriter stream) =>
        stream.WriteLine(new ConverterAstToCStyleCode().GetProgram(program));
}