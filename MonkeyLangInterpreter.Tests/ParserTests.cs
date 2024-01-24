using MonkeyLangInterpreter.Core;
using MonkeyLangInterpreter.Core.Interfaces;
using MonkeyLangInterpreter.Core.Nodes;

namespace MonkeyLangInterpreter.Tests;

public class ParserTests
{
    [Fact]
    public void TestLetStatements()
    {
        var input = @"
let x = 5;
let y = 10;
let foobar = 838383;
";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(3);

        string[] tests = ["x", "y", "foobar"];

        for (var i = 0; i < tests.Length; i++)
        {
            var statement = program.Statements[i];
            var expectedIdentifier = tests[i];

            TestLetStatement(statement, expectedIdentifier);
        }
    }

    [Fact]
    public void TestReturnStatements()
    {
        var input = @"
return 5;
return 10;
return 993322;
";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(3);

        foreach (var statement in program.Statements)
        {
            statement.Should().BeOfType<ReturnStatement>();
            statement.TokenLiteral().Should().Be("return");
        }
    }

    [Fact]
    public void TestIdentifierExpression()
    {
        var input = "foobar;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<Identifier>();

        var identifier = (Identifier)expressionStatement.Expression;
        identifier.Value.Should().Be("foobar");
        identifier.TokenLiteral().Should().Be("foobar");
    }

    [Fact]
    public void TestIntegerLiteralExpression()
    {
        var input = "5;";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<IntegerLiteral>();

        var integerLiteral = (IntegerLiteral)expressionStatement.Expression;
        integerLiteral.Value.Should().Be(5);
        integerLiteral.TokenLiteral().Should().Be("5");
    }

    [Theory]
    [InlineData("!5;", "!", 5)]
    [InlineData("-15;", "-", 15)]
    public void TestParsingPrefixExpressions(string input, string @operator, int value)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<PrefixExpression>();

        var prefixExpression = (PrefixExpression)expressionStatement.Expression;
        prefixExpression.Operator.Should().Be(@operator);
        TestIntegerLiteral(prefixExpression.Right, value);
    }

    [Theory]
    [InlineData("5 + 5;", 5, "+", 5)]
    [InlineData("5 - 5;", 5, "-", 5)]
    [InlineData("5 * 5;", 5, "*", 5)]
    [InlineData("5 / 5;", 5, "/", 5)]
    [InlineData("5 > 5;", 5, ">", 5)]
    [InlineData("5 < 5;", 5, "<", 5)]
    [InlineData("5 == 5;", 5, "==", 5)]
    [InlineData("5 != 5;", 5, "!=", 5)]
    [InlineData("true == true", true, "==", true)]
    [InlineData("true != false", true, "!=", false)]
    [InlineData("false == false", false, "==", false)]
    public void TestParsingInfixExpressions(string input, object leftValue, string @operator, object rightValue)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<InfixExpression>();
        TestInfixExpression(expressionStatement.Expression, leftValue, @operator, rightValue);
    }

    [Theory]
    [InlineData("-a * b", "((-a) * b)")]
    [InlineData("!-a", "(!(-a))")]
    [InlineData("a + b + c", "((a + b) + c)")]
    [InlineData("a + b - c", "((a + b) - c)")]
    [InlineData("a * b * c", "((a * b) * c)")]
    [InlineData("a * b / c", "((a * b) / c)")]
    [InlineData("a + b / c", "(a + (b / c))")]
    [InlineData("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)")]
    [InlineData("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)")]
    [InlineData("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))")]
    [InlineData("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))")]
    [InlineData("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))")]
    [InlineData("true", "true")]
    [InlineData("false", "false")]
    [InlineData("3 > 5 == false", "((3 > 5) == false)")]
    [InlineData("3 < 5 == true", "((3 < 5) == true)")]
    public void TestOperatorPrecedenceParsing(string input, string expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.String().Should().Be(expected);
    }

    [Theory]
    [InlineData("true;", true)]
    [InlineData("false;", false)]
    public void TestBooleanExpressions(string input, bool expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<BooleanExpression>();

        var boolean = (BooleanExpression)expressionStatement.Expression;
        boolean.Value.Should().Be(expected);
        boolean.TokenLiteral().Should().Be(expected.ToString().ToLower());
    }

    private static void TestIdentifier(IExpression expression, string value)
    {
        expression.Should().BeOfType<Identifier>();

        var identifier = (Identifier)expression;
        identifier.Value.Should().Be(value);
        identifier.TokenLiteral().Should().Be(value);
    }

    private static void TestLiteralExpression(IExpression expression, object expected)
    {
        var type = Type.GetTypeCode(expected.GetType());
        switch (type)
        {
            case TypeCode.Int32:
                TestIntegerLiteral(expression, (int)expected);
                break;
            case TypeCode.String:
                TestIdentifier(expression, (string)expected);
                break;
            case TypeCode.Boolean:
                TestBooleanExpression(expression, (bool)expected);
                break;
            default:
                throw new Exception($"type of expression not handled. got={type}");
        }
    }

    private static void TestBooleanExpression(IExpression expression, bool value)
    {
        expression.Should().BeOfType<BooleanExpression>();

        var boolean = (BooleanExpression)expression;
        boolean.Value.Should().Be(value);
        boolean.TokenLiteral().Should().Be(value.ToString().ToLower());
    }

    private static void TestInfixExpression(IExpression expression, object left, string @operator, object right)
    {
        expression.Should().BeOfType<InfixExpression>();

        var infixExpression = (InfixExpression)expression;
        TestLiteralExpression(infixExpression.Left, left);
        infixExpression.Operator.Should().Be(@operator);
        TestLiteralExpression(infixExpression.Right, right);
    }

    private static void TestIntegerLiteral(IExpression expression, int value)
    {
        expression.Should().BeOfType<IntegerLiteral>();

        var integerLiteral = (IntegerLiteral)expression;
        integerLiteral.Value.Should().Be(value);
        integerLiteral.TokenLiteral().Should().Be(value.ToString());
    }

    private static void TestLetStatement(IStatement statement, string name)
    {
        statement.Should().BeOfType<LetStatement>();
        statement.TokenLiteral().Should().Be("let");

        var letStatement = (LetStatement)statement;
        letStatement.Name.Value.Should().Be(name);
        letStatement.Name.TokenLiteral().Should().Be(name);
    }
}
