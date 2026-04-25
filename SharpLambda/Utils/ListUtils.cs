namespace SharpLambda.Utils;

public static class ListUtils
{
    public static T Car<T>(this List<T> list)
    {
        return list[0];
    }
    
    public static List<T> Cdr<T>(this List<T> list)
    {
        return list.Skip(1).ToList();
    }
    
    public static T Foldr<T>(List<T> list, Func<T, T, T> func, T init)
    {
        if (list.Count == 0)
        {
            return init;
        }

        return func(list[0], Foldr(list.Cdr(), func, init));
    }

    public static List<TRes> Mapcar<T, TRes>(List<T> list, Func<T, TRes> func)
    {
        return list.Select(func).ToList();
    }
}
