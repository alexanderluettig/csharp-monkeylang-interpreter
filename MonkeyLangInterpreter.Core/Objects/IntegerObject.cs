namespace MonkeyLangInterpreter.Core.Objects;

public class IntegerObject(int value) : IObject
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
}
