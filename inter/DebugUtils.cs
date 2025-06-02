public static class DebugUtils
{
    public static bool ContainsBreakpoint(string instr)
    {
        var trimmed = instr.Trim();
        // Комментарий # BREAKPOINT
        return trimmed.StartsWith("#") && trimmed.ToUpper().Contains("BREAKPOINT");
    }
}