using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class IntegerLiteral(Token token) : IExpression
{
    public Token Token { get; init; } = token;
    public int Value => int.Parse(Token.Literal);

    public string String()
    {
        return Value.ToString();
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
