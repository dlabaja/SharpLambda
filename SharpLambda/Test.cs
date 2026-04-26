 using SharpLambda.DataTypes;
using System.Diagnostics;

namespace SharpLambda;

public class Test
{
    private Context _context = new Context();
    
    public void TestInterpreter()
    {
        // přebráno z PP2 skript
        Assert("x", "x");
        Assert("(x y)", "(x y)");
        Assert("(λ (x) x)", "(λ (x) x)");
        Assert("(λ (x) u)", "(λ (x) u)");
        Assert("(u (λ (x) x))", "(u (λ (x) x))");
        Assert("((λ (x) x) u)", "u");
        Assert("((λ (x) x) u v)", "(u v)");
        Assert("((λ (x y) (y x)) u)", "(λ (y) (y u))");
        Assert("((λ (x y) (y x)) u v)", "(v u)");
        Assert("((λ (x) (λ (y) (y x))) u v)", "(v u)");
        Assert("((λ (u v) u) v)", "(λ (a) v)"); 
        Assert("((λ (u v) u) (u v))", "(λ (b) (u v))");
        Assert("((λ (x y) (x y)) (λ (z) (z z)) u)", "(u u)");
        Assert("((λ (x y) (y x)) ((λ (z) (z z)) u))", "(λ (y) (y ((λ (z) (z z)) u)))");
        Assert("((λ (x y) (y x)) ((λ (z) u) v) (λ (x y) (y y)) (λ (z) z))", "(λ (z) z)");
    }

    private void Assert(string expr, string result)
    {
        var res1 = Interpreter.Eval(expr, _context);
        var res2 = Interpreter.Eval(result, _context);
        Debug.Assert(Term.Equals(res1, res2));
        Console.WriteLine($"Result {res1}, expected {res2}");
    }
}
