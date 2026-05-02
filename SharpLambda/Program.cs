using SharpLambda.DataTypes;

namespace SharpLambda;

internal static class Program
{
    private static readonly Context _context = new Context();
    
    public static void Main(string[] args)
    {
        Eval("(define true (λ (a b) a))");
        Eval("(define false (λ (a b) b))");
        Eval("(define if (λ (p c a) (p c a)))");
        Eval("(define cons (λ (x y z) (z x y)))");
        Eval("(define car (λ (p) (p true)))");
        Eval("(define cdr (λ (p) (p false)))");
        Eval("(define succ (λ (n) (cons false n)))");
        Eval("(define pred (λ (n) (cons false n)))");
        Eval("(define zero (λ (x) x))");
        Eval("(define one (λ (z) (z false zero)))");
        Eval("(define two (λ (z) (z false one)))");
        Eval("(define three (λ (z) (z false two)))");
        Eval("(define four (λ (z) (z false three)))");
        Eval("(define zero? car)");
        Eval("(define i+% (λ (f a b) (if (zero? b) a (f f (succ a) (pred b)))))");
        Eval("(define i+ (λ (a b) (i+% i+% a b)))");
        Eval("(i+ one one)");
    }

    private static void Eval(string expr)
    {
        Console.WriteLine(Interpreter.Eval(expr, _context));
    }
}

// testování -> pusťte v Debug modu tuhle metodu, pokud se debugger na něčem zasekne tak se něco vyhodnocuje špatně
// new Test().TestInterpreter();
