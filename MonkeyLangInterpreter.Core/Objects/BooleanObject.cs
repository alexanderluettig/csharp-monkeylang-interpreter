namespace MonkeyLangInterpreter.Core.Objects;

public class BooleanObject(bool value) : IObject, IHashable
{
    public bool Value { get; set; } = value;
    public string Inspect()
    {
        return Value.ToString();
    }

    public ObjectType Type()
    {
        return ObjectType.BOOLEAN;
    }

    public HashKey HashKey()
    {
        return new HashKey(Type(), (ulong)Value.GetHashCode());
    }
}
