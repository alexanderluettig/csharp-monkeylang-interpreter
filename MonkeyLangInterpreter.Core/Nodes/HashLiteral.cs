using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core;

public class HashLiteral(Dictionary<IExpression, IExpression> pairs) : IExpression
{
    public Token Token { get; init; } = new(TokenType.LBRACE, "{");
    public Dictionary<IExpression, IExpression> Pairs { get; init; } = pairs;
    public string String()
    {
        return $"{{{string.Join(", ", Pairs.Select(x => $"{x.Key.String()}:{x.Value.String()}"))}}}";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
