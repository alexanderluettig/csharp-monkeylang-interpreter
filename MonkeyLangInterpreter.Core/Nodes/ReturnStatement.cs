using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class ReturnStatement(IExpression returnValue) : IStatement
{
    public Token Token { get; init; } = new(TokenType.RETURN, "return");
    public IExpression ReturnValue { get; init; } = returnValue;

    public string String()
    {
        return $"{TokenLiteral()} {ReturnValue?.String() ?? ""};";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
