using System.Collections.Generic;

namespace Interpreter
{
    public class InterpreterConfig
    {
        /// <summary>
        /// Направление присваивания (x = y или y = x)
        /// <summary>
        public AssignmentDirection Assignment { get; set; } = AssignmentDirection.Left;

        /// <summary>
        /// Унарный синтаксис (op(x) или (x)op)
        /// <summary>
        public UnarySyntax UnaryFormat { get; set; } = UnarySyntax.OpBrackets;

        /// <summary>
        /// Бинарный синтаксис (op(x, y), (x, y)op или x op y)
        /// <summary>
        public BinarySyntax BinaryFormat { get; set; } = BinarySyntax.OpBrackets;

        /// <summary>
        /// Синонимы команд: оригинал → псевдоним
        /// <summary>
        public Dictionary<string, string> CommandAliases { get; set; } = new();

        /// <summary>
        /// Оператор присваивания (по умолчанию "=")
        /// <summary>
        public string AssignOperator { get; set; } = "="; // дефолтно равно!

        public int BaseLiterals { get; set; } = 10;
        public int BaseInput { get; set; } = 10;
        public int BaseOutput { get; set; } = 10;

        /// <summary>
        /// Получить оригинальное имя команды по синониму
        /// <summary>
        public string GetActualCommand(string alias)
        {
            foreach (var pair in CommandAliases)
            {
                if (pair.Value == alias)
                    return pair.Key;
            }
            return alias;
        }

        /// <summary>
        /// Применение директивы из settings.txt
        /// <summary>
        public void ApplyDirective(string directive)  
        {
            directive = directive.Trim();

            switch (directive)
            {
                case "left=":
                    Assignment = AssignmentDirection.Left;
                    break;
                case "right=":
                    Assignment = AssignmentDirection.Right;
                    break;
                case "op()":
                    UnaryFormat = UnarySyntax.OpBrackets;
                    BinaryFormat = BinarySyntax.OpBrackets;
                    break;
                case "()op":
                    UnaryFormat = UnarySyntax.BracketsOp;
                    BinaryFormat = BinarySyntax.BracketsOp;
                    break;
                case "(op)":
                    BinaryFormat = BinarySyntax.Infix;
                    break;
                default:
                    if (directive.StartsWith("="))
                    {
                        var parts = directive.Split('=');
                        if (parts.Length == 2 && parts[0].Trim() == "")
                        // после разбиения получилось ровно 2 части и первая часть пустая
                        {
                            AssignOperator = parts[1].Trim(); // обработка второй части
                        }
                    }
                    break;
            }
        }
    }
}
