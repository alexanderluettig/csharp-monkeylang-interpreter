using MonkeyLangInterpreter.Core.Interfaces;

namespace MonkeyLangInterpreter.Core.Nodes;

public class Program : INode
{
    public List<IStatement> Statements { get; set; } = [];
    public string TokenLiteral()
    {
        if (Statements.Count > 0)
        {
            return Statements[0].TokenLiteral();
        }
        else
        {
            return "";
        }
    }
}
