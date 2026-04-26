using SharpLambda.DataTypes;
using SharpLambda.Exceptions;
using SharpLambda.Factories;
using SharpLambda.Utils;

namespace SharpLambda.Eval;

public static class Eval
{
    public static Term Evaluate(Term term, Context context)
    {
        if (!term.IsApplication())
        {
            return term;
        }

        var head = term.Application.Head;
        var args = term.Application.Args;
        if (args.Count == 0)
        {
            return Evaluate(head, context);
        }

        if (head.IsVariable() && head.Variable.Name == "DEFINE")
        {
            return EvaluateDefine(term.Application, context);
        }
        
        if (head.IsExternal() && head.External.IsFunction())
        {
            return EvaluateExternalFunction(head.External.GetFunction(), args, context);
        }

        if (head.IsApplication())
        {
            return Evaluate(TermFactory.Application([Evaluate(head, context), ..args]), context);
        }

        if (head.IsAbstraction())
        {
            return Evaluate(EvaluateApplication(term), context);
        }

        return term;
    }

    private static Term EvaluateDefine(Application application, Context context)
    {
        if (application.Args.Count != 2)
        {
            throw new ArgCountException("DEFINE", 2, application.Args.Count);
        }

        var name = application.Args[0];
        var body = application.Args[1];

        if (!name.IsVariable())
        {
            throw new ArgNotVariableException("DEFINE");
        }
        context.Add(name.Variable.Name, body);
        return body;
    }

    // externí funkce používají aplikativní vyhodnocování
    private static Term EvaluateExternalFunction(ExternalFunction head, List<Term> args, Context context)
    {
        args = args.Select(x => Evaluate(x, context)).ToList();
        return head.Function(args);
    }

    private static Term EvaluateApplication(Term term)
    {
        if (!term.IsApplication() || !term.Application.Head.IsAbstraction())
        {
            return term;
        }

        var head = term.Application.Head.Abstraction!;
        var args = term.Application.Args;
        
        // checkuju to už nahoře, ale jenom pro jistotu 
        if (args.Count == 0)
        {
            return term;
        }

        if (head.Parameters.Count == 0)
        {
            return head.Body;
        }

        var arg = args.Car();
        var rest = args.Cdr();

        TryAlphaReduce(term, arg);

        BetaReduce(head, arg);

        if (head.Parameters.Count == 0)
        {
            if (rest.Count == 0)
            {
                return head.Body;
            }

            return TermFactory.Application([head.Body, ..rest]);
        }

        return rest.Count == 0 
            ? term.Application.Head
            : TermFactory.Application([term.Application.Head, ..rest]);
    }

    private static void TryAlphaReduce(Term head, Term arg)
    {
        // u arg potřebuju freeVars, u head paramNames
        var freeVarNames = new List<string>();
        var paramNames = new Dictionary<string, Abstraction>();
        CollectVariableNames(head, [], paramNames);
        CollectVariableNames(arg, freeVarNames, []);
        
        // pokud je některá z volných proměnných v seznamu parametrů, musím ji zredukovat
        foreach (var freeVarName in freeVarNames)
        {
            if (paramNames.TryGetValue(freeVarName, out var abstraction))
            {
                AlphaReduce(abstraction, freeVarName);       
            }
        }
    }
    
    // přejmenuju parametr a pak rekurzivně přejmenuju všechny proměnný v těle
    private static void AlphaReduce(Abstraction abstraction, string name)
    {
        var paramIndex = abstraction.Parameters.IndexOf(name);
        if (paramIndex == -1)
        {
            return;
        }

        var newName = NameGenerator.GetName();
        abstraction.Parameters[paramIndex] = newName;
        AlphaReduceRec(abstraction.Body, name, newName);
    }

    // rekurze my beloved
    private static void AlphaReduceRec(Term term, string name, string newName)
    {
        if (term.IsVariable() && term.Variable.Name == name)
        {
            term.Variable.Rename(newName);
            return;
        }

        if (term.IsApplication())
        {
            ListUtils.Mapcar(term.Application.Terms, x =>
            {
                AlphaReduceRec(x, name, newName);
                return x;
            });
            return;
        }

        if (term.IsAbstraction())
        {
            // tady končí scope
            if (term.Abstraction.Parameters.Contains(name))
            {
                return;
            }
            AlphaReduceRec(term.Abstraction.Body, name, newName);
        }
    }

    private static void CollectVariableNames(Term term, List<string> freeVariableNames, Dictionary<string, Abstraction> paramNames)
    {
        if (term.IsVariable())
        {
            if (freeVariableNames.Contains(term.Variable.Name) || paramNames.ContainsKey(term.Variable.Name))
            {
                return;
            }
            freeVariableNames.Add(term.Variable.Name);
        }
        
        if (term.IsApplication())
        {
            ListUtils.Mapcar(term.Application.Terms, x =>
            {
                CollectVariableNames(x, freeVariableNames, paramNames);
                return x;
            });
        }

        if (term.IsAbstraction())
        {
            foreach (var parameter in term.Abstraction.Parameters)
            {
                paramNames.TryAdd(parameter, term.Abstraction);
            }
            CollectVariableNames(term.Abstraction.Body, freeVariableNames, paramNames);
        }
    }
    
    private static void BetaReduce(Abstraction abstraction, Term arg)
    {
        var param = abstraction.PopFirstParam();
        if (param == null)
        {
            return;
        }

        abstraction.ReplaceBody(BetaReduceRec(abstraction.Body, arg, param));
    }

    private static Term BetaReduceRec(Term term, Term arg, string paramName)
    {
        if (term.IsVariable())
        {
            if (term.Variable.Name == paramName)
            {
                return arg.Clone();
            }

            return term;
        }

        if (term.IsApplication())
        {
            var list = term.Application.Terms.Select(item => BetaReduceRec(item, arg, paramName)).ToList();
            return TermFactory.Application(list);
        }

        if (term.IsAbstraction())
        {
            // tady taky končí scope
            if (term.Abstraction.Parameters.Contains(paramName))
            {
                return term;
            }
            term.Abstraction.ReplaceBody(BetaReduceRec(term.Abstraction.Body, arg, paramName));
        }

        return term;
    }
}
