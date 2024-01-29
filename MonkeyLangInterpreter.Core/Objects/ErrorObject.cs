
namespace MonkeyLangInterpreter.Core.Objects;

public class ErrorObject(string message) : IObject
{
    public string Message { get; set; } = message;
    public string Inspect()
    {
        return $"ERROR: {Message}";
    }

    public ObjectType Type()
    {
        return ObjectType.ERROR;
    }
}
