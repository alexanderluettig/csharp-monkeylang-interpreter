namespace MonkeyLangInterpreter.Core.Objects;

public class BooleanObject(bool value) : IObject
{
    public bool Value { get; set; } = value;
    public string Inspect()
    {
        return Value.ToString();
    }

    public ObjectType Type()
    {
        return ObjectType.Boolean;
    }
}
