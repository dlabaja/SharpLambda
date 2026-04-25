namespace SharpLambda.DataTypes;

public class Application
{
    public List<Term> Terms { get; }

    public Application(List<Term> terms)
    {
        Terms = terms;
    }
}
