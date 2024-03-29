﻿namespace MonkeyLangInterpreter.Core.Objects;

public class StringObject(string value) : IObject, IHashable
{
    public string Value { get; set; } = value;

    public string Inspect()
    {
        return Value;
    }

    public ObjectType Type()
    {
        return ObjectType.STRING;
    }

    public HashKey HashKey()
    {
        return new HashKey(Type(), (ulong)Value.GetHashCode());
    }
}
