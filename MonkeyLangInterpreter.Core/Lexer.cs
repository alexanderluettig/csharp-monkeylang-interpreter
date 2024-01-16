





namespace MonkeyLangInterpreter.Core;

public class Lexer
{
    private readonly string _input;
    private int _position;
    private int _readPosition;
    private char _ch;

    public Lexer(string input)
    {
        _input = input;
        ReadChar();
    }

    public Token NextToken()
    {
        Token token;

        SkipWhitespace();

        token = _ch switch
        {
            '=' => new Token(TokenType.ASSIGN, _ch),
            '+' => new Token(TokenType.PLUS, _ch),
            '(' => new Token(TokenType.LPAREN, _ch),
            ')' => new Token(TokenType.RPAREN, _ch),
            '{' => new Token(TokenType.LBRACE, _ch),
            '}' => new Token(TokenType.RBRACE, _ch),
            ',' => new Token(TokenType.COMMA, _ch),
            ';' => new Token(TokenType.SEMICOLON, _ch),
            (char)0 => new Token(TokenType.EOF, _ch),
            _ => CheckForIdentifierOrKeyword()
        };

        ReadChar();
        return token;
    }

    private Token CheckForIdentifierOrKeyword()
    {
        if (IsLetter(_ch))
        {
            var identifier = ReadIdentifier();
            _position--;
            _readPosition--;
            return new Token(Token.LookupIdent(identifier), identifier);
        }
        else if (IsDigit(_ch))
        {
            var number = ReadNumber();
            _position--;
            _readPosition--;
            return new Token(TokenType.INT, number);
        }
        else
        {
            return new Token(TokenType.ILLEGAL, _ch);
        }
    }

    private string ReadNumber()
    {
        var position = _position;
        while (IsDigit(_ch))
        {
            ReadChar();
        }

        return _input[position.._position];
    }

    private string ReadIdentifier()
    {
        var position = _position;
        while (IsLetter(_ch))
        {
            ReadChar();
        }

        return _input[position.._position];
    }

    private void ReadChar()
    {
        if (_readPosition >= _input.Length)
        {
            _ch = (char)0;
        }
        else
        {
            _ch = _input[_readPosition];
        }

        _position = _readPosition;
        _readPosition++;
    }

    private void SkipWhitespace()
    {
        while (_ch is ' ' or '\t' or '\n' or '\r')
        {
            ReadChar();
        }
    }

    private static bool IsLetter(char ch)
    {
        return char.IsLetter(ch) || ch == '_';
    }

    private static bool IsDigit(char ch)
    {
        return char.IsDigit(ch);
    }
}
