using SharpLambda.DataTypes;
using SharpLambda.Factories;
using SharpLambda.Utils;

namespace SharpLambda.Eval;

public static class Eval
{
    public static Term Evaluate(Term term, Context context)
    {
        if (term.IsApplication())
        {
            var head = term.Application.Head;
            var args = term.Application.Args;
            if (args.Count == 0)
            {
                return Evaluate(head, context);
            }
            
            if (head.IsExternal() && head.External.IsFunction())
            {
                return EvaluateExternalFunction(head.External.GetFunction(), args, context);
            }

            if (head.IsAbstraction())
            {
                return Evaluate(EvaluateAbstraction(head.Abstraction, args), context);
            }
        }

        return term;
    }

    // externí funkce používají aplikativní vyhodnocování
    private static Term EvaluateExternalFunction(ExternalFunction head, List<Term> args, Context context)
    {
        args = args.Select(x => Evaluate(x, context)).ToList();
        return head.Function(args);
    }

    private static Term EvaluateAbstraction(Abstraction head, List<Term> args)
    {
        // checkuju to už nahoře, ale jenom pro jistotu 
        if (args.Count == 0)
        {
            return TermFactory.Abstraction(head.Parameters, head.Body);
        }

        if (head.Parameters.Count == 0)
        {
            return head.Body;
        }

        var arg = args.Car();
        var rest = args.Cdr();
        if (arg.IsVariable() && head.Parameters[0] == arg.Variable.Name)
        {
            AlphaReduce(head, arg.Variable.Name);
        }

        BetaReduce(head, arg);

        var abstraction = TermFactory.Abstraction(head.Parameters, head.Body);
        if (rest.Count != 0)
        {
            return TermFactory.Application([abstraction, ..rest]);
        }

        if (head.Parameters.Count == 0)
        {
            return head.Body;
        }

        return abstraction;

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
                return arg;
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
