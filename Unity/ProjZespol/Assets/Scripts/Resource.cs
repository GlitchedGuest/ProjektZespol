using System;
using System.Collections.Generic;

[Serializable]
public class Resource
{
    public string name;

    private QuarkType _value;
    public QuarkType value {
        get => _value;
        set {
            if (Comparer<QuarkType>.Default.Compare(value, default) == 0)
                _value = default;
            else if (Comparer<QuarkType>.Default.Compare(value, Limit) > 0)
                _value = Limit;
            else
                _value = value;

        }
    }
    public QuarkType Limit;

    public Resource(string _name)
    {
        name = _name;
        _value = 0;
        Limit = 10000;
    }
}