using System;
using System.Collections.Generic;

public static class RomanParser
{
    static readonly Dictionary<char, int> values = new()
    {
        {'I',1 },{'V',5 },{'X',10 },{'L',50 },{'C',100 },{'D',500 },{'M',1000 }
    };

    public static uint Parse(string s)
    {
        s = s.ToUpper().Trim();
        int sum = 0, prev = 0;
        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (!values.ContainsKey(s[i])) throw new Exception("Недопустимый символ в римских цифрах");
            int val = values[s[i]];
            if (val < prev) sum -= val;
            else sum += val;
            prev = val;
        }
        return (uint)sum;
    }
}