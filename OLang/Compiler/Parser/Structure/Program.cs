using OLang.Compiler.Overall;
using OLang.Compiler.Parser.Structure.Members.Implementations;

namespace OLang.Compiler.Parser.Structure;

public class Program(Position position, List<Class> classes) : Node(position)
{
    public List<Class> Classes { get; } = classes;
}