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
        factories.Add(new Factory(resources[0], "F1.1", 15, 1.15, 1));
        factories.Add(new Factory(resources[0], "F1.2", 100, 1.17, 10));
        factories.Add(new Factory(resources[0], "F1.3", 1000, 1.2, 50));

        factories.Add(new Factory(resources[1], "F2.1", 150, 1.13, 10));
        factories.Add(new Factory(resources[1], "F2.2", 2000, 1.16, 30));
        factories.Add(new Factory(resources[1], "F2.3", 5000, 1.2, 50));

        factories.Add(new Factory(resources[2], "F3.1", 1000, 1.15, 100));
        factories.Add(new Factory(resources[2], "F3.2", 3000, 1.18, 300));
        factories.Add(new Factory(resources[2], "F3.3", 8000, 1.22, 700));
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

    public bool BuyFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            Debug.LogWarning($"BuyFactory: nieprawidlowy indeks {index}");
            return false;
        }

        var f = factories[index];
        double cost = f.currentCost;
        if (raptorCore.Gold >= cost)
        {
            raptorCore.Gold -= cost;
            f.count++;
            Debug.Log($"Kupiono fabryke '{f.name}' (nowy count = {f.count}), koszt = {cost}");
            LayoutController.Instance?.SetCurrencyText(raptorCore.Currency.ToString());
            return true;
        }
        else
        {
            Debug.Log("BuyFactory: brak wystarczajacych srodkow");
            return false;
        }
    }

    public bool SellFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            Debug.LogWarning($"SellFactory: nieprawidlowy indeks {index}");
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