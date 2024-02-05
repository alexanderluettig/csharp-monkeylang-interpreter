namespace MonkeyLangInterpreter.Core.Objects;

public static class BuiltinFunctions
{
    public static IObject Len(IList<IObject> args)
    {
        if (args.Count != 1)
        {
            return new ErrorObject($"wrong number of arguments. got={args.Count}, want=1");
        }

        switch (args[0])
        {
            case StringObject s:
                return new IntegerObject(s.Value.Length);
            default:
                return new ErrorObject($"argument to `len` not supported, got {args[0].Type()}");
        }
    }
}
