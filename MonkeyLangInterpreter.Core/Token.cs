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
        "if" => TokenType.IF,
        "else" => TokenType.ELSE,
        "return" => TokenType.RETURN,
        "true" => TokenType.TRUE,
        "false" => TokenType.FALSE,
        _ => TokenType.IDENT
    };

    public override string ToString() => $"Type: {Type}, Literal: {Literal}";
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
    MINUS, // "-"
    BANG, // "!"
    ASTERISK, // "*"
    SLASH, // "/"
    LT, // "<"
    GT, // ">"
    EQ, // "=="
    NOT_EQ, // "!="

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
    IF, // "IF"
    ELSE, // "ELSE"
    RETURN, // "RETURN"
    TRUE, // "TRUE"
    FALSE, // "FALSE"
}