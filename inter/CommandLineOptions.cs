using System;
public class CommandLineOptions
{
    public string ProgramPath;
    public string SettingsPath;
    public int BaseAssign = 10;
    public int BaseInput = 10;
    public int BaseOutput = 10;
    public bool DebugMode = false;

    public static CommandLineOptions Parse(string[] args)
    {
        var opts = new CommandLineOptions();
        foreach (var arg in args)
        {
            if (arg == "--debug" || arg == "-d" || arg == "/debug")
                opts.DebugMode = true;
            else if (arg.StartsWith("--base_assign="))
                opts.BaseAssign = int.Parse(arg.Substring("--base_assign=".Length));
            else if (arg.StartsWith("--base_input="))
                opts.BaseInput = int.Parse(arg.Substring("--base_input=".Length));
            else if (arg.StartsWith("--base_output="))
                opts.BaseOutput = int.Parse(arg.Substring("--base_output=".Length));
            else if (arg.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                if (opts.ProgramPath == null)
                    opts.ProgramPath = arg;
                else
                    opts.SettingsPath = arg;
            }
        }
        if (opts.ProgramPath == null)
        {
            Console.WriteLine("Не указан входной файл программы (.txt)");
            Environment.Exit(1);
        }
        if (opts.BaseAssign < 2 || opts.BaseAssign > 36 ||
            opts.BaseInput < 2 || opts.BaseInput > 36 ||
            opts.BaseOutput < 2 || opts.BaseOutput > 36)
        {
            Console.WriteLine("Основания систем счисления должны быть в диапазоне [2..36]");
            Environment.Exit(1);
        }
        return opts;
    }
}