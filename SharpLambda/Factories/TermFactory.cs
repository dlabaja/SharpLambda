using SharpLambda.DataTypes;

namespace SharpLambda.Factories;

public static class TermFactory
{
    public static Term Abstraction(List<string> parameters, Term body)
    {
        return new Term(new Abstraction(parameters, body));
    }

    public static Term Application(List<Term> terms)
    {
        return new Term(new Application(terms));
    }

    public static Term Variable(string name)
    {
        return new Term(new Variable(name));
    }
}
