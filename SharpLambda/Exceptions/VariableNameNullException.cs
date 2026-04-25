namespace SharpLambda.Exceptions;

public class VariableNameNullException(string funcName) : Exception($"{funcName}: Tried to initialize variable without a name");
