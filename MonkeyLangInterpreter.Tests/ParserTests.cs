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

    private static void TestLetStatement(IStatement statement, string name)
    {
        statement.Should().BeOfType<LetStatement>();
        statement.TokenLiteral().Should().Be("let");

        var letStatement = (LetStatement)statement;
        letStatement.Name.Value.Should().Be(name);
        letStatement.Name.TokenLiteral().Should().Be(name);
    }
}
