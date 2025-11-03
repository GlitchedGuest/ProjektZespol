using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RaptorCore : MonoBehaviour
{

    public QuarkType Currency
    {
        get => GetResourceValue(currentResource);
        set
        {
            if(resources.ContainsKey(currentResource))
            {
                resources[currentResource].value = value;
            }
        }
    }

    //Click based
    QuarkType CBasevalue = 1;
    QuarkType CMultiplier = 1;


    //Click based
    QuarkType GBasevalue = 0;
    QuarkType GMultiplier = 1;



    [SerializeField] private Animator anim;
    [SerializeField] private CharacterClass characterClass;
    public double Gold = 0;
    private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
    private string currentResource = "Resource1";

    private void Awake()
    {
        UpdateUI();
    }

    private void OnMouseDown()
    {
        this.click();
        anim.SetTrigger("Clicked");
        UpdateUI();
    }
    void click()
    {
        QuarkType value = 0;
        
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        if (chance < characterClass.GetCriticalChance())
        {
            value += (CBasevalue * CMultiplier * 3); //to do zmiany gdy będzie wchodzić temat balansu
            Debug.Log("Kryt " + chance);
        }
        else
            value = (CBasevalue * CMultiplier);
        AddResource(currentResource, value);
    }

    void Start()
    {
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier); //.Pow(GPower);


    }

    void Update()
    {
        
    }
    void UpdateUI()
    {
        QuarkType currentResourceAmount = GetResourceValue(currentResource);
        LayoutController.Instance?.SetCurrencyText(currentResourceAmount.ToString());
        LayoutController.Instance?.SetGoldText(Gold.ToString());
    }
    public double SellMaterials(float sellValue)
    {
        //TODO: adjust values for correct progression
        switch(currentResource)
        {
            case "Resource1":
                return SellResource("Resource1", sellValue, 1.0);
            case "Resource2":
                return SellResource("Resource2", sellValue, 2.0);
            case "Resource3":
                return SellResource("Resource3", sellValue, 3.0);
            default:
                return 0;
        }
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

            if (resource == currentResource)
            {
                UpdateUI();
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
    public string GetCurrentResourceName()
    {
        return currentResource;
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

            if (resource == currentResource)
            {
                UpdateUI();
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
            UpdateUI();
        }
    }

    public double SellResource(string resource, float sellvalue, double pricePerUnit = 1.0)
    {
        QuarkType currentAmount = GetResourceValue(resource);
        var amountToSell = (currentAmount * (sellvalue / 100f)).Ceil();

        if (amountToSell <= 0)
        {
            return 0;
        }
        
        double goldEarned = amountToSell * pricePerUnit;
        Gold += goldEarned;
        RemoveResource(resource, amountToSell);
        UpdateUI();
        return goldEarned;
    }
}
