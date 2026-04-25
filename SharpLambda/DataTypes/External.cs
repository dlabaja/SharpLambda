using SharpLambda.Exceptions;
using System.Globalization;

namespace SharpLambda.DataTypes;

public class External
{
    public Type Type { get; }
    public dynamic Value { get; }
    
    public External(dynamic value, Type type)
    {
        Value = value;
        Type = type;
    }
    
    public bool IsInt()
    {
        return Type == typeof(long);
    }

    public bool IsFloat()
    {
        return Type == typeof(double);
    }
    
    public bool IsString()
    {
        return Type == typeof(string);
    }
    
    public bool IsPrimitive()
    {
        return Type == typeof(Primitive);
    }
    
    public long GetInt()
    {
        if (!IsInt())
        {
            throw new ExternalTypeException(Type, typeof(long));
        }
        return Convert.ChangeType(Value, Type);
    }

    public double GetFloat()
    {
        if (!IsFloat())
        {
            throw new ExternalTypeException(Type, typeof(double));
        }
        return Convert.ChangeType(Value, Type);
    }
    
    public string GetString()
    {
        if (!IsString())
        {
            throw new ExternalTypeException(Type, typeof(string));
        }
        return Convert.ChangeType(Value, Type);
    }
    
    public Primitive GetPrimitive()
    {
        if (!IsPrimitive())
        {
            throw new ExternalTypeException(Type, typeof(Primitive));
        }
        return Convert.ChangeType(Value, Type);
    }
    
    public override string ToString()
    {
        if (IsString())
        {
            return $"{Value}";
        }

        if (IsFloat() || IsInt())
        {
            return Convert.ChangeType(Value, Type).ToString(CultureInfo.InvariantCulture) ?? "";
        }
        
        return Convert.ChangeType(Value, Type).ToString() ?? "";
    }
}
