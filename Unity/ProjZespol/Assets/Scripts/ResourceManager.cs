using Mono.Cecil;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour, IBinarySaveable
{
    private IdleManager idleManager;
    private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

    public QuarkType resource1Value = 0;
    public QuarkType resource2Value = 0;
    public QuarkType resource3Value = 0;
    public QuarkType resource4Value = 0;
    public QuarkType resource5Value = 0;
    public QuarkType resource6Value = 0;

    public QuarkType resource1Limit = 1000;
    public QuarkType resource2Limit = 2500;
    public QuarkType resource3Limit = 5000;
    public QuarkType resource4Limit = 10000;
    public QuarkType resource5Limit = 25000;
    public QuarkType resource6Limit = 50000;

    public string currentResource = "Resource1";

    public int SaveKey => 001; //Saveid to keep order

    public void Initialize(IdleManager _idleManager)
    {
        idleManager = _idleManager;
    }

    public void LoadAllResources()
    {
        LoadResource("Resource1");
        LoadResource("Resource2");
        LoadResource("Resource3");
        LoadResource("Resource4");
        LoadResource("Resource5");
        LoadResource("Resource6");
    }

    void LoadResource(string name)
    {
        if (!resources.TryGetValue(name, out var res)) return;

        string p = char.ToLower(name[0]) + name[1..];
        var t = GetType();

        res.BaseLimit = (QuarkType)t.GetField(p + "Limit").GetValue(this);
        res.value = (QuarkType)t.GetField(p + "Value").GetValue(this);
    }

    public void RegisterResource(Resource resource)
    {
        if (!resources.ContainsKey(resource.name))
        {
            resources[resource.name] = resource;
        }
    }

    public void AddResource(string resource, QuarkType amount)
    {
        if (resources.ContainsKey(resource))
        {
            resources[resource].value += amount;
            SaveResourceValues();
            if (resource == currentResource)
            {
                LayoutController.Instance?.UpdateUI();
            }
        }
    }

    public QuarkType GetResourceValue(string resource)
    {
        if (resources.ContainsKey(resource))
        {
            return resources[resource].value;
        }
        return 0;
    }

    public QuarkType GetResourceValueDirect(string resourceName)
    {
        SaveResourceValues();
        
        switch(resourceName)
        {
            case "Resource1":
                return resource1Value;
            case "Resource2":
                return resource2Value;
            case "Resource3":
                return resource3Value;
            case "Resource4":
                return resource4Value;
            case "Resource5":
                return resource5Value;
            case "Resource6":
                return resource6Value;
            default:
                return GetResourceValue(resourceName);
        }
    }

    public bool HasResource(string resource, QuarkType amount)
    {
        return GetResourceValue(resource) >= amount;
    }

    public bool RemoveResource(string resource, QuarkType amount)
    {
        if (HasResource(resource, amount))
        {
            resources[resource].value -= amount;
            SaveResourceValues();

            if (resource == currentResource)
            {
                LayoutController.Instance?.UpdateUI();
            }
            return true;
        }
        return false;
    }

    public void SetCurrentResource(string resource)
    {
        if (resources.ContainsKey(resource))
        {
            currentResource = resource;
            LayoutController.Instance?.UpdateUI();
        }
    }

    public void SetResourceValueDirect(string resourceName, QuarkType value)
    {
        switch(resourceName)
        {
            case "Resource1":
                resource1Value = value;
                if (resources.ContainsKey("Resource1"))
                    resources["Resource1"].value = value;
                break;
            case "Resource2":
                resource2Value = value;
                if (resources.ContainsKey("Resource2"))
                    resources["Resource2"].value = value;
                break;
            case "Resource3":
                resource3Value = value;
                if (resources.ContainsKey("Resource3"))
                    resources["Resource3"].value = value;
                break;
            case "Resource4":
                resource4Value = value;
                if (resources.ContainsKey("Resource4"))
                    resources["Resource4"].value = value;
                break;
            case "Resource5":
                resource5Value = value;
                if (resources.ContainsKey("Resource5"))
                    resources["Resource5"].value = value;
                break;
            case "Resource6":
                resource6Value = value;
                if (resources.ContainsKey("Resource6"))
                    resources["Resource6"].value = value;
                break;
        }
        
        if (resourceName == currentResource)
        {
            LayoutController.Instance?.UpdateUI();
        }
    }

    public void SetLvlBoostResource(string resource, QuarkType limitvalue) 
    {
        if (resources.ContainsKey(resource))
        {
            resources[resource].LvlBoost *= limitvalue;
        }
    }

    public QuarkType GetLimitResource(string resource)
    {
        if (resources.ContainsKey(resource))
        {
            return resources[resource].Limit;
        }
        return default;
    }

    public QuarkType GetBaseLimitResource(string resource)
    {
        if (resources.ContainsKey(resource))
        {
            return resources[resource].BaseLimit;
        }
        return default;
    }

    public void IncrementSkillBoostResourceAll(bool mode)
    {
        IncrementSkillBoostResource("Resource1", (GetBaseLimitResource("Resource1") / 2), mode);
        IncrementSkillBoostResource("Resource2", (GetBaseLimitResource("Resource2") / 2), mode);
        IncrementSkillBoostResource("Resource3", (GetBaseLimitResource("Resource3") / 2), mode);
        IncrementSkillBoostResource("Resource4", (GetBaseLimitResource("Resource4") / 2), mode);
        IncrementSkillBoostResource("Resource5", (GetBaseLimitResource("Resource5") / 2), mode);
        IncrementSkillBoostResource("Resource6", (GetBaseLimitResource("Resource6") / 2), mode);
    }

    public void MullLimitResourceAll(QuarkType MullValue)
    {
        SetLvlBoostResource("Resource1", MullValue);
        SetLvlBoostResource("Resource2", MullValue);
        SetLvlBoostResource("Resource3", MullValue);
        SetLvlBoostResource("Resource4", MullValue);
        SetLvlBoostResource("Resource5", MullValue);
        SetLvlBoostResource("Resource6", MullValue);
    }

    public void IncrementSkillBoostResource(string resource, QuarkType AddValue, bool mode)
    {
        if (resources.ContainsKey(resource))
        {
            if(mode)
                resources[resource].SkillBoost += AddValue;
            else
            {
                resources[resource].SkillBoost -= AddValue;
                if (resources[resource].value > resources[resource].Limit)
                    resources[resource].value = resources[resource].Limit;
            }
        }
    }

    public void SaveResourceValues()
    {
        SaveResource("Resource1");
        SaveResource("Resource2");
        SaveResource("Resource3");
        SaveResource("Resource4");
        SaveResource("Resource5");
        SaveResource("Resource6");
    }

    void SaveResource(string name)
    {
        if (!resources.TryGetValue(name, out var res)) return;

        string p = char.ToLower(name[0]) + name[1..];
        var t = GetType();

        t.GetField(p + "Limit")?.SetValue(this, res.Limit);
        t.GetField(p + "Value")?.SetValue(this, res.value);
    }

    public bool CanClickCurrentResource()
    {
        if (idleManager == null) return true;

        foreach (var f in idleManager.factories)
        {
            if (f.resource.name == currentResource)
            {
                return f.isUnlocked;
            }
        }
        return true;
    }
    public Factory GetCurrentFactory()
    {
        if (idleManager == null) return null;
        
        foreach (var f in idleManager.factories)
        {
            if (f.resource.name == currentResource)
            {
                return f;
            }
        }

        return null;
    }

    public void SerializeToStream(BinaryWriter writer)
    {
        SaveResourceValues();
        resource1Value.SerializeToStream(writer);
        resource2Value.SerializeToStream(writer);
        resource3Value.SerializeToStream(writer);
        resource4Value.SerializeToStream(writer);
        resource5Value.SerializeToStream(writer);
        resource6Value.SerializeToStream(writer);

        resource1Limit.SerializeToStream(writer);
        resource2Limit.SerializeToStream(writer);
        resource3Limit.SerializeToStream(writer);
        resource4Limit.SerializeToStream(writer);
        resource5Limit.SerializeToStream(writer);
        resource6Limit.SerializeToStream(writer);

        writer.Write(currentResource); 
    }

    public void DeserializeFromStream(BinaryReader reader)
    {
        resource1Value.DeserializeFromStream(reader);
        resource2Value.DeserializeFromStream(reader);
        resource3Value.DeserializeFromStream(reader);
        resource4Value.DeserializeFromStream(reader);
        resource5Value.DeserializeFromStream(reader);
        resource6Value.DeserializeFromStream(reader);

        resource1Limit.DeserializeFromStream(reader);
        resource2Limit.DeserializeFromStream(reader);
        resource3Limit.DeserializeFromStream(reader);
        resource4Limit.DeserializeFromStream(reader);
        resource5Limit.DeserializeFromStream(reader);
        resource6Limit.DeserializeFromStream(reader);

        currentResource = reader.ReadString();
        LoadAllResources();
    }
}