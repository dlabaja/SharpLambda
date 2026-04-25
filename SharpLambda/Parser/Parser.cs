using SharpLambda.DataTypes;
using SharpLambda.Factories;

namespace SharpLambda.Parser;

public static class Parser
{
    public static Term Parse(string expr)
    {
        return TermFactory.Variable("a");
    }
}
