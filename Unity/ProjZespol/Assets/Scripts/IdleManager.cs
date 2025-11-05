using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour
{
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private float tickInterval = 1f;
    private Coroutine tickCoroutine;
    [SerializeField] private List<Factory> factories;
    [SerializeField] private List<Resource> resources;

    private void Awake()
    {
        InitResources();
        InitFactories();
    }

    private void InitResources()
    {
        if (resources == null) resources = new List<Resource>();

        resources.Clear();
        resources.Add(new Resource("Resource1"));
        resources.Add(new Resource("Resource2"));
        resources.Add(new Resource("Resource3"));

        foreach (var res in resources)
        {
            raptorCore.RegisterResource(res);
        }
    }
    private void InitFactories()
    {
        if (factories == null) factories = new List<Factory>();

        factories.Clear();

        //TODO: Adjust factory parameters as needed
        factories.Add(new Factory(resources[0], "F1.1", 15, 1.15, 1, 0));
        factories.Add(new Factory(resources[0], "F1.2", 100, 1.17, 10, 500));
        factories.Add(new Factory(resources[0], "F1.3", 1000, 1.2, 50, 2500));

        factories.Add(new Factory(resources[1], "F2.1", 200, 1.15, 5, 5000));
        factories.Add(new Factory(resources[1], "F2.2", 1500, 1.17, 25, 15000));
        factories.Add(new Factory(resources[1], "F2.3", 10000, 1.2, 100, 50000));

        factories.Add(new Factory(resources[2], "F3.1", 500, 1.15, 10, 100000));
        factories.Add(new Factory(resources[2], "F3.2", 3000, 1.17, 50, 250000));
        factories.Add(new Factory(resources[2], "F3.3", 20000, 1.2, 200, 500000));
    }
    private void OnEnable()
    {
        if (tickCoroutine == null)
        {
            tickCoroutine = StartCoroutine(TickLoop());
        }
   }

    private void OnDisable()
    {
        if (tickCoroutine != null)
        {
            StopCoroutine(tickCoroutine);
            tickCoroutine = null;
        }
    }

    private IEnumerator TickLoop()
    {
        var wait = new WaitForSecondsRealtime(tickInterval);
        while (true)
        {
            if (raptorCore != null)
            {
                ProcessTick();
            }
            yield return wait;
        }
    }
    
    private void ProcessTick()
    {
        if (factories == null || factories.Count == 0) return;

        foreach (var f in factories)
        {
            if (f == null || f.count <= 0) continue;
            double production = f.GetProduction();
            if (f.resource != null)
            {
                f.resource.value += (QuarkType)production;
            }
        }

        string currentResource = raptorCore.GetCurrentResourceName();
        QuarkType currentResourceAmount = raptorCore.GetResourceValue(currentResource);
        LayoutController.Instance?.SetCurrencyText(currentResourceAmount.ToString());
    }

    public bool UnlockFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            return false;
        }

        var f = factories[index];

        if (raptorCore.Gold >= f.unlockCost)
        {
            raptorCore.Gold -= f.unlockCost;
            f.isUnlocked = true;
            
            Debug.Log($"Odblokowano fabrykę '{f.name}' za {f.unlockCost} gold");
            
            LayoutController.Instance?.SetGoldText(raptorCore.Gold.ToString());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool BuyFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            return false;
        }

        var f = factories[index];
        
        double cost = f.currentCost;
        
        if (raptorCore.Gold >= cost)
        {
            raptorCore.Gold -= cost;
            f.count++;
            
            Debug.Log($"Kupiono fabrykę '{f.name}' (nowy count = {f.count}), koszt = {cost}");
            
            LayoutController.Instance?.SetGoldText(raptorCore.Gold.ToString("F2"));
            return true;
        }
        else
        {
            Debug.Log("BuyFactory: brak wystarczających środków");
            return false;
        }
    }

    public bool SellFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            return false;
        }

        var f = factories[index];
        if (f.count <= 0)
        {
            Debug.Log("SellFactory: brak fabryk do sprzedania");
            return false;
        }

        double sellValue = f.GetSellValue();
        f.count--;
        raptorCore.Gold += sellValue;
        Debug.Log($"Sprzedano fabryke '{f.name}' za {sellValue} (nowy count = {f.count})");
        LayoutController.Instance?.SetGoldText(raptorCore.Gold.ToString());
        return true;
    }

    public Factory GetFactory(int index)
    {
        if (factories != null && index >= 0 && index < factories.Count)
            return factories[index];
        return null;
    }

    public List<Factory> GetFactories()
    {
        return factories;
    }

    public Resource GetResource(int index)
    {
        if (resources != null && index >= 0 && index < resources.Count)
            return resources[index];
        return null;
    }

    public int GetFactoryCount()
    {
        return factories != null ? factories.Count : 0;
    }
}