namespace MonkeyLangInterpreter.Core.Objects;

public class StringObject(string value) : IObject
{
    public string Value { get; set; } = value;

    public string Inspect()
    {
        return Value;
    }

    public ObjectType Type()
    {
        return ObjectType.STRING;
    }
}
