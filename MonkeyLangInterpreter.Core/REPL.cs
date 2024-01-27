namespace MonkeyLangInterpreter.Core;

public class REPL
{
    private const string PROMPT = ">> ";

    public static void Start()
    {
        while (true)
        {
            Console.Write(PROMPT);
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            var lexer = new Lexer(input!);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();

            if (parser.Errors.Count != 0)
            {
                parser.Errors.ForEach(Console.WriteLine);
                continue;
            }

            Console.WriteLine(program.String());
        }
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, This is the Monkey programming Language!");
        Console.WriteLine("Feel free to type in commands");

        Start();
    }
}
