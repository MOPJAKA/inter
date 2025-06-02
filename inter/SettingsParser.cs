using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Interpreter
{
    public static class SettingsParser
    {
        public static InterpreterConfig ParseSettings(string path)
        {
            var config = new InterpreterConfig();
            var lines = File.ReadAllLines(path);

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) // skip comments/empty
                    continue;

                if (line == "left=")
                {
                    config.Assignment = AssignmentDirection.Left;
                }
                else if (line == "right=")
                {
                    config.Assignment = AssignmentDirection.Right;
                }
                else if (line == "op()")
                {
                    config.UnaryFormat = UnarySyntax.OpBrackets;
                    config.BinaryFormat = BinarySyntax.OpBrackets;
                }
                else if (line == "()op")
                {
                    config.UnaryFormat = UnarySyntax.BracketsOp;
                    config.BinaryFormat = BinarySyntax.BracketsOp;
                }
                else if (line == "(op)")
                {
                    config.BinaryFormat = BinarySyntax.Infix;
                }
                else if (Regex.IsMatch(line, @"^=\s*\S+$")) // например: = ->
                {
                    // Пример строки: '= ->'
                    var assignOp = line.Substring(1).Trim();
                    config.AssignOperator = assignOp;
                }
                // Новый блок для разбора оснований систем счисления
                else if (Regex.IsMatch(line, @"^base_literals\s*=\s*\d+$", RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(line, @"^base_literals\s*=\s*(\d+)$", RegexOptions.IgnoreCase);
                    config.BaseLiterals = int.Parse(match.Groups[1].Value);
                }
                else if (Regex.IsMatch(line, @"^base_input\s*=\s*\d+$", RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(line, @"^base_input\s*=\s*(\d+)$", RegexOptions.IgnoreCase);
                    config.BaseInput = int.Parse(match.Groups[1].Value);
                }
                else if (Regex.IsMatch(line, @"^base_output\s*=\s*\d+$", RegexOptions.IgnoreCase))
                {
                    var match = Regex.Match(line, @"^base_output\s*=\s*(\d+)$", RegexOptions.IgnoreCase);
                    config.BaseOutput = int.Parse(match.Groups[1].Value);
                }
                else
                {
                    var match = Regex.Match(line, @"^(\S+)\s+(\S+)$"); // проверка на соответствие
                    if (match.Success)
                    {
                        var original = match.Groups[1].Value;
                        var alias = match.Groups[2].Value;
                        config.CommandAliases[original] = alias;
                    }
                }
            }

            return config;
        }
    }
}