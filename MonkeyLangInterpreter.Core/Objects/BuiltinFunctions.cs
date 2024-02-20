namespace MonkeyLangInterpreter.Core.Objects;

public static class BuiltinFunctions
{
    public static IObject Len(IList<IObject> args)
    {
        if (args.Count != 1)
        {
            return new ErrorObject($"wrong number of arguments. got={args.Count}, want=1");
        }

        return args[0] switch
        {
            StringObject s => new IntegerObject(s.Value.Length),
            ArrayObject a => new IntegerObject(a.Elements.Count),
            _ => new ErrorObject($"argument to `len` not supported, got {args[0].Type()}"),
        };
    }

    public static IObject First(IList<IObject> args)
    {
        if (args.Count != 1)
        {
            return new ErrorObject($"wrong number of arguments. got={args.Count}, want=1");
        }

        if (args[0].Type() != ObjectType.ARRAY)
        {
            return new ErrorObject($"argument to `first` must be ARRAY, got {args[0].Type()}");
        }

        var arr = (ArrayObject)args[0];
        if (arr.Elements.Count > 0)
        {
            return arr.Elements[0];
        }

        return new NullObject();
    }

    public static IObject Last(IList<IObject> args)
    {
        if (args.Count != 1)
        {
            return new ErrorObject($"wrong number of arguments. got={args.Count}, want=1");
        }

        if (args[0].Type() != ObjectType.ARRAY)
        {
            return new ErrorObject($"argument to `last` must be ARRAY, got {args[0].Type()}");
        }

        var arr = (ArrayObject)args[0];
        var length = arr.Elements.Count;
        if (length > 0)
        {
            return arr.Elements[length - 1];
        }

        return new NullObject();
    }

    public static IObject Rest(IList<IObject> args)
    {
        if (args.Count != 1)
        {
            return new ErrorObject($"wrong number of arguments. got={args.Count}, want=1");
        }

        if (args[0].Type() != ObjectType.ARRAY)
        {
            return new ErrorObject($"argument to `rest` must be ARRAY, got {args[0].Type()}");
        }

        var arr = (ArrayObject)args[0];
        var length = arr.Elements.Count;
        if (length > 0)
        {
            var newElements = arr.Elements.Skip(1).ToList();
            return new ArrayObject(newElements);
        }

        return new NullObject();
    }

    public static IObject Push(IList<IObject> args)
    {
        if (args.Count != 2)
        {
            return new ErrorObject($"wrong number of arguments. got={args.Count}, want=2");
        }

        if (args[0].Type() != ObjectType.ARRAY)
        {
            return new ErrorObject($"argument to `push` must be ARRAY, got {args[0].Type()}");
        }

        var arr = (ArrayObject)args[0];
        var newElements = arr.Elements.ToList();
        newElements.Add(args[1]);
        return new ArrayObject(newElements);
    }
}
