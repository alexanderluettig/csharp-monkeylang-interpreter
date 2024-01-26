using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class CallExpression(IExpression function, List<IExpression> arguments) : IExpression
{
    public Token Token { get; init; } = new Token(TokenType.LPAREN, "(");
    public IExpression Function { get; init; } = function;
    public List<IExpression> Arguments { get; init; } = arguments;

    public string String()
    {

        return $"{Function.String()}({string.Join(", ", Arguments.Select(x => x.String()))})";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
