namespace SharpLambda.Exceptions;

public class ExternalTypeException(Type actualType, Type type) : Exception($"External is typeof {actualType}, tried to parse it to {type} instead");
