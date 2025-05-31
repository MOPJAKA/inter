namespace Interpreter
{
    public enum AssignmentDirection
    {
        Left,
        Right
    }

    public enum UnarySyntax
    {
        OpBrackets,     // op(x)
        BracketsOp      // (x)op
    }

    public enum BinarySyntax
    {
        OpBrackets,     // op(x, y)
        BracketsOp,     // (x, y)op
        Infix           // x op y
    }
}
