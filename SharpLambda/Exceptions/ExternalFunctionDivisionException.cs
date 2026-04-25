using SharpLambda.DataTypes;

namespace SharpLambda.Exceptions;

public class ExternalFunctionDivisionException(string funcName, Term application) : Exception($"Division by zero error - called {funcName} with application {application}");

