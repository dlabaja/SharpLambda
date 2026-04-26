namespace SharpLambda.Exceptions;

public class ParseException(string expr) : Exception($"Cannot parse the expression {expr}");
