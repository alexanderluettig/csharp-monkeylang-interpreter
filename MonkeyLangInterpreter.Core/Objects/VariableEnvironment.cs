namespace MonkeyLangInterpreter.Core.Objects;

public class VariableEnvironment
{
    private readonly Dictionary<string, IObject> _store = new();

    public IObject Get(string name)
    {
        if (_store.TryGetValue(name, out var value))
        {
            return value;
        }
        else
        {
            return new ErrorObject($"identifier not found: {name}");
        }
    }

    public void Set(string name, IObject value)
    {
        _store[name] = value;
    }
}
