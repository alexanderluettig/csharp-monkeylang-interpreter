namespace MonkeyLangInterpreter.Core.Objects;

public interface IObject
{
    string Inspect();
    ObjectType Type();
}
