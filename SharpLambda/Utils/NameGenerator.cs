namespace SharpLambda.Utils;

public static class NameGenerator
{
    private static int _count = 0;

    public static string GetName()
    {
        _count++;
        return "VAR_" + _count;
    }
}
