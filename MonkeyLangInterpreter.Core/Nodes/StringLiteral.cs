using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class StringLiteral(Token token) : IExpression
{
    public Token Token { get; init; } = token;
    public string Value => Token.Literal;
    public string String()
    {
        return Token.Literal;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
