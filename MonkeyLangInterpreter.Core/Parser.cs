using MonkeyLangInterpreter.Core.Interfaces;
using MonkeyLangInterpreter.Core.Nodes;

namespace MonkeyLangInterpreter.Core;

public class Parser
{
    private Lexer Lexer { get; init; }
    private Token _currentToken;
    private Token _peekToken;
    public List<string> Errors { get; internal set; } = [];

    public Parser(Lexer lexer)
    {
        Lexer = lexer;

        NextToken();
        NextToken();
    }

    public void NextToken()
    {
        _currentToken = _peekToken;
        _peekToken = Lexer.NextToken();
    }

    public Program ParseProgram()
    {
        Program program = new();

        while (!CurrentTokenIs(TokenType.EOF))
        {
            var statement = ParseStatement();
            if (statement != null)
            {
                program.Statements.Add(statement);
            }

            NextToken();
        }

        return program;
    }

    private IStatement ParseStatement()
    {
        return _currentToken.Type switch
        {
            TokenType.LET => ParseLetStatement(),
            _ => null!,
        };
    }

    private LetStatement ParseLetStatement()
    {

        if (!ExpectPeek(TokenType.IDENT))
        {
            return null!;
        }

        var name = new Identifier(_currentToken.Literal);

        if (!ExpectPeek(TokenType.ASSIGN))
        {
            return null!;
        }

        //TODO: We're skipping the expressions until we encounter a semicolon


        while (CurrentTokenIs(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new LetStatement(name, null!);
    }

    private bool CurrentTokenIs(TokenType type)
    {
        return _currentToken.Type == type;
    }

    private bool PeekTokenIs(TokenType type)
    {
        return _peekToken.Type == type;
    }

    private bool ExpectPeek(TokenType type)
    {
        if (PeekTokenIs(type))
        {
            NextToken();
            return true;
        }
        else
        {
            PeekError(type);
            return false;
        }
    }

    private void PeekError(TokenType type)
    {
        Errors.Add($"expected next token to be {type}, got {_peekToken.Type} instead");
    }
}
