using System;
using System.IO;
using Interpreter;

class Program
{
    static void Main(string[] args)
    {
        var options = CommandLineOptions.Parse(args);

        var settingsPath = options.SettingsPath ?? SettingsPersistence.LoadLastSettings();
        if (settingsPath == null) settingsPath = "settings.txt";
        SettingsPersistence.SaveLastSettings(settingsPath);

        var config = SettingsParser.ParseSettings(settingsPath);
        config.BaseLiterals = options.BaseAssign;
        config.BaseInput = options.BaseInput;
        config.BaseOutput = options.BaseOutput;

        string programText = File.ReadAllText(options.ProgramPath);
        var instructions = InstructionSplitter.Split(programText);

        var trie = new VariableTrie();
        foreach (var instr in instructions)
        {
            try
            {
                if (DebugUtils.ContainsBreakpoint(instr) && options.DebugMode)
                {
                    DebugMenu.Run(trie, config);
                    continue;
                }
                var expr = InstructionParser.Parse(instr, config);
                if (expr != null)
                    Evaluator.Evaluate(expr, config, trie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в инструкции: {instr}");
                Console.WriteLine("  " + ex.Message);
            }
        }
    }
}