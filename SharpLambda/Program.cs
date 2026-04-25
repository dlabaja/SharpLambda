using SharpLambda;
using SharpLambda.DataTypes;

var context = new Context();
var expr = Interpreter.Eval("((lambda (a b) c) 1 2 d)", context);
Console.WriteLine(expr);
