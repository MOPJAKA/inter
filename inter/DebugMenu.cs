using System;

public static class DebugMenu
{
    public static void Run(VariableTrie trie, Interpreter.InterpreterConfig config)
    {
        while (true)
        {
            Console.WriteLine("\n--- DEBUG MENU ---");
            Console.WriteLine("1. Показать значение переменной и дамп памяти");
            Console.WriteLine("2. Показать все переменные");
            Console.WriteLine("3. Изменить значение переменной");
            Console.WriteLine("4. Объявить переменную (цекендорф/римские цифры)");
            Console.WriteLine("5. Удалить переменную");
            Console.WriteLine("6. Продолжить выполнение");
            Console.WriteLine("7. Завершить интерпретатор");
            Console.Write("Выберите действие: ");
            var choise = Console.ReadLine();

            try
            {
                switch (choise)
                {
                    case "1":
                        Console.Write("Имя переменной: ");
                        string name = Console.ReadLine();
                        if (!trie.TryGet(name, out uint value))
                            throw new Exception("Нет такой переменной");
                        Console.WriteLine("Значение (16): " + value.ToString("X"));
                        Console.Write("Дамп (байты, осн.2): ");
                        var bytes = BitConverter.GetBytes(value);
                        foreach (var b in bytes)
                            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0') + " ");
                        Console.WriteLine();
                        break;
                    case "2":
                        foreach (var (n, v) in trie.ListAll())
                            Console.WriteLine($"{n} = {v}");
                        break;
                    case "3":
                        Console.Write("Имя переменной: ");
                        name = Console.ReadLine();
                        if (!trie.TryGet(name, out _))
                            throw new Exception("Нет такой переменной");
                        Console.Write("Новое значение (16): ");
                        string sval = Console.ReadLine();
                        uint nval = Convert.ToUInt32(sval, 16);
                        trie.Set(name, nval);
                        break;
                    case "4":
                        Console.Write("Имя переменной: ");
                        name = Console.ReadLine();
                        if (trie.TryGet(name, out _))
                            throw new Exception("Переменная уже существует");
                        Console.WriteLine("1 - цекендорф, 2 - римские:");
                        var vtype = Console.ReadLine();
                        uint val = 0;
                        if (vtype == "1")
                        {
                            Console.Write("Введите число в цекендорфовом представлении: ");
                            string s = Console.ReadLine();
                            val = ZeckendorfParser.Parse(s);
                        }
                        else if (vtype == "2")
                        {
                            Console.Write("Введите число римскими цифрами: ");
                            string s = Console.ReadLine();
                            val = RomanParser.Parse(s);
                        }
                        trie.Set(name, val);
                        break;
                    case "5":
                        Console.Write("Имя переменной: ");
                        name = Console.ReadLine();
                        if (!trie.Remove(name))
                            throw new Exception("Нет такой переменной");
                        break;
                    case "6":
                        return;
                    case "7":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Неверный выбор");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка отладчика: " + ex.Message);
            }
        }
    }
}