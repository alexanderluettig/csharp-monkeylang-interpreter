using MonkeyLangInterpreter.Core;

namespace MonkeyLangInterpreter.Tests;

public class LexerTests
{
    [Theory]
    [InlineData(TokenType.EOF, (char)0)]
    [InlineData(TokenType.ASSIGN, '=')]
    [InlineData(TokenType.PLUS, '+')]
    [InlineData(TokenType.COMMA, ',')]
    [InlineData(TokenType.SEMICOLON, ';')]
    [InlineData(TokenType.LPAREN, '(')]
    [InlineData(TokenType.RPAREN, ')')]
    [InlineData(TokenType.LBRACE, '{')]
    [InlineData(TokenType.RBRACE, '}')]
    public void It_should_parse_basic_tokens(TokenType type, char expectedLiteral)
    {
        var lexer = new Lexer(expectedLiteral.ToString());
        var token = lexer.NextToken();
        token.Type.Should().Be(type);
        token.Literal.Should().Be(expectedLiteral.ToString());
    }

    [Fact]
    public void It_should_parse_basic_monkey_code()
    {
        var input = @"let five = 5;
let ten = 10;
let add = fn(x, y) {
    x + y;
};
let result = add(five, ten);";
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

    [Fact]
    public void It_should_parse_more_monkey_code()
    {
        var input = @"!-/*5
        5 < 10 > 5";

        var lexer = new Lexer(input);
        var expectedTokens = new List<Token>
        {
            new(TokenType.BANG, "!"),
            new(TokenType.MINUS, "-"),
            new(TokenType.SLASH, "/"),
            new(TokenType.ASTERISK, "*"),
            new(TokenType.INT, "5"),
            new(TokenType.INT, "5"),
            new(TokenType.LT, "<"),
            new(TokenType.INT, "10"),
            new(TokenType.GT, ">"),
            new(TokenType.INT, "5"),
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
    public void It_should_parse_more_monkey_keywords()
    {
        var input = @"if (5 < 10) {
            return true;
        } else {
            return false;
        }";

        var lexer = new Lexer(input);
        var expectedTokens = new List<Token>
        {
            new(TokenType.IF, "if"),
            new(TokenType.LPAREN, "("),
            new(TokenType.INT, "5"),
            new(TokenType.LT, "<"),
            new(TokenType.INT, "10"),
            new(TokenType.RPAREN, ")"),
            new(TokenType.LBRACE, "{"),
            new(TokenType.RETURN, "return"),
            new(TokenType.TRUE, "true"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.RBRACE, "}"),
            new(TokenType.ELSE, "else"),
            new(TokenType.LBRACE, "{"),
            new(TokenType.RETURN, "return"),
            new(TokenType.FALSE, "false"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.RBRACE, "}"),
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
    public void It_should_parse_2_character_tokens()
    {
        var input = @"10 == 10;
        10 != 9;";

        var lexer = new Lexer(input);
        var expectedTokens = new List<Token>
        {
            new(TokenType.INT, "10"),
            new(TokenType.EQ, "=="),
            new(TokenType.INT, "10"),
            new(TokenType.SEMICOLON, ";"),
            new(TokenType.INT, "10"),
            new(TokenType.NOT_EQ, "!="),
            new(TokenType.INT, "9"),
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

    [Fact]
    public void It_should_parse_strings()
    {
        var input = @"""foobar"";";

        var lexer = new Lexer(input);
        var expectedTokens = new List<Token>
        {
            new(TokenType.STRING, "foobar"),
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