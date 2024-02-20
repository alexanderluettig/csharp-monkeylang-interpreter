using MonkeyLangInterpreter.Core;
using MonkeyLangInterpreter.Core.Nodes;
using MonkeyLangInterpreter.Core.Objects;

namespace MonkeyLangInterpreter.Tests;

public class ObjectTests
{
    [Fact]
    public void TestStringHashKey()
    {
        var hello1 = new StringObject("Hello");
        var hello2 = new StringObject("Hello");

        var diff1 = new StringObject("World");
        var diff2 = new StringObject("World");

        hello1.HashKey().Should().Be(hello2.HashKey());
        diff1.HashKey().Should().Be(diff2.HashKey());
        hello1.HashKey().Should().NotBe(diff1.HashKey());
        diff1.HashKey().Should().NotBe(hello2.HashKey());
    }
}
