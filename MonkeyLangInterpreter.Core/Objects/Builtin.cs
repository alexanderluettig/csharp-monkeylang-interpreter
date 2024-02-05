namespace MonkeyLangInterpreter.Core.Objects;

public class Builtin(Func<IList<IObject>, IObject> func) : IObject
{
    public Func<IList<IObject>, IObject> Fn { get; } = func;
    public string Inspect()
    {
        return "builtin function";
    }

    public ObjectType Type()
    {
        return ObjectType.BUILTIN;
    }
}
