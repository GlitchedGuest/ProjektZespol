using System;

[Serializable]
public class Factory
{
    public string name;
    public double baseCost = 1;
    public double costMultiplier = 1.05;
    public double baseProduction = 1;
    public double productionMultiplier = 1.05;
    // Number of factories of the same type owned by the player
    public int count = 1;
    public double currentCost => baseCost * Math.Pow(costMultiplier, count);
    public double GetProduction() => baseProduction * productionMultiplier * count;
    public double GetSellValue()
    {
        if (count <= 0) return 0;
        double multiplier = costMultiplier > 0 ? costMultiplier : 1.0;
        return baseCost * Math.Pow(multiplier, count - 1) * 0.5;
    }

    public Factory(string _name)
    {
        name = _name;
    }
}