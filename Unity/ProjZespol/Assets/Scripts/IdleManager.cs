using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour
{
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private float tickInterval = 1f;
    private Coroutine tickCoroutine;
    public List<Factory> factories;
    public List<Resource> resources;
    public List<Potion> potions;
    public QuarkType potionFactoryBonus = 0;
    private void Awake()
    {
        InitResources();
        InitFactories();
        InitPotions();
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
        factories.Add(new Factory(resources[0], "F1", 15, 1.15, 1, 0));

        factories.Add(new Factory(resources[1], "F2", 15, 1.15, 5, 5000));

        factories.Add(new Factory(resources[2], "F3", 15, 1.15, 10, 25000));
    }

    private void InitPotions()
    {
        if (potions == null) potions = new List<Potion>();

        potions.Clear();

        potions.Add(new Potion("Uno", resources[0], 1, "click", 20, "Zwiększa ilość zasobów za kliknięcie o 5 przez 20 sekund."));
        potions.Add(new Potion("Dos", resources[1], 1, "idle", 20, "Zwiększa produkcję zasobów z fabryk o 100% przez 20 sekund."));
        potions.Add(new Potion("Tres", resources[2], 1, "sell", 20, "Zwiększa wartość sprzedaży zasobów o 100% przez 20 sekund."));
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

        foreach (var p in potions)
        {
            if (p.isActive)
            {
                p.timeRemaining -= tickInterval;
                if (p.timeRemaining <= 0)
                {
                    p.isActive = false;
                    p.timeRemaining = 0;
                    DisablePotionEffect(p);
                }
            }
        }

        foreach (var f in factories)
        {
            if (f == null || f.count <= 0) continue;
            double production = f.GetProduction();
            if(potions[1].isActive && potions[1].linkedFactory.name == f.name)
            {
                production = production * potionFactoryBonus;
            }
            if (f.resource != null)
            {
                f.resource.value += (QuarkType)production;
            }
        }

        string currentResource = raptorCore.currentResource;
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

    public int GetFactoryCount()
    {
        return factories != null ? factories.Count : 0;
    }

    public Potion GetPotion(int index)
    {
        if (potions != null && index >= 0 && index < potions.Count)
            return potions[index];
        return null;
    }
    public bool BuyPotion(int index)
    {
        if (potions == null || index < 0 || index >= potions.Count)
        {
            return false;
        }

        var p = potions[index];
        
        switch(p.resourceType.name)
        {
            case "Resource1":
                QuarkType resource1Amount = raptorCore.GetResourceValueDirect("Resource1");
                if(resource1Amount >= p.cost)
                {
                    raptorCore.SetResourceValueDirect("Resource1", resource1Amount - p.cost);
                }
                else
                {
                    return false;
                }
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.GetCurrentFactory();
                EnablePotionEffect(p);
                return true;
            case "Resource2":
                QuarkType resource2Amount = raptorCore.GetResourceValueDirect("Resource2");
                if(resource2Amount >= p.cost)
                {
                    raptorCore.SetResourceValueDirect("Resource1", resource2Amount - p.cost);
                }
                else
                {
                    return false;
                }
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.GetCurrentFactory();
                EnablePotionEffect(p);
                return true;
            case "Resource3":
                QuarkType resource3Amount = raptorCore.GetResourceValueDirect("Resource3");
                if(resource3Amount >= p.cost)
                {
                    raptorCore.SetResourceValueDirect("Resource1", resource3Amount - p.cost);
                }
                else
                {
                    return false;
                }
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.GetCurrentFactory();
                EnablePotionEffect(p);
                return true;
            default:
                return false;
        }
    }
    public void EnablePotionEffect(Potion potion)
    {
        if (potion == null || !potion.isActive) return;
        switch (potion.effect)
        {
            case "click":
                Debug.Log("Enabling click potion effect");
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    raptorCore.potionClickBonus = 5;
                }
                else
                {
                    raptorCore.potionClickBonus = 0;
                }
                break;
            case "idle":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    potionFactoryBonus = 2;
                }
                else
                {
                    potionFactoryBonus = 0;
                }
                break;
            case "sell":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    raptorCore.potionSellBonus = 2;
                }
                else
                {
                    raptorCore.potionSellBonus = 1;
                }
                break;
            default:
                break;
        }
    }
    public void DisablePotionEffect(Potion potion)
    {
        if (potion == null) return;
        potion.isActive = false;
        potion.linkedFactory = null;
        switch (potion.effect)
        {
            case "click":
                raptorCore.potionClickBonus = 0;
                break;
            case "idle":
                potionFactoryBonus = 0;
                break;
            case "sell":
                raptorCore.potionSellBonus = 1;
                break;
            default:
                break;
        }
    }
}