using SharpLambda.Utils;

namespace SharpLambda.DataTypes;

public class Abstraction
{
    public List<string> Parameters { get; private set; }
    public Term Body { get; private set; }

    public Abstraction(List<string> parameters, Term body)
    {
        Parameters = parameters;
        Body = body;
        ResolveParameterConflict(Body, Parameters);
    }

    public bool Curry()
    {
        if (Parameters.Count < 2)
        {
            return false;
        }

        var newAbstraction = new Term(new Abstraction(Parameters.Cdr(), Body));
        Parameters = [Parameters.Car()];
        Body = newAbstraction;
        return true;
    }
    
    // přejmenuju parametr a pak rekurzivně přejmenuju všechny proměnné v těle
    // počítám s tím že lambdy vevnitř nemají konflikty parametrů, to jsem řešil v konstruktoru
    public bool AlphaReduce(string name)
    {
        var paramIndex = Parameters.IndexOf(name);
        if (paramIndex == -1)
        {
            return false;
        }

        var newName = NameGenerator.GetName();
        Parameters[paramIndex] = newName;
        AlphaReduceRec(Body, name, newName);
        return true;
    }

    // rekurze my beloved
    private void AlphaReduceRec(Term term, string name, string newName)
    {
        if (term.IsVariable() && term.Variable.Name == name)
        {
            term.Variable.Rename(newName);
            return;
        }

        if (term.IsApplication())
        {
            foreach (var item in term.Application.Terms)
            {
                AlphaReduceRec(item, name, newName);
            }
            return;
        }

        if (term.IsAbstraction())
        {
            AlphaReduceRec(term.Abstraction.Body, name, newName);
        }
    }

    private void ResolveParameterConflict(Term term, List<string> parameters)
    {
        if (term.IsApplication())
        {
            foreach (var item in term.Application.Terms)
            {
                ResolveParameterConflict(item, parameters);
            }
            return;
        }

        if (term.IsAbstraction())
        {
            var intersection = parameters.Intersect(term.Abstraction.Parameters).ToList();
            if (intersection.Count == 0)
            {
                ResolveParameterConflict(term.Abstraction.Body, parameters);
                return;
            }

            foreach (var paramName in intersection)
            {
                term.Abstraction.AlphaReduce(paramName);
            }
        }
    }
    
    public override string ToString()
    {
        return $"(λ ({string.Join(' ', Parameters)}) {Body})";
    }
}
