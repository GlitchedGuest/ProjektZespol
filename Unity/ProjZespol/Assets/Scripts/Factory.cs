using NUnit.Framework;
using System;
using System.IO;

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

    public double costOverEstimate(int amount){
        double r = costMultiplier;

        double exact = baseCost * Math.Pow(r, count)
                       * (Math.Pow(r, amount) - 1)
                       / (r - 1);

        return exact + amount; 
    }
    public double GetProduction() => baseProduction * count * productionMultiplier;
    public double GetSellValue()
    {
        if (count <= 0) return 0;
        double multiplier = costMultiplier > 0 ? costMultiplier : 1.0;
        return Math.Ceiling(baseCost * Math.Pow(multiplier, count - 1) * 0.5);
    }
    public Factory() { }

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

    public void SerializeToStream(BinaryWriter writer)
    {
        writer.Write(name);

        writer.Write(baseCost);
        writer.Write(costMultiplier);
        writer.Write(baseProduction);
        writer.Write(productionMultiplier);

        writer.Write(isUnlocked);
        writer.Write(unlockCost);
        writer.Write(count);
    }

    public void DeserializeFromStream(BinaryReader reader)
    {
        name = reader.ReadString();

        baseCost = reader.ReadDouble();
        costMultiplier = reader.ReadDouble();
        baseProduction = reader.ReadDouble();
        productionMultiplier = reader.ReadDouble();

        isUnlocked = reader.ReadBoolean();
        unlockCost = reader.ReadDouble();
        count = reader.ReadUInt64();
    }


}