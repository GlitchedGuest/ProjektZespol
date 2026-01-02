using System;
using System.Collections.Generic;
using UnityEngine;
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
    public QuarkType Limit
    {
        get
        {
            QuarkType limit = ((BaseLimit * LvlBoost) + SkillBoost).Ceil();
            return limit;
        }
    }

    public QuarkType BaseLimit;
    public QuarkType LvlBoost;
    public QuarkType SkillBoost;

    public Resource(string _name)
    {
        name = _name;
        _value = 0;
        BaseLimit = 10000;
        LvlBoost = 1;
        SkillBoost = 0;
}
}