namespace DispatchCalcVR
{
    /// <summary>
    /// Command-dispatch menu: parses user input, supports
    /// commands, bare numbers, simple infix, and help.
    /// </summary>
    public class Menu
    {
        private readonly Calc calc = new Calc();
        private readonly FileManager fileManager = new FileManager();
        //Dispatch table
        private readonly Dictionary<string, Action<string[]>> commands;

        public Menu()
        {
            //Populates dispatch table
            commands = new(StringComparer.OrdinalIgnoreCase)
            {
                { "add",       args => calc.Add(ParseToken(args[0])) },
                { "sub",       args => calc.Subtract(ParseToken(args[0])) },
                { "mul",       args => calc.Multiply(ParseToken(args[0])) },
                { "div",       args => calc.Divide(ParseToken(args[0])) },
                { "mod",       args => calc.Mod(ParseToken(args[0])) },
                { "pow",       args => calc.Power(ParseToken(args[0])) },
                { "fib",       args => calc.Fibonacci() },
                { "clear",     args => calc.Clear() },
                { "undo",      args => { if (!calc.Undo()) Console.WriteLine("Nothing to undo."); } },
                { "save",      args => calc.SaveVariable(args[0]) },
                { "load",      args => calc.LoadVariable(args[0]) },
                { "delete",    args => calc.DeleteVariable(args[0]) },
                { "vars",      args => ListVars() },
                { "savefile",  args => fileManager.SaveVariables(args.Length>0? args[0]:"vars.txt", calc) },
                { "loadfile",  args => fileManager.LoadVariables(args.Length>0? args[0]:"vars.txt", calc) },
                { "help",      args => PrintHelp() },
                { "exit",      args => Environment.Exit(0) }
            };
        }

        /// <summary>
        /// Main loop: reads input, handles bare number, or commands.
        /// </summary>
        public void StartLoop()
        {
            Console.WriteLine("Welcome to Terminalator! A calculator in a terminal (type 'help' for commands)");
            while (true)
            {
                Console.Write($"ans = {calc.Ans} > ");
                var line = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);


                if (parts.Length == 1 && double.TryParse(parts[0], out var literal))
                {
                    calc.Clear();
                    calc.Add(literal);
                    Console.WriteLine($"=> {calc.Ans}");
                    continue;
                }

                if (parts.Length == 3 && IsOperator(parts[1]))
                {
                    var left = ParseToken(parts[0]);
                    var right = ParseToken(parts[2]);
                    var result = parts[1] switch
                    {
                        "+" => left + right,
                        "-" => left - right,
                        "*" => left * right,
                        "/" => left / right,
                        "%" => left % right,
                        _ => throw new ArgumentException("Unsupported operator")
                    };
                    calc.Clear();
                    calc.Add(result);
                    Console.WriteLine($"=> {calc.Ans}");
                    continue;
                }

                var cmd = parts[0];
                var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

                if (commands.TryGetValue(cmd, out var action))
                {
                    try
                    {
                        action(args);
                        Console.WriteLine($"=> {calc.Ans}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown command '{cmd}'. Type 'help'.");
                }
            }
        }

        private double ParseToken(string token)
        {
            if (double.TryParse(token, out var num)) 
                return num;
            return calc.GetVariable(token);
        }

        private static bool IsOperator(string op) => "+-*/%".Contains(op);

        private void ListVars()
        {
            Console.WriteLine("Saved variables:");
            foreach (var name in calc.ListVariables())
                Console.WriteLine($"{name} = {calc.GetVariable(name)}");
        }

        private void PrintHelp()
        {
            Console.WriteLine(@"Commands:
                    <number>            set ans to number
                    X op Y              compute simple expression (+,-,*,/,%)
                    add N, sub N, mul N, div N, mod N, pow N
                    fib, clear, undo
                    save NAME, load NAME, delete NAME, vars
                    savefile FILE, loadfile FILE
                    help, exit"
                    );
        }
    }


}




