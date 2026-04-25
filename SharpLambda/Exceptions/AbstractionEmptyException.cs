namespace SharpLambda.Exceptions;

public class AbstractionEmptyException(string funcName) : Exception($"{funcName}: Tried to initialize abstraction without parameters");
