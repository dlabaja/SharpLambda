using SharpLambda.DataTypes;
using SharpLambda.Factories;
using SharpLambda.Parser;

namespace SharpLambda;

public static class Interpreter
{
    private static Term Expand(Term term, Context context)
    {
        if (term.IsVariable() && context.DefinedTerms.TryGetValue(term.Variable.Name, out Term? value))
        {
            return Expand(value, context);
        }

        if (term.IsApplication())
        {
            return TermFactory.Application(term.Application.Terms.Select(x => Expand(x, context)).ToList());
        }

        if (term.IsAbstraction())
        {
            return TermFactory.Abstraction(term.Abstraction.Parameters, Expand(term.Abstraction.Body, context));
        }

        return term;
    }
    
    public static Term Eval(string expr, Context context)
    {
        return SharpLambda.Eval.Eval.Evaluate(
            Expand(
                Parser.Parser.Parse(
                    PreParser.PreParse(expr, context)), 
                context),
            context);
    }
}
