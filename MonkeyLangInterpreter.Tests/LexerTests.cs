using MonkeyLangInterpreter.Core;

namespace MonkeyLangInterpreter.Tests;

public class LexerTests
{
    [Fact]
    public void It_should_parse_basic_tokens()
    {
        var tokens = "=+(){},;";
        var lexer = new Lexer(tokens);

        var expectedTokens = new List<Token>
        {
            new(TokenType.ASSIGN, '='),
            new(TokenType.PLUS, '+'),
            new(TokenType.LPAREN, '('),
            new(TokenType.RPAREN, ')'),
            new(TokenType.LBRACE, '{'),
            new(TokenType.RBRACE, '}'),
            new(TokenType.COMMA, ','),
            new(TokenType.SEMICOLON, ';'),
            new(TokenType.EOF, (char)0)
        };

        foreach (var expectedToken in expectedTokens)
        {
            var token = lexer.NextToken();
            token.Type.Should().Be(expectedToken.Type);
            token.Literal.Should().Be(expectedToken.Literal);
        }
    }

    [Fact]
    public void It_should_parse_basic_monkey_code()
    {
        var input = @"let five = 5;
let ten = 10;
let add = fn(x, y) {
    x + y;
};
let result = add(five, ten);
";
        var lexer = new Lexer(input);
        var expectedTokens = new List<Token>
        {
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "five"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.INT, "5"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "ten"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.INT, "10"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "add"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.FUNCTION, "fn"),
            new(TokenType.LPAREN, "("),
            new(TokenType.IDENT, "x"),
            new(TokenType.COMMA, ","),
            new(TokenType.IDENT, "y"),
            new(TokenType.RPAREN, ")"),
            new(TokenType.LBRACE, "{"),
            new(TokenType.IDENT, "x"),
            new(TokenType.PLUS, "+"),
            new(TokenType.IDENT, "y"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.RBRACE, "}"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.LET, "let"),
            new(TokenType.IDENT, "result"),
            new(TokenType.ASSIGN, "="),
            new(TokenType.IDENT, "add"),
            new(TokenType.LPAREN, "("),
            new(TokenType.IDENT, "five"),
            new(TokenType.COMMA, ","),
            new(TokenType.IDENT, "ten"),
            new(TokenType.RPAREN, ")"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.EOF, (char)0)
        };

        foreach (var expectedToken in expectedTokens)
        {
            var token = lexer.NextToken();
            token.Type.Should().Be(expectedToken.Type);
            token.Literal.Should().Be(expectedToken.Literal);
        }
    }
}