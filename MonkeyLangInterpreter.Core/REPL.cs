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
            var lexer = new Lexer(input!);

            while (true)
            {
                var token = lexer.NextToken();
                if (token.Type == TokenType.EOF)
                {
                    break;
                }

                Console.WriteLine(token);
            }
        }
    }

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, This is the Monkey programming Language!");
        Console.WriteLine("Feel free to type in commands");

        Start();
    }
}
