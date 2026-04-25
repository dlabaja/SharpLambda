namespace SharpLambda.Exceptions;

public class ExternalFunctionArgNotNumberException(string funcName) : Exception($"Invalid arg type for {funcName}, needs to be a number");
