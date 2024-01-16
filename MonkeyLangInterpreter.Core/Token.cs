namespace MonkeyLangInterpreter.Core;

public readonly struct Token
{
    public TokenType Type { get; init; }
    public string Literal { get; init; }

    public Token(TokenType type, char literal)
    {
        Type = type;
        Literal = literal.ToString();
    }

    public Token(TokenType type, string literal)
    {
        Type = type;
        Literal = literal;
    }

    public static TokenType LookupIdent(string ident) => ident switch
    {
        "fn" => TokenType.FUNCTION,
        "let" => TokenType.LET,
        _ => TokenType.IDENT
    };
}

public enum TokenType
{
    ILLEGAL,
    EOF,

    // Identifiers + literals
    IDENT, // add, foobar, x, y, ...
    INT, // 1337

    // Operators
    ASSIGN, // "="
    PLUS, // "+"

    // Delimiters
    COMMA, // ","
    SEMICOLON, // ";"

    LPAREN, // "("
    RPAREN, // ")"
    LBRACE, // "{"
    RBRACE, // "}"

    // Keywords
    FUNCTION, // "FUNCTION"
    LET, // "LET"
}