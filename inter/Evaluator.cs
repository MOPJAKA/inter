using System;
using System.Collections.Generic;

namespace Interpreter
{
    public static class Evaluator
    {
        private static readonly Dictionary<string, uint> Variables = new();
        // ключ - имя переменной, значение - числовое значение

        public static void Evaluate(Expression expr, InterpreterConfig config, int baseAssign)
        {
            switch (expr)
            {
                case Assignment a:
                    {
                        var value = EvaluateExpression(a.Value, config, baseAssign);
                        Variables[a.VariableName] = value;
                        break;
                    }
                case FunctionCall call:
                    {
                        var result = EvaluateFunction(call, config, baseAssign);
                        // Если функция — вывод, то не сохраняем значение
                        if (call.Command == "output" || call.Command == "print")
                            return;
                        break;
                    }
                default:
                    throw new Exception($"Невозможно вычислить выражение типа {expr.GetType().Name}");
            }
        }

        /// <summary>
        /// Вычисление выражения Expression 
        /// </summary>
        private static uint EvaluateExpression(Expression expr, InterpreterConfig config, int baseAssign)
        {
            return expr switch
            {
                Constant c => c.Value,
                Variable v => Variables.TryGetValue(v.Name, out var val)
                                ? val // если есть переменная
                                : throw new Exception($"Переменная '{v.Name}' не определена."),
                FunctionCall f => EvaluateFunction(f, config, baseAssign),
                _ => throw new Exception($"Неподдерживаемый тип выражения: {expr.GetType().Name}")
            };
        }

        /// <summary>
        /// Вызов операции с именем и аргументами
        /// </summary>
        private static uint EvaluateFunction(FunctionCall call, InterpreterConfig config, int baseAssign)
        {
            string command = call.Command.ToLowerInvariant();

            List<uint> args = new();
            foreach (var expr in call.Arguments)
                args.Add(EvaluateExpression(expr, config, baseAssign));

            return command switch
            {
                // Унарные
                "not" => args.Count == 1 ? ~args[0] : throw new Exception("Ожидался 1 аргумент для 'not'"),
                "input" or "in" => ReadInput(baseAssign), // читаем значение в нужной сс
                "output" or "print" => WriteOutput(args[0], baseAssign),

                // Бинарные арифметика
                "add" => CheckArgs(args, 2, () => args[0] + args[1]),
                "sub" => CheckArgs(args, 2, () => args[0] - args[1]),
                "mult" => CheckArgs(args, 2, () => args[0] * args[1]),
                "div" => CheckArgs(args, 2, () => args[1] == 0 ? throw new DivideByZeroException() : args[0] / args[1]),
                "rem" => CheckArgs(args, 2, () => args[1] == 0 ? throw new DivideByZeroException() : args[0] % args[1]),
                "pow" => CheckArgs(args, 2, () => ModPow(args[0], args[1])),

                // Логические
                "xor" => CheckArgs(args, 2, () => args[0] ^ args[1]),
                "and" => CheckArgs(args, 2, () => args[0] & args[1]),
                "or" => CheckArgs(args, 2, () => args[0] | args[1]),

                _ => throw new Exception($"Неизвестная команда: {call.Command}")
            };
        }

        private static uint CheckArgs(List<uint> args, int expectedCount, Func<uint> action)
        {
            if (args.Count != expectedCount)
                throw new Exception($"Ожидалось {expectedCount} аргументов, получено: {args.Count}");
            return action();
        }

        /// <summary>
        /// Возведение в степень по модулю
        /// </summary>
        private static uint ModPow(uint baseVal, uint exponent)
        // 1 - основание, 2 - показатель
        {
            uint result = 1;
            uint mod = 1u << 32;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1) 
                // если показатель нечётный, то умножаем результат на текущее основание
                    result = (uint)(((ulong)result * baseVal) % mod);

                baseVal = (uint)(((ulong)baseVal * baseVal) % mod);
                exponent >>= 1; // побитовый сдвиг вправо
            }

            return result;
        }

        /// <summary>
        /// Чтение
        /// </summary>
        private static uint ReadInput(int baseInput)
        {
            Console.Write($"Введите значение (основание {baseInput}): ");
            var input = Console.ReadLine()?.Trim() ?? "";
            if (!Utils.TryParseUInt32(input, baseInput, out var value))
                throw new Exception($"Невозможно распарсить ввод: {input}");
            return value;
        }

        private static uint WriteOutput(uint value, int baseOutput)
        {
            string formatted = Convert.ToString(value, baseOutput).ToUpper();
            Console.WriteLine(formatted);
            return value;
        }
    }
}
