using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class Identifier(string value) : IExpression
{
    public Token Token { get; } = new Token(TokenType.IDENT, value);
    public string Value { get; set; } = value;

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
