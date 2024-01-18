using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class PrefixExpression(Token token, string @operator, IExpression expression) : IExpression
{
    public Token Token { get; init; } = token;
    public string Operator { get; init; } = @operator;
    public IExpression Right { get; init; } = expression;

    public string String()
    {
        return $"({Operator}{Right.String()})";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
