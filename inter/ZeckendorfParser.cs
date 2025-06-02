using System;

public static class ZeckendorfParser
{
    // Ожидает строку из 0 и 1 (например "1001000"), где 1 на позиции i означает, что в числе есть F(i+2)
    // F - числа фибоначчи: F(1)=1, F(2)=2, F(3)=3, F(4)=5, F(5)=8, ...
    public static uint Parse(string s)
    {
        s = s.Trim().Replace(" ", "");
        if (string.IsNullOrEmpty(s)) throw new Exception("Пустое цекендорфово представление");
        uint sum = 0;
        var fibs = GetFibs(s.Length + 2);
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '1')
                sum += fibs[s.Length - i]; // F(i+2), справа налево
            else if (s[i] != '0')
                throw new Exception("Недопустимый символ в цекендорфовом представлении");
        }
        return sum;
    }

    // Возвращает массив Фибоначчи с 0 по n включительно
    private static uint[] GetFibs(int n)
    {
        var f = new uint[n + 1];
        f[0] = 0; f[1] = 1;
        for (int i = 2; i <= n; i++)
            f[i] = f[i - 1] + f[i - 2];
        return f;
    }
}