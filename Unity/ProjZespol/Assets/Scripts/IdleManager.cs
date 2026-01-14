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
    private void Start()
    {
        if (raptorCore == null)
            raptorCore = FindAnyObjectByType<RaptorCore>();
        InitResources();
        InitFactories();
        InitPotions();
    }

    private void InitResources()
    {
        if (resources == null) resources = new List<Resource>();

        resources.Clear();
        resources.Add(new Resource("Resource1","Blood Rock Of Night"));
        resources.Add(new Resource("Resource2", "Hot Metal Of Lost World"));
        resources.Add(new Resource("Resource3", "Light Crystal"));
        resources.Add(new Resource("Resource4", "Killer Flower Of Earth"));
        resources.Add(new Resource("Resource5", "Red Diamond Of Telor"));
        resources.Add(new Resource("Resource6", "Time Remnant"));


        foreach (var res in resources)
        {
            raptorCore.ResourceManager.RegisterResource(res);
        }
        raptorCore.ResourceManager.LoadAllResources();
    }
    private void InitFactories()
    {
        if (factories == null) factories = new List<Factory>();

        factories.Clear();

        //TODO: Adjust factory parameters as needed
        factories.Add(new Factory(resources[0], "Astral Rock of Solitude", 15, 1.15, 1, 0));

        factories.Add(new Factory(resources[1], "Fields of War", 15, 1.15, 5, 5000));

        factories.Add(new Factory(resources[2], "The Last Forest", 15, 1.15, 10, 25000));

        factories.Add(new Factory(resources[3], "Dead Rises", 15, 1.15, 15, 125000));

        factories.Add(new Factory(resources[4], "City of Dreams", 15, 1.15, 20, 625000));

        factories.Add(new Factory(resources[5], "Hell", 15, 1.15, 25, 3125000));
    }

    private void InitPotions()
    {
        if (potions == null) potions = new List<Potion>();

        potions.Clear();

        potions.Add(new Potion("Potion of Clicking", resources[0], resources[1], 1, 1,"click1", 20, "Increases amount of resources per click by 5 for 20 seconds."));
        potions.Add(new Potion("Potion of Idle", resources[1], resources[2], 1, 1,"idle1", 20, "Increases factory production by 100% for 20 seconds."));
        potions.Add(new Potion("Potion of Profit", resources[0], resources[2], 1, 1,"sell1", 20, "Increases selling prices by 100% for 20 seconds."));
        potions.Add(new Potion("Potion of Clicking II", resources[3], resources[4], 1, 1,"click2", 40, "Increases amount of resources per click by 10 for 40 seconds."));
        potions.Add(new Potion("Potion of Idle II", resources[4], resources[5], 1, 1,"idle2", 40, "Increases factory production by 200% for 40 seconds."));
        potions.Add(new Potion("Potion of Profit II", resources[3], resources[5], 1, 1,"sell2", 40, "Increases selling prices by 200% for 40 seconds."));
        
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
                    p.boughtCount = 0;
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

        string currentResource = raptorCore.ResourceManager.currentResource;
        QuarkType Resource1 = raptorCore.ResourceManager.GetResourceValue("Resource1");
        QuarkType Resource2 = raptorCore.ResourceManager.GetResourceValue("Resource2");
        QuarkType Resource3 = raptorCore.ResourceManager.GetResourceValue("Resource3");
        QuarkType Resource4 = raptorCore.ResourceManager.GetResourceValue("Resource4");
        QuarkType Resource5 = raptorCore.ResourceManager.GetResourceValue("Resource5");
        QuarkType Resource6 = raptorCore.ResourceManager.GetResourceValue("Resource6");
        List<QuarkType> lista = new List<QuarkType>() { Resource1, Resource2, Resource3, Resource4, Resource5, Resource6 };
        LayoutController.Instance?.SetCurrencyText(lista);

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
                resource1Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource1");
                resource2Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource2");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource1", resource1Amount - p.cost1);
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource2", resource2Amount - p.cost2);
                }
                else
                {
                    return false;
                }
                p.boughtCount++;
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
                EnablePotionEffect(p);
                FindObjectOfType<ActivePotionsUI>()?.RefreshPanel();
                return true;
            case ("Resource2","Resource3"):
                resource1Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource2");
                resource2Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource3");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource2", resource1Amount - p.cost1);
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource3", resource2Amount - p.cost2);
                }
                else
                {
                    return false;
                }
                p.boughtCount++;
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
                EnablePotionEffect(p);
                FindObjectOfType<ActivePotionsUI>()?.RefreshPanel();
                return true;
            case ("Resource1","Resource3"):
                resource1Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource1");
                resource2Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource3");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource1", resource1Amount - p.cost1);
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource3", resource2Amount - p.cost2);
                }
                else
                {
                    return false;
                }
                p.boughtCount++;
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
                EnablePotionEffect(p);
                FindObjectOfType<ActivePotionsUI>()?.RefreshPanel();
                return true;
            case ("Resource4","Resource5"):
                resource1Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource4");
                resource2Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource5");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource4", resource1Amount - p.cost1);
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource5", resource2Amount - p.cost2);
                }
                else
                {
                    return false;
                }
                p.boughtCount++;
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
                EnablePotionEffect(p);
                FindObjectOfType<ActivePotionsUI>()?.RefreshPanel();
                return true;
            case ("Resource4","Resource6"):
                resource1Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource4");
                resource2Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource6");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource4", resource1Amount - p.cost1);
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource6", resource2Amount - p.cost2);
                }
                else
                {
                    return false;
                }
                p.boughtCount++;
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
                EnablePotionEffect(p);
                FindObjectOfType<ActivePotionsUI>()?.RefreshPanel();
                return true;
            case ("Resource5","Resource6"):
                resource1Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource5");
                resource2Amount = raptorCore.ResourceManager.GetResourceValueDirect("Resource6");
                if((resource1Amount >= p.cost1) && (resource2Amount >= p.cost2))
                {
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource5", resource1Amount - p.cost1);
                    raptorCore.ResourceManager.SetResourceValueDirect("Resource6", resource2Amount - p.cost2);
                }
                else
                {
                    return false;
                }
                p.boughtCount++;
                p.isActive = true;
                p.timeRemaining = p.duration;
                p.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
                EnablePotionEffect(p);
                FindObjectOfType<ActivePotionsUI>()?.RefreshPanel();
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
                if(potion.linkedFactory.name == raptorCore.ResourceManager.GetCurrentFactory().name)
                {
                    raptorCore.potionClickBonus1 += 5;
                }
                break;
            case "idle1":
                if(potion.linkedFactory.name == raptorCore.ResourceManager.GetCurrentFactory().name)
                {
                    potionFactoryBonus1 *= 2;
                }
                break;
            case "sell1":
                if(potion.linkedFactory.name == raptorCore.ResourceManager.GetCurrentFactory().name)
                {
                    raptorCore.SellManager.potionSellBonus1 *= 2;
                }
                break;

            case "click2":
                if(potion.linkedFactory.name == raptorCore.ResourceManager.GetCurrentFactory().name)
                {
                    raptorCore.potionClickBonus2 += 10;
                }
                break;
            case "idle2":
                if(potion.linkedFactory.name == raptorCore.ResourceManager.GetCurrentFactory().name)
                {
                    potionFactoryBonus2 *= 4;
                }
                break;
            case "sell2":
                if(potion.linkedFactory.name == raptorCore.ResourceManager.GetCurrentFactory().name)
                {
                    raptorCore.SellManager.potionSellBonus2 *= 4;
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
                raptorCore.potionClickBonus1 = 0;
                break;
            case "idle1":
                potionFactoryBonus1 = 1;
                break;
            case "sell1":
                raptorCore.SellManager.potionSellBonus1 = 1;
                break;
            case "click2":
                raptorCore.potionClickBonus2 = 0;
                break;
            case "idle2":
                potionFactoryBonus2 = 1;
                break;
            case "sell2":
                raptorCore.SellManager.potionSellBonus2 = 1;
                break;
            default:
                break;
        }
    }

    private int GetUnlockedFactoryCount(string resource)
    {
        if (raptorCore.ResourceManager.currentResource != resource)
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
        if (raptorCore.ResourceManager.currentResource != resource)
            return false;
        return true;
    }

    internal void ResetFactory()
    {
        InitFactories();
    }
}