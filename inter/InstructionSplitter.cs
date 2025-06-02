using System.Collections.Generic;
using System.Text;

public static class InstructionSplitter
{
    public static List<string> Split(string programText)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        int i = 0;
        int len = programText.Length;
        int multiCommentDepth = 0;

        while (i < len)
        {
            // Однострочный комментарий
            if (multiCommentDepth == 0 && i < len && programText[i] == '#')
            {
                while (i < len && programText[i] != '\n') i++;
                continue;
            }
            // Многострочный комментарий с вложенностью
            if (i < len && programText[i] == '[')
            {
                multiCommentDepth++;
                i++;
                continue;
            }
            if (i < len && programText[i] == ']' && multiCommentDepth > 0)
            {
                multiCommentDepth--;
                i++;
                continue;
            }
            if (multiCommentDepth > 0)
            {
                i++;
                continue;
            }
            if (i < len && programText[i] == ';')
            {
                var instr = sb.ToString().Trim();
                if (!string.IsNullOrEmpty(instr))
                    result.Add(instr);
                sb.Clear();
                i++;
                continue;
            }
            sb.Append(programText[i]);
            i++;
        }
        var last = sb.ToString().Trim();
        if (!string.IsNullOrEmpty(last))
            result.Add(last);

        return result;
    }
}