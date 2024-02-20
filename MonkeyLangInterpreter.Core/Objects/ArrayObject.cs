using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Core;

public class ArrayObject(IEnumerable<IObject> objects) : IObject
{
    public List<IObject> Elements { get; init; } = objects.ToList();
    public string Inspect()
    {
        return $"[{string.Join(", ", Elements.Select(x => x.Inspect()))}]";
    }

    public ObjectType Type()
    {
        return ObjectType.ARRAY;
    }
}
