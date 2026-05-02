namespace SharpLambda.DataTypes;

public class Variable
{
    public string Name { get; }
    
    public Variable(string name)
    {
        Name = name.ToUpper();
    }

    public override string ToString()
    {
        return Name;
    }
}
