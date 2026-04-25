using SharpLambda.DataTypes;

namespace SharpLambda.Exceptions;

public class ExternalFunctionException(string funcName, Term application) : Exception($"External function error - called {funcName} with application {application}");

