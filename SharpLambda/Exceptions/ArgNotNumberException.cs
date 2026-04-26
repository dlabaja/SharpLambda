namespace SharpLambda.Exceptions;

public class ArgNotNumberException(string funcName) : Exception($"Invalid arg type for {funcName}, needs to be a number");
