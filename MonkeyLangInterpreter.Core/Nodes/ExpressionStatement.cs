using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class ExpressionStatement(Token token, IExpression expression) : IStatement
{
    public Token Token { get; init; } = token;
    public IExpression Expression { get; init; } = expression;

    public string String()
    {
        return Expression?.String() ?? "";
    }

    public string TokenLiteral() => Token.Literal;
}