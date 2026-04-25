namespace SharpLambda.Exceptions;

public class ApplicationEmptyException(string funcName) : Exception($"{funcName}: Tried to initialize application without terms");
