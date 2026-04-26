namespace SharpLambda.Exceptions;

public class ArgNotVariableException(string funcName) : Exception($"Invalid arg type for {funcName}, needs to be a variable");
