using System;
using UnityEngine;

[Serializable]
public class Potion
{
    public string name;
    public Resource resourceType;
    public QuarkType cost;
    public string effect;
    public double duration;
    public bool isActive = false;
    public double timeRemaining;
    public Factory linkedFactory; // Fabryka, na którą działa mikstura

    public string effectDescription;

    public Potion(string _name, Resource _resource, QuarkType _cost, string _effect, double _duration, string _effectDescription)
    {
        name = _name;
        cost = _cost;
        effect = _effect;
        duration = _duration;
        resourceType = _resource;
        isActive = false;
        timeRemaining = 0;
        effectDescription = _effectDescription;
    }
}
