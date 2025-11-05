using System;
using NUnit.Framework;

[Serializable]
public class Factory
{
    public string name;
    public Resource resource;
    public double baseCost;
    public double costMultiplier;
    public double baseProduction;
    public double productionMultiplier;
    // Number of factories of the same type owned by the player€
    public bool isUnlocked;
    public double unlockCost;
    public ulong count;
    public double currentCost => Math.Ceiling(baseCost * Math.Pow(costMultiplier, count));
    public double GetProduction() => baseProduction * count * productionMultiplier;
    public double GetSellValue()
    {
        if (count <= 0) return 0;
        double multiplier = costMultiplier > 0 ? costMultiplier : 1.0;
        return Math.Ceiling(baseCost * Math.Pow(multiplier, count - 1) * 0.5);
    }
    public Factory(Resource _resource, string _name, double _baseCost, double _costMultiplier, double _baseProduction, double _unlockCost)
    {
        resource = _resource;
        name = _name;
        baseCost = _baseCost;
        costMultiplier = _costMultiplier;
        baseProduction = _baseProduction;
        productionMultiplier = 1;
        count = 0;
        unlockCost = _unlockCost;
        isUnlocked = _unlockCost == 0;
    }

}
/* 
    Steps for correct Factory implementation:
    1 - Create Resource instances in RaptorCore.cs and IdleManager.cs
    2 - Add Factory instances in IdleManager.cs for each Resource
    3 - Update Layout.uxml to display all Factories
    4 - Change numberOfFactories variable in LayoutController.cs to match the number of Factory instances
*/