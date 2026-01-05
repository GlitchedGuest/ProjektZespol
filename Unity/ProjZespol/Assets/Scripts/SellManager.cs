
using System;
using UnityEngine;

public class SellingManager : MonoBehaviour
{
    private RaptorCore raptorCore;
    private ResourceManager resourceManager;
    private IdleManager idleManager;
    private CharacterClass characterClass;

    public QuarkType potionSellBonus1 = 1;
    public QuarkType potionSellBonus2 = 1;

    public void Initialize(RaptorCore raptorCore, ResourceManager resManager, IdleManager idle, CharacterClass character)
    {
        this.raptorCore = raptorCore;
        resourceManager = resManager;
        idleManager = idle;
        characterClass = character;
    }

    private void Update()
    {
        if (characterClass.michealScott)
        {
            for(int i = 1; i<=6; i++)
            {
                QuarkType amount = resourceManager.GetResourceValue("Resource"+i);
                QuarkType limit = resourceManager.GetLimitResource("Resource" + i);
                if (amount == limit)
                    characterClass.GainExp((ulong)SellResource("Resource" + i, 100f, i));
            }
        }
    }

    public double SellMaterials(float sellValue)
    {
        string currentResource = resourceManager.currentResource;
        
        switch(currentResource)
        {
            case "Resource1":
                return SellResource("Resource1", sellValue, 1.0);
            case "Resource2":
                return SellResource("Resource2", sellValue, 2.0);
            case "Resource3":
                return SellResource("Resource3", sellValue, 3.0);
            case "Resource4":
                return SellResource("Resource4", sellValue, 4.0);
            case "Resource5":
                return SellResource("Resource5", sellValue, 5.0);
            case "Resource6":
                return SellResource("Resource6", sellValue, 6.0);
            default:
                return 0;
        }
    }

    public double SellResource(string resource, float sellvalue, double pricePerUnit = 1.0)
    {
        QuarkType currentAmount = resourceManager.GetResourceValue(resource);
        var amountToSell = (currentAmount * (sellvalue / 100f)).Ceil();

        if (amountToSell <= 0)
        {
            return 0;
        }

        double goldEarned = amountToSell * pricePerUnit;
        
        Factory currentFactory = raptorCore.ResourceManager.GetCurrentFactory();
        
        if(idleManager.potions[2].isActive && idleManager.potions[2].linkedFactory.name == currentFactory.name)
        {
            goldEarned *= potionSellBonus1 * characterClass.potionBoost;
        }
        if(idleManager.potions[5].isActive && idleManager.potions[5].linkedFactory.name == currentFactory.name)
        {
            goldEarned *= potionSellBonus2 * characterClass.potionBoost;
        }

        raptorCore.Gold += Math.Ceiling(goldEarned * characterClass.sellingBonus * characterClass.sellingHardBonus);
        resourceManager.RemoveResource(resource, amountToSell);
        LayoutController.Instance?.UpdateUI();
        return goldEarned;
    }
}