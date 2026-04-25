using SharpLambda.DataTypes;
using SharpLambda.Exceptions;
using SharpLambda.Factories;
using SharpLambda.Utils;

namespace SharpLambda.Defined;

public static class ExternalFunctions
{
    public static Dictionary<string, ExternalFunction> Functions { get; } = new Dictionary<string, ExternalFunction>();

    static ExternalFunctions()
    {
        Functions.Add("+", new ExternalFunction(Sum));
        Functions.Add("-", new ExternalFunction(Subtract));
        Functions.Add("*", new ExternalFunction(Multiply));
        Functions.Add("/", new ExternalFunction(Divide));
        Functions.Add("=", new ExternalFunction(Eql));
        Functions.Add(">", new ExternalFunction(Gt));
    }
    
    private static Term Sum(Term term)
    {
        Term Body(List<Term> args)
        {
            return NumberFoldr(nameof(Multiply), args, (a, b) => a + b, (a, b) => a + b, 0);
        }

        return ApplyExternalFunction(nameof(Sum), term, Body);
    }
    
    private static Term Subtract(Term term)
    {
        Term Body(List<Term> args)
        {
            if (args.Count == 0)
            {
                throw new ExternalFunctionArgCountException(nameof(Subtract), 1, 0);
            }
            
            var _args = new List<Term>(args.Cdr());
            _args.Reverse();
        
            var first = args.Car().External?.GetNumber() ?? throw new ExternalFunctionArgNotNumberException(nameof(Subtract));
            if (_args.Count == 0)
            {
                return TermFactory.Float(-first);
            }
            return NumberFoldr(nameof(Subtract), _args, (a, b) => b - a, (a, b) => b - a, first);
        }

        return ApplyExternalFunction(nameof(Subtract), term, Body);
    }

    private static Term Multiply(Term term)
    {
        Term Body(List<Term> args)
        {
            return NumberFoldr(nameof(Multiply), args, (a, b) => a * b, (a, b) => a * b, 1);
        }

        return ApplyExternalFunction(nameof(Multiply), term, Body);
    }
    
    private static Term Divide(Term term)
    {
        double FuncFloat(double a, double b)
        {
            if (b == 0)
            {
                throw new ExternalFunctionDivisionException(nameof(Divide), term);
            }
            return a / b;
        }

        long FuncInt(long a, long b)
        {
            if (b == 0)
            {
                throw new ExternalFunctionDivisionException(nameof(Divide), term);
            }
            return a / b;
        }
        
        Term Body(List<Term> args)
        {
            return NumberFoldr(nameof(Divide), args, FuncInt, FuncFloat, 1);
        }

        return ApplyExternalFunction(nameof(Divide), term, Body);
    }

    private static Term Sqrt(Term term)
    {
        Term Body(List<Term> args)
        {
            if (args.Count != 1)
            {
                throw new ExternalFunctionArgCountException(nameof(Sqrt), 1, args.Count);
            }

            if (FunctionUtils.AllNumber(args))
            {
                return TermFactory.Float(double.Sqrt(args[0].External!.GetNumber()));
            }

            throw new ExternalFunctionArgNotNumberException(nameof(Sqrt));
        }
        
        return ApplyExternalFunction(nameof(Sqrt), term, Body);
    }

    private static Term Eql(Term term)
    {
        Term Body(List<Term> args)
        {
            if (args.Count != 2)
            {
                throw new ExternalFunctionArgCountException(nameof(Eql), 2, args.Count);
            }
            
            if (FunctionUtils.AllNumber(args))
            {
                var num1 = args[0].External!.GetNumber();
                var num2 = args[1].External!.GetNumber();
                return Math.Abs(num1 - num2) < 0.001 ? TermFactory.True() : TermFactory.False();
            }
            
            throw new ExternalFunctionArgNotNumberException(nameof(Eql));
        }

        return ApplyExternalFunction(nameof(Eql), term, Body);
    }
    
    private static Term Gt(Term term)
    {
        Term Body(List<Term> args)
        {
            if (args.Count != 2)
            {
                throw new ExternalFunctionArgCountException(nameof(Gt), 2, args.Count);
            }
            
            if (FunctionUtils.AllNumber(args))
            {
                var num1 = args[0].External!.GetNumber();
                var num2 = args[1].External!.GetNumber();
                return num1 > num2 ? TermFactory.True() : TermFactory.False();
            }
            
            throw new ExternalFunctionArgNotNumberException(nameof(Gt));
        }

        return ApplyExternalFunction(nameof(Gt), term, Body);
    }

    private static Term ApplyExternalFunction(string name, Term term, Func<List<Term>, Term> body)
    {
        if (!term.IsApplication())
        {
            throw new ExternalFunctionException(name, term);
        }

        try
        {
            var args = term.Application.Terms.Cdr();
            return body(args);
        }
        catch
        {
            throw new ExternalFunctionException(name, term);
        }
    }
    
    private static Term NumberFoldr(string funcName, List<Term> args, Func<long, long, long> funcInt, Func<double, double, double> funcFloat, double init)
    {
        if (FunctionUtils.AllInt(args))
        {
            var sub = ListUtils.Foldr(args.Select(x => x.External!.GetInt()).ToList(), funcInt, (long)init);
            return TermFactory.Int(sub);
        }

        if (FunctionUtils.AllNumber(args))
        {
            var sub = ListUtils.Foldr(args.Select(x => x.External!.GetNumber()).ToList(), funcFloat, init);
            return TermFactory.Float(sub);
        }

        throw new ExternalFunctionException(funcName, TermFactory.Application(args));
    }
}
