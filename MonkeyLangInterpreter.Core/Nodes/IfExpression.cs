using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class IfExpression(Token token, IExpression condition, BlockStatement consequence, BlockStatement alternative) : IExpression
{
    public Token Token { get; init; } = token;
    public IExpression Condition { get; init; } = condition;
    public BlockStatement Consequence { get; init; } = consequence;
    public BlockStatement Alternative { get; init; } = alternative;
    public string String()
    {
        return $"if {Condition.String()} {Consequence.String()} else {Alternative.String()}";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
