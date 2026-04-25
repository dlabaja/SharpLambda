using SharpLambda;
using SharpLambda.DataTypes;

var context = new Context();
Console.WriteLine(Interpreter.Eval("ahoj", context));
