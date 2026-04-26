using SharpLambda.DataTypes;
using System.Text;

namespace SharpLambda.Parser;

public static class PreParser
{
    private static string ExpressionToUpper(char[] expr)
    {
        List<char> blacklist = ['λ'];
        char prev = '\0';
        var canProcess = true;
        for (int i = 0; i < expr.Length; i++)
        {
            if (expr[i] == '"' && prev != '\\')
            {
                canProcess = !canProcess;
            }

            if (!canProcess)
            {
                continue;
            }

            expr[i] = char.ToUpper(expr[i]);
        }

        return new string(expr);
    }

    private static string ExpandDefineds(string expr, Context context)
    {
        var builder = new StringBuilder(expr);
        foreach (var (name, term) in context.DefinedTerms)
        {
            builder.Replace(name, term.ToString());
        }

        var result = builder.ToString();
        if (expr == result)
        {
            return result;
        }

        return ExpandDefineds(result, context);
    }
    
    public static string PreParse(string expr, Context context)
    {
        var new_exp = ExpandDefineds(
            ExpressionToUpper(expr.Trim().ToCharArray()),
            context);
        var builder = new StringBuilder(new_exp);
        builder.Replace("LAMBDA", "λ");
        builder.Replace("Λ", "λ");
        builder.Replace('\n', ' ');
        builder.Replace('\t', ' ');
        return builder.ToString();
    }
}
