using System;

[Serializable]
public class Factory
{
    public string name;
    public double baseCost;
    public double costMultiplier;
    public double baseProduction;
    public double productionMultiplier;
    // Number of factories of the same type owned by the player
    public ulong count;
    public double currentCost => Math.Ceiling(baseCost * Math.Pow(costMultiplier, count));
    public double GetProduction() => baseProduction * count * productionMultiplier;
    public double GetSellValue()
    {
        if (count <= 0) return 0;
        double multiplier = costMultiplier > 0 ? costMultiplier : 1.0;
        return Math.Ceiling(baseCost * Math.Pow(multiplier, count - 1) * 0.5);
    }
    public Factory(string _name, double _baseCost, double _costMultiplier, double _baseProduction)
    {
        name = _name;
        baseCost = _baseCost;
        costMultiplier = _costMultiplier;
        baseProduction = _baseProduction;
        productionMultiplier = 1;
        count = 0;
    }

}