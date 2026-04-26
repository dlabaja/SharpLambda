namespace SharpLambda.DataTypes;

public class ExternalFunction
{
    public string Name { get; }
    public Func<List<Term>, Term> Function { get; }

    public ExternalFunction(string name, Func<List<Term>, Term> function)
    {
        Name = name;
        Function = function;
    }
}
