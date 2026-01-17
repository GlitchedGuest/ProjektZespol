using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
[Serializable]
public class Resource
{
    public string name;

    public string displayName;

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

    public Resource() { }
    public Resource(string _name, string _displayName)
    {
        name = _name;
        displayName = _displayName;
        _value = 0;
        BaseLimit = 10000;
        LvlBoost = 1;
        SkillBoost = 0;
    }

    public void SerializeToStream(BinaryWriter writer)
    {
        writer.Write(name);

        _value.SerializeToStream(writer);
        BaseLimit.SerializeToStream(writer);
        LvlBoost.SerializeToStream(writer);
        SkillBoost.SerializeToStream(writer);
    }

    public void DeserializeFromStream(BinaryReader reader)
    {
        name = reader.ReadString();

        BaseLimit = new();
        LvlBoost = new();
        SkillBoost = new();

        BaseLimit.DeserializeFromStream(reader);
        LvlBoost.DeserializeFromStream(reader);
        SkillBoost.DeserializeFromStream(reader);


        _value = new();
        _value.DeserializeFromStream(reader);
    }

}