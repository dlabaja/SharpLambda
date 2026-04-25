namespace SharpLambda.DataTypes;

public class ExternalFunction
{
    public Func<Term, Term> Function { get; }

    public ExternalFunction(Func<Term, Term> function)
    {
        Function = function;
    }
}
