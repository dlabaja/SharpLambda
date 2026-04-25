using System.Diagnostics.CodeAnalysis;

namespace SharpLambda.DataTypes;

public enum TermType
{
    Abstraction,
    Application,
    Variable
}

public class Term
{
    public TermType Type { get; }
    public Abstraction? Abstraction { get; }
    public Application? Application { get; }
    public Variable? Variable { get; }

    public Term(Abstraction abstraction)
    {
        Type = TermType.Abstraction;
        Abstraction = abstraction;
    }
    
    public Term(Application application)
    {
        Type = TermType.Application;
        Application = application;
    }
    
    public Term(Variable variable)
    {
        Type = TermType.Variable;
        Variable = variable;
    }

    [MemberNotNullWhen(true, nameof(Abstraction))]
    public bool IsAbstraction()
    {
        return Type == TermType.Abstraction;
    }
    
    [MemberNotNullWhen(true, nameof(Application))]
    public bool IsApplication()
    {
        return Type == TermType.Application;
    }
    
    [MemberNotNullWhen(true, nameof(Variable))]
    public bool IsVariable()
    {
        return Type == TermType.Variable;
    }

    public override string ToString()
    {
        if (IsAbstraction())
        {
            return Abstraction.ToString();
        }

        if (IsApplication())
        {
            return Application.ToString();
        }

        if (IsVariable())
        {
            return Variable.ToString();
        }

        return "";
    }
}
