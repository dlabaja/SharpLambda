namespace SharpLambda.Exceptions;

public class VariableNameInvalidException(string funcName, string name) : Exception($"{funcName}: Tried to initialize variable with name '{name}'");
