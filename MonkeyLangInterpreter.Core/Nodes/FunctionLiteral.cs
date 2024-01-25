using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class FunctionLiteral(List<Identifier> identifiers, BlockStatement body) : IExpression
{
    public Token Token { get; init; } = new(TokenType.FUNCTION, "fn");
    public List<Identifier> Parameters { get; init; } = identifiers;
    public BlockStatement Body { get; init; } = body;

    public string String()
    {
        return $"{TokenLiteral()}({string.Join(", ", Parameters.Select(p => p.String()))}) {Body.String()}";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
