using SharpLambda.DataTypes;
using SharpLambda.Exceptions;
using SharpLambda.Factories;

namespace SharpLambda.Parser;

public static class Parser
{
    public static Term Parse(string expr)
    {
        if (IsAtom(expr))
        {
            return AtomParser.ParseAtom(expr);
        }

        if (IsExpresion(expr))
        {
            return ExpressionParser.ParseExpression(expr);
        }

        throw new ParseException($"Cannot parse the sexpression {expr}");
    }
    
    public static bool IsExpresion(string expr)
    {
        return expr[0] == '(' && expr[^1] == ')';
    }

    public static bool IsAtom(string expr)
    {
        return expr[0] != '(' && expr[^1] != ')';
    }
}
