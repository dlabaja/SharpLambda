using SharpLambda.Utils;

namespace SharpLambda.DataTypes;

public class Application
{
    public List<Term> Terms { get; }
    public Term Head
    {
        get => Terms.Car();
    }
    
    public List<Term> Args
    {
        get => Terms.Cdr();
    }

    public Application(List<Term> terms)
    {
        Terms = terms;
    }

    public override string ToString()
    {
        return $"({string.Join(' ', Terms)})";
    }
}
