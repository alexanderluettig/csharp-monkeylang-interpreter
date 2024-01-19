using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class InfixExpression(Token token, IExpression left, string @operator, IExpression right) : IExpression
{
    public Token Token { get; init; } = token;
    public IExpression Left { get; init; } = left;
    public string Operator { get; init; } = @operator;
    public IExpression Right { get; init; } = right;

    public string String()
    {
        return $"({Left.String()} {Operator} {Right.String()})";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
