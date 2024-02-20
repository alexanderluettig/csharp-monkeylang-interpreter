using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Core;

public struct HashKey(ObjectType type, ulong value) : IObject
{
    public ObjectType ObjectType { get; init; } = type;
    public ulong Value { get; init; } = value;

    public readonly string Inspect()
    {
        return $"{ObjectType} {Value}";
    }

    public ObjectType Type()
    {
        throw new NotImplementedException();
    }
}