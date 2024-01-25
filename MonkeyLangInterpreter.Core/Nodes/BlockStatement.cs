using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class BlockStatement(List<IStatement> statements) : IStatement
{
    public Token Token { get; init; } = new(TokenType.LBRACE, "{");
    public List<IStatement> Statements { get; init; } = statements;
    public string String()
    {
        return string.Join("", Statements.Select(s => s.String()));
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}
