using SharpLambda.Utils;
using System.Diagnostics.CodeAnalysis;

namespace SharpLambda.DataTypes;

public enum TermType
{
    Abstraction,
    Application,
    Variable,
    External
}

public class Term
{
    public TermType Type { get; }
    public Abstraction? Abstraction { get; }
    public Application? Application { get; }
    public Variable? Variable { get; }
    public External? External { get; }

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
    
    public Term(External external)
    {
        Type = TermType.External;
        External = external;
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
    
    [MemberNotNullWhen(true, nameof(External))]
    public bool IsExternal()
    {
        return Type == TermType.External;
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

        if (IsExternal())
        {
            return External.ToString();
        }

        return "";
    }
    
    public Term Clone()
    {
        if (IsVariable())
        {
            return new Term(Variable.Clone());
        }

        if (IsAbstraction())
        {
            return new Term(Abstraction.Clone());
        }

        if (IsApplication())
        {
            return new Term(Application.Clone());
        }

        if (IsExternal())
        {
            return new Term(External);
        }

        return this;
    }

    public static bool Equals(Term t1, Term t2)
    {
        if (t1.IsVariable())
        {
            return t2.IsVariable();
        }

        if (t1.IsExternal())
        {
            return t2.IsExternal()
                   && t1.External.Type == t2.External.Type
                   && t1.External.Value == t2.External.Value;
        }

        if (t1.IsAbstraction())
        {
            return t2.IsAbstraction()
                   && t1.Abstraction.Parameters.Count == t2.Abstraction.Parameters.Count
                   && Equals(t1.Abstraction.Body, t2.Abstraction.Body);
        }

        if (t1.IsApplication())
        {
            return t2.IsApplication()
                   && Equals(t1.Application.Head, t2.Application.Head)
                   && t1.Application.Args.Count == t2.Application.Args.Count
                   && ListUtils.Foldr(
                       t1.Application.Args.Zip(t2.Application.Args, Equals).ToList(),
                       (b1, b2) => b1 && b2,
                       true);
        }

        return false;
    }
}
