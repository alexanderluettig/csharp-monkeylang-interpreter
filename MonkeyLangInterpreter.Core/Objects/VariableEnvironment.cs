namespace MonkeyLangInterpreter.Core.Objects;

public class VariableEnvironment(VariableEnvironment? outer = null)
{
    private readonly Dictionary<string, IObject> _store = [];
    private readonly VariableEnvironment? _outer = outer;

    public IObject Get(string name)
    {
        if (_store.TryGetValue(name, out var value))
        {
            return value;
        }
        if (_outer is not null)
        {
            return _outer.Get(name);
        }
        return new ErrorObject($"identifier not found: {name}");
    }

    public IObject Set(string name, IObject value)
    {
        _store[name] = value;
        return value;
    }
}
