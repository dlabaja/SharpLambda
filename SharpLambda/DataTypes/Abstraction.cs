namespace SharpLambda.DataTypes;

public class Abstraction
{
    public List<string> Parameters { get; }
    public Term Body { get; }

    public Abstraction(List<string> parameters, Term body)
    {
        Parameters = parameters;
        Body = body;
    }

    public override string ToString()
    {
        return $"(λ ({string.Join(' ', Parameters)}) {Body})";
    }
}
