using SharpLambda.DataTypes;
using SharpLambda.Exceptions;
using SharpLambda.Factories;

namespace SharpLambda.Parser;

public static class ExpressionParser
{
    private const string _atomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-+/*-%!?.><=λ\"";
    private const string _exprChars = "()";
    
    public static Term ParseExpression(string expr)
    {
        var items = SplitExpression(expr);
        if (items.Count == 0)
        {
            throw new ParseException(expr);
        }

        if (IsAbstraction(items))
        {
            return ParseAbstraction(items);
        }
        
        var terms = items.Select(Parser.Parse).ToList();
        return TermFactory.Application(terms);
    }

    private static Term ParseAbstraction(List<string> items)
    {
        var parametersTerm = Parser.Parse(items[1]); // bude to aplikace s proměnnýma
        if (!parametersTerm.IsApplication() || !parametersTerm.Application.Terms.All(t => t.IsVariable()))
        {
            throw new ParseException(items[1]);
        }
        var parameters = parametersTerm.Application.Terms.Select(x => x.Variable!.Name).ToList();
        return TermFactory.Abstraction(parameters, Parser.Parse(items[2]));
    }
 
    private static bool IsAbstraction(List<string> items)
    {
        return items.Count == 3 && items[0] == "λ";
    }

    private static List<string> SplitExpression(string expr)
    {
        var result = new List<string>();
        var slicedExpr = expr.Substring(1, expr.Length - 2).Trim();
        if (slicedExpr.Length == 0)
        {
            return result;
        }
        
        return IndexesToParts(slicedExpr, GetExpressionPartsIndexes(slicedExpr));
    }

    private static List<string> IndexesToParts(string expr, List<(int start, int end)> list)
    {
        var result = new List<string>();
        foreach (var (start, end) in list)
        {
            var part = expr.Substring(start, end - start + 1).Trim();
            result.Add(part);
        }

        return result;
    } 
    
    private static List<(int start, int end)> GetExpressionPartsIndexes(string expr)
    {
        var indexList = new List<(int start, int end)>();
        int start = 0;
        var atomProcessing = false;
        var expressionProcessing = false;
        var numberOfBracketsToFind = 0;
        var inString = false;
        for (int i = 0; i < expr.Length; i++)
        {
            var c = expr[i];
            if (c == '\"')
            {
                inString = !inString;
            }
            
            if (_atomChars.Contains(c) && !atomProcessing && !expressionProcessing)
            {
                atomProcessing = true;
                start = i;
                continue;
            }

            if (!_atomChars.Contains(c) && atomProcessing && !inString)
            {
                atomProcessing = false;
                indexList.Add((start, i));
                continue;
            }

            if (_exprChars.Contains(c) && !atomProcessing && !expressionProcessing)
            {
                expressionProcessing = true;
                numberOfBracketsToFind = 0;
                start = i;
                continue;
            }

            if (c == '(' && expressionProcessing)
            {
                numberOfBracketsToFind++;
                continue;
            }

            if (c == ')' && expressionProcessing)
            {
                if (numberOfBracketsToFind == 0)
                {
                    expressionProcessing = false;
                    indexList.Add((start, i));
                }
                numberOfBracketsToFind--;
            }
            
        }

        if (atomProcessing)
        {
            indexList.Add((start, expr.Length - 1));
        }

        if (expressionProcessing)
        {
            indexList.Add((start, expr.Length - 1));
        }

        return indexList;
    }
}
