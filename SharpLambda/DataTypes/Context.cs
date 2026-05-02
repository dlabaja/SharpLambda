namespace SharpLambda.DataTypes;

public class Context
{
    public Dictionary<string, Term> DefinedTerms { get; } = new Dictionary<string, Term>();

    public void Add(string name, Term body)
    {
        var _name = name.ToUpper();
        if (!DefinedTerms.TryAdd(_name, body))
        {
            DefinedTerms[_name] = body;
        }
    }
}
