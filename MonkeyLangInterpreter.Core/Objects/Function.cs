using MonkeyLangInterpreter.Core.Nodes;

namespace MonkeyLangInterpreter.Core.Objects;

public class Function(List<Identifier> parameters, BlockStatement body, VariableEnvironment env) : IObject
{
    public List<Identifier> Parameters { get; set; } = parameters;
    public BlockStatement Body { get; set; } = body;
    public VariableEnvironment Environment { get; set; } = env;
    public string Inspect()
    {
        var parameters = new List<string>();
        foreach (var parameter in Parameters)
        {
            parameters.Add(parameter.String());
        }
        return $"fn({string.Join(", ", parameters)}) {{\n{Body.String()}\n}}";
    }

    public ObjectType Type()
    {
        return ObjectType.FUNCTION;
    }
}
