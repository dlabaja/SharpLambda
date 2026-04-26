using SharpLambda.DataTypes;
using SharpLambda.Exceptions;

namespace SharpLambda.Factories;

public static class TermFactory
{
    public static Term Abstraction(List<string> parameters, Term body)
    {
        if (parameters.Count == 0)
        {
            throw new AbstractionEmptyException(nameof(Abstraction));
        }
        return new Term(new Abstraction([..parameters], body));
    }

    public static Term Application(List<Term> terms)
    {
        if (terms.Count == 0)
        {
            throw new ApplicationEmptyException(nameof(Application));
        }
        return new Term(new Application([..terms]));
    }

    public static Term Variable(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new VariableNameInvalidException(nameof(Variable), name);
        }
        return new Term(new Variable(name));
    }
    
    public static Term Int(long value)
    {
        return new Term(new External(value, typeof(long)));
    }
    
    public static Term Float(double value)
    {
        return new Term(new External(value, typeof(double)));
    }
    
    public static Term String(string content)
    {
        return new Term(new External(content, typeof(string)));
    }

    public static Term Function(ExternalFunction externalFunction)
    {
        return new Term(new External(externalFunction, typeof(ExternalFunction)));
    }

    public static Term True()
    {
        return new Term(new Abstraction(["a", "b"], Variable("a")));
    }
    
    public static Term False()
    {
        return new Term(new Abstraction(["a", "b"], Variable("b")));
    }
}
