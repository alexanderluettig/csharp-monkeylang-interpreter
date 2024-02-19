using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class ArrayLiteral(IEnumerable<IExpression> expressions) : IExpression
{
    public Token Token { get; init; } = new(TokenType.LBRACKET, "[");
    public List<IExpression> Elements { get; init; } = expressions.ToList();
    public string String()
    {
        return $"[{string.Join(", ", Elements)}]";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
