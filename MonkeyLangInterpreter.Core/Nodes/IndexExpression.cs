using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core;

public class IndexExpression(IExpression left, IExpression index) : IExpression
{
    public Token Token { get; init; } = new Token(TokenType.LBRACKET, "[");
    public IExpression Left { get; init; } = left;
    public IExpression Index { get; init; } = index;

    public string String()
    {
        return $"({Left.String()}[{Index.String()}])";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
