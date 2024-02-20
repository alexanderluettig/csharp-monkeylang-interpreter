namespace MonkeyLangInterpreter.Core.Objects;

public class IntegerObject(int value) : IObject, IHashable
{
    public int Value { get; set; } = value;
    public string Inspect()
    {
        return Value.ToString();
    }

    public ObjectType Type()
    {
        return ObjectType.INTEGER;
    }

    public HashKey HashKey()
    {
        return new HashKey(Type(), (ulong)Value.GetHashCode());
    }
}
