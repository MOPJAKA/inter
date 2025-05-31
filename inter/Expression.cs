using System;
using System.Collections.Generic;

namespace Interpreter
{
    public abstract class Expression { }

    public class Assignment : Expression 
    {
        public string VariableName { get; set; }
        public Expression Value { get; set; }

        public Assignment(string variableName, Expression value)
        {
            VariableName = variableName;
            Value = value;
        }
    }

    public class FunctionCall : Expression
    {
        public string Command { get; set; }
        public List<Expression> Arguments { get; set; }

        public FunctionCall(string command, List<Expression> arguments)
        {
            Command = command;
            Arguments = arguments;
        }
    }

    public class Variable : Expression
    {
        public string Name { get; set; }

        public Variable(string name)
        {
            Name = name;
        }
    }

    public class Constant : Expression
    {
        public uint Value { get; set; }

        public Constant(uint value)
        {
            Value = value;
        }
    }
}
