using MonkeyLangInterpreter.Core.Nodes;

namespace MonkeyLangInterpreter.Tests;

public class AstTests
{
    [Fact]
    public void TestString()
    {
        var program = new Program();
        program.Statements.Add(new LetStatement(new Identifier("myVar"), new Identifier("anotherVar")));

        program.String().Should().Be("let myVar = anotherVar;");
    }
}
