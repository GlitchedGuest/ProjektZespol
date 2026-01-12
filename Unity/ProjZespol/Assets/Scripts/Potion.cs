using System;
using UnityEngine;

[Serializable]
public class Potion
{
    public string name;
    public Resource resourceType1;
    public Resource resourceType2;
    public QuarkType cost1;
    public QuarkType cost2;
    public string effect;
    public double duration;
    public bool isActive = false;
    public double timeRemaining;
    public Factory linkedFactory; // Fabryka, na którą działa mikstura

    public string effectDescription;
    public int boughtCount;

    public Potion(string _name, Resource _resource1, Resource _resource2, QuarkType _cost1, QuarkType _cost2, string _effect, double _duration, string _effectDescription)
    {
        name = _name;
        cost1 = _cost1;
        cost2 = _cost2;
        effect = _effect;
        duration = _duration;
        resourceType1 = _resource1;
        resourceType2 = _resource2;
        isActive = false;
        timeRemaining = 0;
        effectDescription = _effectDescription;
        linkedFactory=null;
        boughtCount = 0;
    }

}
