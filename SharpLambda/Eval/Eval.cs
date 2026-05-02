using SharpLambda.DataTypes;
using SharpLambda.Exceptions;
using SharpLambda.Factories;
using SharpLambda.Utils;
using System.Diagnostics;

namespace SharpLambda.Eval;

public static class Eval
{
    private static Stopwatch _sw = new Stopwatch();
    
    public static Term Evaluate(Term term, Context context, ref int steps)
    {
        if (steps == 0)
        {
            throw new StackOverflowException();
        }

        steps--;
        if (steps % 1000 == 0)
        {
            Console.WriteLine($"{steps}, {_sw.ElapsedMilliseconds}");
            _sw.Restart();
        }
        
        if (!term.IsApplication())
        {
            return term;
        }

        var head = term.Application.Head;
        var args = term.Application.Args;
        if (args.Count == 0)
        {
            return Evaluate(head, context, ref steps);
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
            return Evaluate(TermFactory.Application([Evaluate(head, context, ref steps), ..args]), context, ref steps);
        }

        if (head.IsAbstraction())
        {
            return Evaluate(EvaluateApplication(term), context, ref steps);
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
        return TermFactory.String($"#DEFINED {name}#");
    }

    // externí funkce používají aplikativní vyhodnocování
    private static Term EvaluateExternalFunction(ExternalFunction head, List<Term> args, Context context)
    {
        int steps = 100;
        args = args.Select(x => Evaluate(x, context, ref steps)).ToList();
        return head.Function(args);
    }

    private static Term EvaluateApplication(Term term)
    {
        if (!term.IsApplication() || !term.Application.Head.IsAbstraction())
        {
            return term;
        }

        var head = term.Application.Head;
        var args = term.Application.Args;
        
        // checkuju to už nahoře, ale jenom pro jistotu 
        if (args.Count == 0)
        {
            return term;
        }

        if (head.Abstraction.Parameters.Count == 0)
        {
            return head.Abstraction.Body;
        }

        var arg = args.Car();
        var rest = args.Cdr();

        head = TryAlphaReduce(head, arg);
        head = BetaReduce(head, arg);

        return rest.Count == 0 ? head : TermFactory.Application([head, ..rest]);
    }

    private static Term TryAlphaReduce(Term head, Term arg)
    {
        // u arg potřebuju freeVars, u head paramNames
        var freeVarNames = new List<string>();
        var paramNames = new List<string>();
        CollectVariableNames(head, [], paramNames);
        CollectVariableNames(arg, freeVarNames, []);
        var paramsToRename = paramNames.Intersect(freeVarNames);

        // pokud je někdy konflikt (freeVar == paramName), udělám na hlavě alpha redukci (přejmenuju výskyty paramName)
        return paramsToRename.Aggregate(head, AlphaReduce);
    }
    
    // přejmenuju parametr a pak rekurzivně přejmenuju všechny proměnný v těle
    private static Term AlphaReduce(Term abstraction, string name)
    {
        if (!abstraction.IsAbstraction())
        {
            return abstraction;
        }
        
        var paramIndex = abstraction.Abstraction.Parameters.IndexOf(name);
        if (paramIndex == -1)
        {
            return abstraction;
        }

        var paramFound = false;
        return AlphaReduceRec(abstraction, name, NameGenerator.GetName(), ref paramFound);
    }

    // rekurze my beloved
    // paramFound -> pokud je true, rekurze se zastaví v lambdě co obsahuje stejně pojmenovanej parametr, 
    private static Term AlphaReduceRec(Term term, string name, string newName, ref bool paramFound)
    {
        if (term.IsVariable() && term.Variable.Name == name)
        {
            return TermFactory.Variable(newName);
        }

        if (term.IsApplication())
        {
            var newTerms = new List<Term>();

            // kvůli ref nemůžu použít linq
            foreach (var x in term.Application.Terms)
            {
                newTerms.Add(AlphaReduceRec(x, name, newName, ref paramFound));
            }

            return TermFactory.Application(newTerms);
        }

        // external nebo nezajímavá proměnná
        if (!term.IsAbstraction())
        {
            return term;
        }

        // abstrakce
        var containsParamWithName = term.Abstraction.Parameters.Contains(name);
        if (containsParamWithName)
        {
            if (paramFound)
            {
                return term; // jsem mimo scope a svůj už jsem prošel
            }

            // jdu poprvé do svého scopu
            paramFound = true;
            return TermFactory.Abstraction(
                term.Abstraction.Parameters.ReplaceImut(name, newName), 
                AlphaReduceRec(term.Abstraction.Body, name, newName, ref paramFound));
        }
            
        // normální abstrakce, jdu dovnitř
        return TermFactory.Abstraction(
            term.Abstraction.Parameters, 
            AlphaReduceRec(term.Abstraction.Body, name, newName, ref paramFound));

    }

    private static void CollectVariableNames(Term term, List<string> freeVariableNames, List<string> paramNames)
    {
        if (term.IsVariable())
        {
            if (freeVariableNames.Contains(term.Variable.Name) || paramNames.Contains(term.Variable.Name))
            {
                return;
            }
            freeVariableNames.Add(term.Variable.Name);
        }
        
        if (term.IsApplication())
        {
            foreach (var x in term.Application.Terms)
            {
                CollectVariableNames(x, freeVariableNames, paramNames);
            }
        }

        if (term.IsAbstraction())
        {
            foreach (var parameter in term.Abstraction.Parameters)
            {
                if (!paramNames.Contains(parameter))
                {
                    paramNames.Add(parameter);
                }
            }
            CollectVariableNames(term.Abstraction.Body, freeVariableNames, paramNames);
        }
    }
    
    private static Term BetaReduce(Term abstraction, Term arg)
    {
        if (!abstraction.IsAbstraction())
        {
            return abstraction;
        }
        
        var param = abstraction.Abstraction.Parameters.FirstOrDefault();
        if (param == null)
        {
            return abstraction;
        }

        var body = BetaReduceRec(abstraction.Abstraction.Body, arg, param);
        var args = abstraction.Abstraction.Parameters.Cdr();
        if (args.Count == 0)
        {
            return body;
        }
        return TermFactory.Abstraction(args, body);
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

            return TermFactory.Abstraction(term.Abstraction.Parameters, BetaReduceRec(term.Abstraction.Body, arg, paramName));
        }

        return term;
    }
}
