using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Core;

public struct HashPair(IHashable key, IObject value)
{
    public IHashable Key { get; init; } = key;
    public IObject Value { get; init; } = value;
}
