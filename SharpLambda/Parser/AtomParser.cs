using SharpLambda.DataTypes;
using SharpLambda.Defined;
using SharpLambda.Factories;
using System.Globalization;

namespace SharpLambda.Parser;

public static class AtomParser
{
    public static Term ParseAtom(string expr)
    {
        Term atom;
        if (double.TryParse(expr, CultureInfo.InvariantCulture, out var floatNum))
        {
            atom = TermFactory.Float(floatNum);
        }
        else if (long.TryParse(expr, out var num))
        {
            atom = TermFactory.Int(num);
        }
        else if (expr.StartsWith('"') && expr.EndsWith('"'))
        {
            atom = TermFactory.String(expr);
        }
        else if (ExternalFunctions.Functions.TryGetValue(expr, out ExternalFunction? value))
        {
            atom = TermFactory.Function(value);
        }
        else
        {
            atom = TermFactory.Variable(expr);
        }

        return atom;
    }
}
