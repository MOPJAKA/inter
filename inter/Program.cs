using System;
using System.IO;
using Interpreter;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length < 2)
            //{
            //    Console.WriteLine("Использование: interpreter <settings.txt> <program.txt>");
            //    return;
            //}
            //
            //System.Threading.Thread.Sleep(20000);
            //
            //var settingsPath = args[0];
            //var programPath = args[1];

            var settingsPath = "settings.txt";
            var programPath = "program.txt";

            var config = SettingsParser.ParseSettings(settingsPath);
            var lines = File.ReadAllLines(programPath);

            int baseAssign = 16; 

            foreach (var line in lines)
            {
                try
                {
                    var expr = InstructionParser.Parse(line, config, baseAssign);
                    if (expr != null)
                    {
                        Console.WriteLine($"Успешно распознано выражение: {expr.GetType().Name}");
                        Evaluator.Evaluate(expr, config, baseAssign);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в строке: {line}");
                    Console.WriteLine("  " + ex.Message);
                }
            }

        }
    }
}
