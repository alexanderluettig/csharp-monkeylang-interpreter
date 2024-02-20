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

        switch (_ch)
        {
            case '=':
                if (PeekChar() == '=')
                {
                    var ch = _ch;
                    ReadChar();
                    token = new Token(TokenType.EQ, $"{ch}{_ch}");
                }
                else
                {
                    token = new Token(TokenType.ASSIGN, _ch);
                }
                break;
            case '+':
                token = new Token(TokenType.PLUS, _ch);
                break;
            case '-':
                token = new Token(TokenType.MINUS, _ch);
                break;
            case '!':
                if (PeekChar() == '=')
                {
                    var ch = _ch;
                    ReadChar();
                    token = new Token(TokenType.NOT_EQ, $"{ch}{_ch}");
                }
                else
                {
                    token = new Token(TokenType.BANG, _ch);
                }
                break;
            case '*':
                token = new Token(TokenType.ASTERISK, _ch);
                break;
            case '/':
                token = new Token(TokenType.SLASH, _ch);
                break;
            case '<':
                token = new Token(TokenType.LT, _ch);
                break;
            case '>':
                token = new Token(TokenType.GT, _ch);
                break;
            case '(':
                token = new Token(TokenType.LPAREN, _ch);
                break;
            case ')':
                token = new Token(TokenType.RPAREN, _ch);
                break;
            case '{':
                token = new Token(TokenType.LBRACE, _ch);
                break;
            case '}':
                token = new Token(TokenType.RBRACE, _ch);
                break;
            case ',':
                token = new Token(TokenType.COMMA, _ch);
                break;
            case ';':
                token = new Token(TokenType.SEMICOLON, _ch);
                break;
            case (char)0:
                token = new Token(TokenType.EOF, _ch);
                break;
            case '"':
                var stringLiteral = ReadString();
                token = new Token(TokenType.STRING, stringLiteral);
                break;
            case '[':
                token = new Token(TokenType.LBRACKET, _ch);
                break;
            case ']':
                token = new Token(TokenType.RBRACKET, _ch);
                break;
            case ':':
                token = new Token(TokenType.COLON, _ch);
                break;
            default:
                if (IsLetter(_ch))
                {
                    var identifier = ReadIdentifier();
                    return new Token(Token.LookupIdent(identifier), identifier);
                }
                else if (IsDigit(_ch))
                {
                    var number = ReadNumber();
                    return new Token(TokenType.INT, number);
                }
                else
                {
                    token = new Token(TokenType.ILLEGAL, _ch);
                }
                break;
        };

        ReadChar();
        return token;
    }

    private string ReadString()
    {
        var position = _position + 1;
        while (true)
        {
            ReadChar();
            if (_ch == '"' || _ch == (char)0)
            {
                break;
            }
        }

        return _input[position.._position];
    }

    private char PeekChar()
    {
        if (_readPosition >= _input.Length)
        {
            return (char)0;
        }
        else
        {
            return _input[_readPosition];
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
