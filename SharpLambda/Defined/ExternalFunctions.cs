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
        Functions.Add("+", new ExternalFunction("+", Sum));
        Functions.Add("-", new ExternalFunction("-", Subtract));
        Functions.Add("*", new ExternalFunction("*", Multiply));
        Functions.Add("/", new ExternalFunction("/", Divide));
        Functions.Add("=", new ExternalFunction("=", Eql));
        Functions.Add(">", new ExternalFunction(">", Gt));
    }
    
    private static Term Sum(List<Term> args)
    {
        return NumberFoldr(nameof(Multiply), args, (a, b) => a + b, (a, b) => a + b, 0);
    }
    
    private static Term Subtract(List<Term> args)
    {
        if (args.Count == 0)
        {
            throw new ArgCountException(nameof(Subtract), 1, 0);
        }
            
        var _args = new List<Term>(args.Cdr());
        _args.Reverse();
        
        var first = args.Car().External?.GetNumber() ?? throw new ArgNotNumberException(nameof(Subtract));
        if (_args.Count == 0)
        {
            return TermFactory.Float(-first);
        }
        return NumberFoldr(nameof(Subtract), _args, (a, b) => b - a, (a, b) => b - a, first);
    }

    private static Term Multiply(List<Term> args)
    {
        return NumberFoldr(nameof(Multiply), args, (a, b) => a * b, (a, b) => a * b, 1);
    }
    
    private static Term Divide(List<Term> args)
    {
        double FuncFloat(double a, double b)
        {
            if (b == 0)
            {
                throw new DivisionException();
            }
            return a / b;
        }

        long FuncInt(long a, long b)
        {
            if (b == 0)
            {
                throw new DivisionException();
            }
            return a / b;
        }
        
        return NumberFoldr(nameof(Divide), args, FuncInt, FuncFloat, 1);
    }

    private static Term Sqrt(List<Term> args)
    {
        if (args.Count != 1)
        {
            throw new ArgCountException(nameof(Sqrt), 1, args.Count);
        }

        if (FunctionUtils.AllNumber(args))
        {
            return TermFactory.Float(double.Sqrt(args[0].External!.GetNumber()));
        }

        throw new ArgNotNumberException(nameof(Sqrt));
    }

    private static Term Eql(List<Term> args)
    {
        if (args.Count != 2)
        {
            throw new ArgCountException(nameof(Eql), 2, args.Count);
        }
            
        if (FunctionUtils.AllNumber(args))
        {
            var num1 = args[0].External!.GetNumber();
            var num2 = args[1].External!.GetNumber();
            return Math.Abs(num1 - num2) < 0.001 ? TermFactory.True() : TermFactory.False();
        }
            
        throw new ArgNotNumberException(nameof(Eql));
    }
    
    private static Term Gt(List<Term> args)
    {
        if (args.Count != 2)
        {
            throw new ArgCountException(nameof(Gt), 2, args.Count);
        }
            
        if (FunctionUtils.AllNumber(args))
        {
            var num1 = args[0].External!.GetNumber();
            var num2 = args[1].External!.GetNumber();
            return num1 > num2 ? TermFactory.True() : TermFactory.False();
        }
            
        throw new ArgNotNumberException(nameof(Gt));
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
