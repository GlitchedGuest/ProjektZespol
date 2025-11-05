using System;

[Serializable]
public class Resource
{
    public string name;
    public QuarkType value;

    public Resource(string _name)
    {
        name = _name;
        value = 0;
    }
}