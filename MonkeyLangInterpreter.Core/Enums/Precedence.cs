namespace MonkeyLangInterpreter.Core.Enums;

public enum Precedence
{
    LOWEST,
    EQUALS, // ==
    LESSGREATER, // > or <
    SUM, // +
    PRODUCT, // *
    PREFIX, // -X or !X
    CALL, // myFunction(X)
    INDEX // array[index]
}
