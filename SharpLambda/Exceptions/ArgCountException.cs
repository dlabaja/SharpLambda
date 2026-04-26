namespace SharpLambda.Exceptions;

public class ArgCountException(string funcName, int required, int has) : Exception($"Invalid arg count for {funcName}, required {required}, got {has}");
