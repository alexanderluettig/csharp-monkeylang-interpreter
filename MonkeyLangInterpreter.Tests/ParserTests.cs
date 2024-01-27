using MonkeyLangInterpreter.Core;
using MonkeyLangInterpreter.Core.Interfaces;
using MonkeyLangInterpreter.Core.Nodes;

namespace MonkeyLangInterpreter.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("let x = 5;", "x", 5)]
    [InlineData("let y = true;", "y", true)]
    [InlineData("let foobar = y;", "foobar", "y")]
    public void TestLetStatements(string input, string identifier, object expectedValue)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        TestLetStatement(statement, identifier);

        var letStatement = (LetStatement)statement;
        TestLiteralExpression(letStatement.Value, expectedValue);
    }

    [Theory]
    [InlineData("return 5;", 5)]
    [InlineData("return true;", true)]
    [InlineData("return foobar;", "foobar")]
    public void TestReturnStatements(string input, object expected)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();

        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ReturnStatement>();
        statement.TokenLiteral().Should().Be("return");

        var returnStatement = (ReturnStatement)statement;
        TestLiteralExpression(returnStatement.ReturnValue, expected);
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
    [InlineData("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)")]
    [InlineData("(5 + 5) * 2", "((5 + 5) * 2)")]
    [InlineData("2 / (5 + 5)", "(2 / (5 + 5))")]
    [InlineData("-(5 + 5)", "(-(5 + 5))")]
    [InlineData("!(true == true)", "(!(true == true))")]
    [InlineData("a + add(b * c) + d", "((a + add((b * c))) + d)")]
    [InlineData("add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))")]
    [InlineData("add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))")]
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

    [Fact]
    public void TestIfExpression()
    {
        var input = "if (x < y) { x }";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();
        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<IfExpression>();

        var ifExpression = (IfExpression)expressionStatement.Expression;
        TestInfixExpression(ifExpression.Condition, "x", "<", "y");
        ifExpression.Consequence.Statements.Should().HaveCount(1);

        var consequence = ifExpression.Consequence.Statements[0];
        consequence.Should().BeOfType<ExpressionStatement>();

        var consequenceExpressionStatement = (ExpressionStatement)consequence;
        TestIdentifier(consequenceExpressionStatement.Expression, "x");

        ifExpression.Alternative.Should().BeNull();
    }

    [Fact]
    public void TestIfElseExpression()
    {
        var input = "if (x < y) { x } else { y }";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();
        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<IfExpression>();

        var ifExpression = (IfExpression)expressionStatement.Expression;
        TestInfixExpression(ifExpression.Condition, "x", "<", "y");
        ifExpression.Consequence.Statements.Should().HaveCount(1);

        var consequence = ifExpression.Consequence.Statements[0];
        consequence.Should().BeOfType<ExpressionStatement>();

        var consequenceExpressionStatement = (ExpressionStatement)consequence;
        TestIdentifier(consequenceExpressionStatement.Expression, "x");

        ifExpression.Alternative.Statements.Should().HaveCount(1);

        var alternative = ifExpression.Alternative.Statements[0];
        alternative.Should().BeOfType<ExpressionStatement>();

        var alternativeExpressionStatement = (ExpressionStatement)alternative;
        TestIdentifier(alternativeExpressionStatement.Expression, "y");
    }

    [Fact]
    public void TestFunctionLiteralParsing()
    {
        var input = "fn(x, y) { x + y; }";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();
        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<FunctionLiteral>();

        var function = (FunctionLiteral)expressionStatement.Expression;
        function.Parameters.Should().HaveCount(2);

        TestLiteralExpression(function.Parameters[0], "x");
        TestLiteralExpression(function.Parameters[1], "y");

        function.Body.Statements.Should().HaveCount(1);

        var bodyStatement = function.Body.Statements[0];
        bodyStatement.Should().BeOfType<ExpressionStatement>();

        var bodyExpressionStatement = (ExpressionStatement)bodyStatement;
        TestInfixExpression(bodyExpressionStatement.Expression, "x", "+", "y");
    }

    [Theory]
    [InlineData("fn() {};", new string[0])]
    [InlineData("fn(x) {};", new[] { "x" })]
    [InlineData("fn(x, y, z) {};", new[] { "x", "y", "z" })]
    public void TestFunctionParameterParsing(string input, string[] expectedParams)
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
        expressionStatement.Expression.Should().BeOfType<FunctionLiteral>();

        var function = (FunctionLiteral)expressionStatement.Expression;
        function.Parameters.Should().HaveCount(expectedParams.Length);

        for (var i = 0; i < expectedParams.Length; i++)
        {
            TestLiteralExpression(function.Parameters[i], expectedParams[i]);
        }
    }

    [Fact]
    public void TestCallExpressionParsing()
    {
        var input = "add(1, 2 * 3, 4 + 5);";

        var lexer = new Lexer(input);
        var parser = new Parser(lexer);

        var program = parser.ParseProgram();
        parser.Errors.Should().BeEmpty();
        program.Should().NotBeNull();
        program.Statements.Should().HaveCount(1);

        var statement = program.Statements[0];
        statement.Should().BeOfType<ExpressionStatement>();

        var expressionStatement = (ExpressionStatement)statement;
        expressionStatement.Expression.Should().BeOfType<CallExpression>();

        var callExpression = (CallExpression)expressionStatement.Expression;
        TestIdentifier(callExpression.Function, "add");
        callExpression.Arguments.Should().HaveCount(3);
        TestLiteralExpression(callExpression.Arguments[0], 1);
        TestInfixExpression(callExpression.Arguments[1], 2, "*", 3);
        TestInfixExpression(callExpression.Arguments[2], 4, "+", 5);
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
