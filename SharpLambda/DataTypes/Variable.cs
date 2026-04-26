using SharpLambda.Utils;

namespace SharpLambda.DataTypes;

public class Variable
{
    public string Name { get; private set; }
    
    public Variable(string name)
    {
        Name = name.ToUpper();
    }

    public void Rename(string newName)
    {
        Name = newName;
    }

    public override string ToString()
    {
        return Name;
    }

    public Variable Clone()
    {
        return new Variable(Name);
    }
}
