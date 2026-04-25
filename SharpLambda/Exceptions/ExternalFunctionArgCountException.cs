namespace SharpLambda.Exceptions;

public class ExternalFunctionArgCountException(string funcName, int required, int has) : Exception($"Invalid arg count for {funcName}, required {required}, got {has}");
