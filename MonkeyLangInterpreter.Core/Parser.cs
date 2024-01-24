﻿using MonkeyLangInterpreter.Core.Enums;
using MonkeyLangInterpreter.Core.Interfaces;
using MonkeyLangInterpreter.Core.Nodes;

namespace MonkeyLangInterpreter.Core;

public class Parser
{
    public List<string> Errors { get; internal set; } = [];
    private Lexer Lexer { get; init; }
    private Token _currentToken;
    private Token _peekToken;
    private readonly Dictionary<TokenType, Func<IExpression>> _prefixParseFns = [];
    private readonly Dictionary<TokenType, Func<IExpression, IExpression>> _infixParseFns = [];
    private readonly Dictionary<TokenType, Precedence> _precedences = new(){
        { TokenType.EQ, Precedence.EQUALS },
        { TokenType.NOT_EQ, Precedence.EQUALS },
        { TokenType.LT, Precedence.LESSGREATER },
        { TokenType.GT, Precedence.LESSGREATER },
        { TokenType.PLUS, Precedence.SUM },
        { TokenType.MINUS, Precedence.SUM },
        { TokenType.SLASH, Precedence.PRODUCT },
        { TokenType.ASTERISK, Precedence.PRODUCT },
        { TokenType.LPAREN, Precedence.CALL },
    };

    public Parser(Lexer lexer)
    {
        Lexer = lexer;

        NextToken();
        NextToken();

        RegisterPrefix(TokenType.IDENT, () => new Identifier(_currentToken.Literal));
        RegisterPrefix(TokenType.INT, ParseIntegerLiteral);
        RegisterPrefix(TokenType.TRUE, ParseBooleanExpression);
        RegisterPrefix(TokenType.FALSE, ParseBooleanExpression);
        RegisterPrefix(TokenType.BANG, ParsePrefixExpression);
        RegisterPrefix(TokenType.MINUS, ParsePrefixExpression);

        RegisterInfix(TokenType.PLUS, ParseInfixExpression);
        RegisterInfix(TokenType.MINUS, ParseInfixExpression);
        RegisterInfix(TokenType.SLASH, ParseInfixExpression);
        RegisterInfix(TokenType.ASTERISK, ParseInfixExpression);
        RegisterInfix(TokenType.EQ, ParseInfixExpression);
        RegisterInfix(TokenType.NOT_EQ, ParseInfixExpression);
        RegisterInfix(TokenType.LT, ParseInfixExpression);
        RegisterInfix(TokenType.GT, ParseInfixExpression);
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
            TokenType.RETURN => ParseReturnStatement(),
            _ => ParseExpressionStatement(),
        };
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        var stmt = new ExpressionStatement(_currentToken, ParseExpression(Precedence.LOWEST));

        if (PeekTokenIs(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return stmt;
    }

    private IExpression ParseExpression(Precedence precedence)
    {
        _prefixParseFns.TryGetValue(_currentToken.Type, out var prefix);
        if (prefix == null)
        {
            Errors.Add($"no prefix parse function for {_currentToken.Type} found");
            return null!;
        }

        var leftExp = prefix();

        while (!PeekTokenIs(TokenType.SEMICOLON) && precedence < PeekPrecedence())
        {
            _infixParseFns.TryGetValue(_peekToken.Type, out var infix);
            if (infix == null)
            {
                return leftExp;
            }

            NextToken();

            leftExp = infix(leftExp);
        }

        return leftExp;
    }

    private ReturnStatement ParseReturnStatement()
    {
        NextToken();

        //TODO: We're skipping the expressions until we encounter a semicolon
        while (!CurrentTokenIs(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new ReturnStatement(null!);
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
        while (!CurrentTokenIs(TokenType.SEMICOLON))
        {
            NextToken();
        }

        return new LetStatement(name, null!);
    }

    private IntegerLiteral ParseIntegerLiteral()
    {
        var lit = new IntegerLiteral(_currentToken);

        if (!int.TryParse(_currentToken.Literal, out var _))
        {
            Errors.Add($"could not parse {_currentToken.Literal} as integer");
            return null!;
        }

        return lit;
    }

    private BooleanExpression ParseBooleanExpression()
    {
        return new BooleanExpression(_currentToken, CurrentTokenIs(TokenType.TRUE));
    }

    private IExpression ParsePrefixExpression()
    {
        var currentToken = _currentToken;
        NextToken();
        var expression = ParseExpression(Precedence.PREFIX);

        return new PrefixExpression(currentToken, currentToken.Literal, expression);
    }

    private IExpression ParseInfixExpression(IExpression left)
    {
        var currentToken = _currentToken;
        var precedence = CurrentPrecedence();
        NextToken();
        var right = ParseExpression(precedence);

        return new InfixExpression(currentToken, left, currentToken.Literal, right);
    }

    private void RegisterPrefix(TokenType type, Func<IExpression> fn)
    {
        _prefixParseFns[type] = fn;
    }

    private void RegisterInfix(TokenType type, Func<IExpression, IExpression> fn)
    {
        _infixParseFns[type] = fn;
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

    private Precedence PeekPrecedence()
    {
        _precedences.TryGetValue(_peekToken.Type, out var precedence);
        return precedence;
    }

    private Precedence CurrentPrecedence()
    {
        _precedences.TryGetValue(_currentToken.Type, out var precedence);
        return precedence;
    }

    private void PeekError(TokenType type)
    {
        Errors.Add($"expected next token to be {type}, got {_peekToken.Type} instead");
    }
}