using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class StringLiteral(Token token, string value) : IExpression
{
    public Token Token { get; init; } = token;
    public string Value { get; init; } = value;
    public string String()
    {
        return Token.Literal;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
