using SharpLambda.DataTypes;

namespace SharpLambda.Utils;

public static class FunctionUtils
{
    public static bool AllInt(List<Term> args)
    {
        return args.All(arg => arg.IsExternal() && arg.External.IsInt());
    }
    
    public static bool AllNumber(List<Term> args)
    {
        return args.All(arg => arg.IsExternal() && (arg.External.IsInt() || arg.External.IsFloat()));
    }
}
