using MonkeyLangInterpreter.Core;
using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Tests;

public class EvaluatorTests
{
    [Theory]
    [InlineData("5", 5)]
    [InlineData("10", 10)]
    [InlineData("-5", -5)]
    [InlineData("-10", -10)]
    [InlineData("5 + 5 + 5 + 5 - 10", 10)]
    [InlineData("2 * 2 * 2 * 2 * 2", 32)]
    [InlineData("-50 + 100 + -50", 0)]
    [InlineData("5 * 2 + 10", 20)]
    [InlineData("5 + 2 * 10", 25)]
    [InlineData("20 + 2 * -10", 0)]
    [InlineData("50 / 2 * 2 + 10", 60)]
    [InlineData("2 * (5 + 10)", 30)]
    [InlineData("3 * 3 * 3 + 10", 37)]
    [InlineData("3 * (3 * 3) + 10", 37)]
    [InlineData("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50)]
    public void TestEvalIntegerExpression(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("1 < 2", true)]
    [InlineData("1 > 2", false)]
    [InlineData("1 < 1", false)]
    [InlineData("1 > 1", false)]
    [InlineData("1 == 1", true)]
    [InlineData("1 != 1", false)]
    [InlineData("1 == 2", false)]
    [InlineData("1 != 2", true)]
    [InlineData("true == true", true)]
    [InlineData("false == false", true)]
    [InlineData("true == false", false)]
    [InlineData("true != false", true)]
    [InlineData("false != true", true)]
    [InlineData("(1 < 2) == true", true)]
    [InlineData("(1 < 2) == false", false)]
    [InlineData("(1 > 2) == true", false)]
    [InlineData("(1 > 2) == false", true)]
    public void TestEvalBooleanExpression(string input, bool expected)
    {
        var evaluated = TestEval(input);
        TestBooleanObject(evaluated, expected);
    }

    [Theory]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("!5", false)]
    [InlineData("!!true", true)]
    [InlineData("!!false", false)]
    [InlineData("!!5", true)]
    public void TestBangOperator(string input, bool expected)
    {
        var evaluated = TestEval(input);
        TestBooleanObject(evaluated, expected);
    }

    [Theory]
    [InlineData("if (true) { 10 }", 10)]
    [InlineData("if (false) { 10 }", null)]
    [InlineData("if (1) { 10 }", 10)]
    [InlineData("if (1 < 2) { 10 }", 10)]
    [InlineData("if (1 > 2) { 10 }", null)]
    [InlineData("if (1 > 2) { 10 } else { 20 }", 20)]
    [InlineData("if (1 < 2) { 10 } else { 20 }", 10)]
    public void TestIfElseExpressions(string input, int? expected)
    {
        var evaluated = TestEval(input);

        if (expected is null)
        {
            evaluated.Should().BeOfType<NullObject>();
        }
        else
        {
            TestIntegerObject(evaluated, expected.Value);
        }
    }

    [Theory]
    [InlineData("return 10;", 10)]
    [InlineData("return 10; 9;", 10)]
    [InlineData("return 2 * 5; 9;", 10)]
    [InlineData("9; return 2 * 5; 9;", 10)]
    public void TestReturnStatements(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    [Theory]
    [InlineData("5 + true;", "type mismatch: INTEGER + BOOLEAN")]
    [InlineData("5 + true; 5;", "type mismatch: INTEGER + BOOLEAN")]
    [InlineData("-true", "unknown operator: -BOOLEAN")]
    [InlineData("true + false;", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("5; true + false; 5", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("if (10 > 1) { true + false; }", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("if (10 > 1) { if (10 > 1) { return true + false; } return 1; }", "unknown operator: BOOLEAN + BOOLEAN")]
    [InlineData("foobar", "identifier not found: foobar")]
    public void TestErrorHandling(string input, string expectedMessage)
    {
        var evaluated = TestEval(input);
        evaluated.Should().BeOfType<ErrorObject>();
        var error = (ErrorObject)evaluated;
        error.Message.Should().Be(expectedMessage);
    }

    [Theory]
    [InlineData("let a = 5; a;", 5)]
    [InlineData("let a = 5 * 5; a;", 25)]
    [InlineData("let a = 5; let b = a; b;", 5)]
    [InlineData("let a = 5; let b = a; let c = a + b + 5; c;", 15)]
    public void TestLetStatements(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    [Theory]
    [InlineData("fn(x) { x + 2; };", "x", "(x + 2)")]
    public void TestFunctionObject(string input, string expectedParameter, string expectedBody)
    {
        var evaluated = TestEval(input);
        evaluated.Should().BeOfType<Function>();
        var function = (Function)evaluated;
        function.Parameters.Should().HaveCount(1);
        function.Parameters[0].String().Should().Be(expectedParameter);
        function.Body.String().Should().Be(expectedBody);
    }

    [Theory]
    [InlineData("let identity = fn(x) { x; }; identity(5);", 5)]
    [InlineData("let identity = fn(x) { return x; }; identity(5);", 5)]
    [InlineData("let double = fn(x) { x * 2; }; double(5);", 10)]
    [InlineData("let add = fn(x, y) { x + y; }; add(5, 5);", 10)]
    [InlineData("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20)]
    public void TestFunctionApplication(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    [Theory]
    [InlineData("let newAdder = fn(x) { fn(y) { x + y }; }; let addTwo = newAdder(2); addTwo(2);", 4)]
    public void TestClosures(string input, int expected)
    {
        var evaluated = TestEval(input);
        TestIntegerObject(evaluated, expected);
    }

    private static IObject TestEval(string input)
    {
        var lexer = new Lexer(input);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        var env = new VariableEnvironment();

        return Evaluator.Eval(program, env);
    }

    private static void TestIntegerObject(IObject obj, int expected)
    {
        obj.Should().BeOfType<IntegerObject>();
        var result = (IntegerObject)obj;
        result.Value.Should().Be(expected);
    }

    private static void TestBooleanObject(IObject obj, bool expected)
    {
        obj.Should().BeOfType<BooleanObject>();
        var result = (BooleanObject)obj;
        result.Value.Should().Be(expected);
    }
}
