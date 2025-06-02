using Interpreter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Interpreter
{
    public static class InstructionParser
    {
        public static Expression Parse(string line, InterpreterConfig config)
        {
            line = Utils.StripComment(line);

            if (string.IsNullOrWhiteSpace(line))
                return null;

            // Попробуем распознать как команду (print(...))
            var commandMatch = Regex.Match(line, @"^(\w+)\((.*)\)$");
            if (commandMatch.Success)
            {
                var cmd = commandMatch.Groups[1].Value;
                var args = commandMatch.Groups[2].Value;

                var actual = config.GetActualCommand(cmd);
                var parsedArgs = SplitArgumentsRespectingNesting(args)
                    .ConvertAll(arg => ParseExpression(arg, config));
                // ConvertAll применяет указанную функцию ко всем элементам списка

                return new FunctionCall(actual, parsedArgs);
            }

            return config.Assignment switch
            {
                AssignmentDirection.Left => ParseLeftAssignment(line, config),
                AssignmentDirection.Right => ParseRightAssignment(line, config),
                _ => throw new Exception("Неизвестный стиль присваивания.")
            };
        }


        /// <summary>
        /// Разбор выражения присваивания в классическом стиле x = ... 
        /// </summary>
        private static Expression ParseLeftAssignment(string line, InterpreterConfig config)
        {
            int eqIndex = FindTopLevelAssignmentOperator(line, config.AssignOperator);

            if (eqIndex == -1) throw new Exception("Неверный формат присваивания.");

            string varName = line.Substring(0, eqIndex).Trim(); // извлечение имени переменной (до оператора всё)
            string exprPart = line.Substring(eqIndex + config.AssignOperator.Length).Trim(); // выражение, которое будет вычислено и присвоено переменной

            if (!Utils.IsValidVariableName(varName))
                throw new Exception($"Недопустимое имя переменной: {varName}");

            return new Assignment(varName, ParseExpression(exprPart, config));
        }

        /// <summary>
        /// Разбор выражения присваивания в стиле обратного присваивания ... = х
        /// </summary>
        private static Expression ParseRightAssignment(string line, InterpreterConfig config)
        {
            int eqIndex = FindTopLevelAssignmentOperator(line, config.AssignOperator);

            if (eqIndex == -1) throw new Exception("Неверный формат присваивания.");

            string exprPart = line.Substring(0, eqIndex).Trim(); // выражение, которое будет вычислено и присвоено переменной
            string varName = line.Substring(eqIndex + config.AssignOperator.Length).Trim(); // извлечение имени переменной (после оператора всё)

            if (!Utils.IsValidVariableName(varName))
                throw new Exception($"Недопустимое имя переменной: {varName}");

            return new Assignment(varName, ParseExpression(exprPart, config));
        }

        /// <summary>
        /// Ищет позицию верхнеуровневого знака '=' (не внутри скобок)
        /// </summary>
        private static int FindTopLevelAssignmentOperator(string line, string assignOp)
        // assignOp - =
        {
            int depth = 0;
            for (int i = 0; i <= line.Length - assignOp.Length; i++)
            {
                if (line[i] == '(') depth++;
                else if (line[i] == ')') depth--;
                else if (depth == 0 && line.Substring(i, assignOp.Length) == assignOp)
                    // если на текущей позиции depth == 0 и подстрока совпадает с assignOp 
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Парсит каждый аргумент как выражение
        /// </summary>
        private static Expression ParseExpression(string expr, InterpreterConfig config)
        {
            expr = expr.Trim();

            // Переменная
            if (Utils.IsValidVariableName(expr))
            {
                return new Variable(expr);
            }

            // Литерал (константа)
            if (Regex.IsMatch(expr, @"^[0-9A-Fa-f]+$"))
            {
                if (Utils.TryParseUInt32(expr, config.BaseLiterals, out var val))
                    return new Constant(val);

                throw new Exception($"Невозможно распарсить число: {expr}");
            }

            // Унарная или бинарная операция
            return ParseFunction(expr, config);
        }

        /// <summary>
        /// Разделяет аргументы, учитывая вложенные скобки (разделяет по запятым)
        /// </summary>
        private static List<string> SplitArgumentsRespectingNesting(string argsString)
        {
            var args = new List<string>();
            int depth = 0;
            var current = new System.Text.StringBuilder();  // итог
                                                            // уровень вложенности по скобкам
            for (int i = 0; i < argsString.Length; i++)
            {
                char ch = argsString[i];

                if (ch == ',' && depth == 0)
                {                                         // если символ — запятая, и мы не внутри скобок (depth == 0), то это разделитель
                    // Добавляем аргумент, если не пустой
                    var arg = current.ToString().Trim();
                    if (!string.IsNullOrEmpty(arg))
                        args.Add(arg);
                    current.Clear();
                }
                else
                {
                    if (ch == '(')                        // добавляем символ в текущий аргумент
                        depth++;
                    else if (ch == ')')
                        depth--;
                    current.Append(ch);                   // для последнего аргумента
                }
            }

            var lastArg = current.ToString().Trim();
            if (!string.IsNullOrEmpty(lastArg))
                args.Add(lastArg);

            return args;
        }

        /// <summary>
        /// Разбор строкового выражения, представляющего операцию
        /// </summary>
        private static Expression ParseFunction(string expr, InterpreterConfig config)
        {
            expr = expr.Trim();

            if (config.BinaryFormat == BinarySyntax.OpBrackets || config.UnaryFormat == UnarySyntax.OpBrackets)
            {
                var funcMatch = Regex.Match(expr, @"^([^(]+)\((.+)\)$");
                if (funcMatch.Success)
                {
                    var command = funcMatch.Groups[1].Value;
                    var args = SplitArgumentsRespectingNesting(funcMatch.Groups[2].Value);

                    var actualCmd = config.GetActualCommand(command);

                    var parsedArgs = new List<Expression>();
                    foreach (var arg in args)
                        parsedArgs.Add(ParseExpression(arg.Trim(), config));
                    // каждый аргумент из строки преобразуется в объект Expression с помощью ParseExpression

                    return new FunctionCall(actualCmd, parsedArgs);
                    // объект с полями: имя операции и список аргументов
                }
            }

            if (config.BinaryFormat == BinarySyntax.Infix)
            {
                // (arg1 op arg2)
                var match = Regex.Match(expr, @"^(\w+)\s+(\S+)\s+(\w+)$");
                if (match.Success)
                {
                    var arg1 = match.Groups[1].Value;
                    var op = match.Groups[2].Value;
                    var arg2 = match.Groups[3].Value;

                    var actualCmd = config.GetActualCommand(op);

                    return new FunctionCall(actualCmd, new List<Expression>
                    {
                        ParseExpression(arg1, config),
                        ParseExpression(arg2, config)
                    });
                }
            }

            throw new Exception($"Невозможно разобрать выражение: {expr}");
        }
    }
}