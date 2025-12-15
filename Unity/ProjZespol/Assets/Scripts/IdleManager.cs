using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour
{
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private float tickInterval = 1f;
    private Coroutine tickCoroutine;
    [NonSerialized]public List<Factory> factories;
    [NonSerialized]public List<Resource> resources;
    [NonSerialized]public List<Potion> potions;
    public QuarkType potionFactoryBonus1 = 1;
    public QuarkType potionFactoryBonus2 = 1;
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
        resources.Add(new Resource("Resource4"));
        resources.Add(new Resource("Resource5"));
        resources.Add(new Resource("Resource6"));


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

        factories.Add(new Factory(resources[3], "F4", 15, 1.15, 15, 125000));

        factories.Add(new Factory(resources[4], "F5", 15, 1.15, 20, 625000));

        factories.Add(new Factory(resources[5], "F6", 15, 1.15, 25, 3125000));
    }

    private void InitPotions()
    {
        if (potions == null) potions = new List<Potion>();

        potions.Clear();

        potions.Add(new Potion("Uno", resources[0], resources[1], 1, 1,"click1", 20, "Zwiększa ilość zasobów za kliknięcie o 5 przez 20 sekund."));
        potions.Add(new Potion("Dos", resources[1], resources[2], 1, 1,"idle1", 20, "Zwiększa produkcję zasobów z fabryk o 100% przez 20 sekund."));
        potions.Add(new Potion("Tres", resources[0], resources[2], 1, 1,"sell1", 20, "Zwiększa wartość sprzedaży zasobów o 100% przez 20 sekund."));
        potions.Add(new Potion("Cuatro", resources[3], resources[4], 1, 1,"click2", 40, "Zwiększa ilość zasobów za kliknięcie o 10 przez 40 sekund."));
        potions.Add(new Potion("Cinco", resources[4], resources[5], 1, 1,"idle2", 40, "Zwiększa produkcję zasobów z fabryk o 200% przez 40 sekund."));
        potions.Add(new Potion("Seis", resources[3], resources[5], 1, 1,"sell2", 40, "Zwiększa wartość sprzedaży zasobów o 200% przez 40 sekund."));
        
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
                production *= potionFactoryBonus1 * characterClass.potionBoost;
            }
            
            if(potions[4].isActive && potions[4].linkedFactory.name == f.name)
            {
                production *= potionFactoryBonus2 * characterClass.potionBoost;
            }

            if (f.resource != null)
            {
                if (!GetFocusedFactory(f.resource.name) && characterClass.whatEyesDontSee)
                    f.resource.value += production / 2 + 100;
                if (GetFocusedFactory(f.resource.name) && characterClass.passiveAgressive && !characterClass.christmasBonus)
                    f.resource.value += characterClass.productionIdleAgressiveBonus;
                if (characterClass.passiveAgressive && characterClass.christmasBonus)
                    f.resource.value += characterClass.productionIdleAgressiveBonus;
                if (characterClass.hungryWolf)
                    f.resource.value += (production * (2 - (f.resource.value / f.resource.Limit))).Ceil();
                   

                if (characterClass.oneForEveryone)
                    f.resource.value += (QuarkType)(production + (GetUnlockedFactoryCount(f.resource.name) * (characterClass.productionIdleBonus + characterClass.productionIdlePedatorBonus)));
                else
                    f.resource.value += (QuarkType)(production + (GetUnlockedFactoryCount(f.resource.name) * characterClass.productionIdleBonus) + characterClass.productionIdlePedatorBonus);
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
        QuarkType resource1Amount, resource2Amount = null;


        switch((p.resourceType1.name, p.resourceType2.name))
        {
            case ("Resource1","Resource2"):
                resource1Amount = raptorCore.GetResourceValueDirect("Resource1");
                resource2Amount = raptorCore.GetResourceValueDirect("Resource2");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.SetResourceValueDirect("Resource1", resource1Amount - p.cost1);
                    raptorCore.SetResourceValueDirect("Resource2", resource2Amount - p.cost2);
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
            case ("Resource2","Resource3"):
                resource1Amount = raptorCore.GetResourceValueDirect("Resource2");
                resource2Amount = raptorCore.GetResourceValueDirect("Resource3");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.SetResourceValueDirect("Resource2", resource1Amount - p.cost1);
                    raptorCore.SetResourceValueDirect("Resource3", resource2Amount - p.cost2);
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
            case ("Resource1","Resource3"):
                resource1Amount = raptorCore.GetResourceValueDirect("Resource1");
                resource2Amount = raptorCore.GetResourceValueDirect("Resource3");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.SetResourceValueDirect("Resource1", resource1Amount - p.cost1);
                    raptorCore.SetResourceValueDirect("Resource3", resource2Amount - p.cost2);
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
            case ("Resource4","Resource5"):
                resource1Amount = raptorCore.GetResourceValueDirect("Resource4");
                resource2Amount = raptorCore.GetResourceValueDirect("Resource5");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.SetResourceValueDirect("Resource4", resource1Amount - p.cost1);
                    raptorCore.SetResourceValueDirect("Resource5", resource2Amount - p.cost2);
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
            case ("Resource4","Resource6"):
                resource1Amount = raptorCore.GetResourceValueDirect("Resource4");
                resource2Amount = raptorCore.GetResourceValueDirect("Resource6");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.SetResourceValueDirect("Resource4", resource1Amount - p.cost1);
                    raptorCore.SetResourceValueDirect("Resource6", resource2Amount - p.cost2);
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
            case ("Resource5","Resource6"):
                resource1Amount = raptorCore.GetResourceValueDirect("Resource5");
                resource2Amount = raptorCore.GetResourceValueDirect("Resource6");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.SetResourceValueDirect("Resource5", resource1Amount - p.cost1);
                    raptorCore.SetResourceValueDirect("Resource6", resource2Amount - p.cost2);
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
            case "click1":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    raptorCore.potionClickBonus1 += 5;
                }
                break;
            case "idle1":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    potionFactoryBonus1 *= 2;
                }
                break;
            case "sell1":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    raptorCore.potionSellBonus1 *= 2;
                }
                break;

            case "click2":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    raptorCore.potionClickBonus2 += 10;
                }
                break;
            case "idle2":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    potionFactoryBonus2 *= 4;
                }
                break;
            case "sell2":
                if(potion.linkedFactory.name == raptorCore.GetCurrentFactory().name)
                {
                    raptorCore.potionSellBonus2 *= 4;
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
            case "click1":
                raptorCore.potionClickBonus1 -= 5;
                break;
            case "idle1":
                potionFactoryBonus1 /= 2;
                break;
            case "sell1":
                raptorCore.potionSellBonus1 /= 2;
                break;
            case "click2":
                raptorCore.potionClickBonus2 -= 10;
                break;
            case "idle2":
                potionFactoryBonus2 /= 4;
                break;
            case "sell2":
                raptorCore.potionSellBonus2 /= 4;
                break;
            default:
                break;
        }
    }

    private int GetUnlockedFactoryCount(string resource)
    {
        if (raptorCore.currentResource != resource)
            return 0;
        int sum = 0;
        foreach(var f in factories)
        {
            if(f.isUnlocked)
                sum++;
        }
        return sum;
    }

    private bool GetFocusedFactory(string resource)
    {
        if (raptorCore.currentResource != resource)
            return false;
        return true;
    }

}