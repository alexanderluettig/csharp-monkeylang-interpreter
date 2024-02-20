using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Core;

public class Hash(Dictionary<HashKey, HashPair> pairs) : IObject
{
    public Dictionary<HashKey, HashPair> Pairs { get; set; } = pairs;
    public string Inspect()
    {
        var pairs = Pairs.Select(pair => $"{pair.Key.Inspect()}:{pair.Value.Value.Inspect()}");
        return $"{{{string.Join(", ", pairs)}}}";
    }

    public ObjectType Type()
    {
        return ObjectType.HASH;
    }
}
