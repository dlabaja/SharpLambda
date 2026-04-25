namespace SharpLambda.DataTypes;

public class Primitive
{
    public Func<Term, Term> Function { get; }

    public Primitive(Func<Term, Term> function)
    {
        Function = function;
    }
}
