namespace SharpLambda.DataTypes;

public class Context
{
    public Dictionary<string, Term> DefinedTerms { get; } = new Dictionary<string, Term>();

    public void Add(string name, Term body)
    {
        if (!DefinedTerms.TryAdd(name, body))
        {
            DefinedTerms[name] = body;
        }
    }
}
