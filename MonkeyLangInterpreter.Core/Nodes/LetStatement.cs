using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class LetStatement(Identifier name, IExpression value) : IStatement
{
    public Token Token { get; } = new Token(TokenType.LET, "let");
    public Identifier Name { get; set; } = name;
    public IExpression Value { get; set; } = value;

    public string String()
    {
        return $"{TokenLiteral()} {Name.String()} = {Value.String()};";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
