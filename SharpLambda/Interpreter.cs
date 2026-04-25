using SharpLambda.DataTypes;
using SharpLambda.Parser;

namespace SharpLambda;

public static class Interpreter
{
    public static Term Eval(string expr, Context context)
    {
        return SharpLambda.Eval.Eval.Evaluate(
            Parser.Parser.Parse(
                PreParser.PreParse(expr, context)));
    }
}
