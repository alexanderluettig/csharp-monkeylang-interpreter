using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class BooleanExpression(Token token, bool value) : IExpression
{
    public Token Token { get; init; } = token;
    public bool Value { get; init; } = value;
    public string String()
    {
        return Token.Literal;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
