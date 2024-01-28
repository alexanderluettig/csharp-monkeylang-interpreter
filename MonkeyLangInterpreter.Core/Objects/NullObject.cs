namespace MonkeyLangInterpreter.Core.Objects;

public class NullObject : IObject
{
    public string Inspect()
    {
        return "null";
    }

    public ObjectType Type()
    {
        return ObjectType.Null;
    }
}
