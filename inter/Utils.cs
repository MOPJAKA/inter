using System;
using System.Text.RegularExpressions;

namespace Interpreter
{
    public static class Utils
    {
        /// <summary>
        /// Проверяет допустимость имени переменной.
        /// </summary>
        public static bool IsValidVariableName(string name)
        {
            return Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$");
        }

        /// <summary>
        /// Пробует безопасно распарсить строку как беззнаковое 32-битное целое.
        /// </summary>
        public static bool TryParseUInt32(string input, int numberBase, out uint result)
        {
            try
            {
                result = Convert.ToUInt32(input, numberBase);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        /// <summary>
        /// Удаляет комментарии (начиная с #) из строки.
        /// </summary>
        public static string StripComment(string line)
        {
            int index = line.IndexOf('#'); // ищем первое вхождение символа # в строке
            return index >= 0 ? line.Substring(0, index).Trim() : line.Trim();
        }
    }
}
