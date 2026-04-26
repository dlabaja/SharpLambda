using SharpLambda.Factories;
using SharpLambda.Utils;

namespace SharpLambda.DataTypes;

public class Abstraction
{
    public List<string> Parameters { get; private set; }
    public Term Body { get; private set; }

    public Abstraction(List<string> parameters, Term body)
    {
        Parameters = parameters;
        Body = body;
    }

    public string? PopFirstParam()
    {
        if (Parameters.Count == 0)
        {
            return null;
        }
        var param = Parameters.Car();
        Parameters = Parameters.Cdr();
        return param;
    }

    public void ReplaceBody(Term body)
    {
        Body = body;
    }

    public override string ToString()
    {
        return $"(λ ({string.Join(' ', Parameters)}) {Body})";
    }
}
