namespace MonkeyLangInterpreter.Core.Objects;

public class ReturnValue(IObject value) : IObject
{
    public IObject Value { get; } = value;

    public string Inspect() => Value.Inspect();

    public ObjectType Type() => ObjectType.RETURN_VALUE;
}
