using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class RaptorCore : MonoBehaviour
{

    [AutoSave] public QuarkType Currency
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
    [AutoSave] QuarkType CBasevalue = 1;
    [AutoSave] QuarkType CMultiplier = 1;
    public QuarkType SkillMultiplier = 0;


    //Click based
    [AutoSave] QuarkType GBasevalue = 0;
    [AutoSave] QuarkType GMultiplier = 1;


    int tickCount = 0;

    [SerializeField] private Animator anim;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private GameObject skillCheck;
    [SerializeField] private IdleManager idleManager;
    [AutoSave] public double Gold = 0;
    private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

    [AutoSave] public QuarkType resource1Value = 0;
    [AutoSave] public QuarkType resource2Value = 0;
    [AutoSave] public QuarkType resource3Value = 0;
    public string currentResource = "Resource1";
    private enum ClickSource { None, Mouse, Space, Enter }
    private ClickSource activeClickSource = ClickSource.None;
    private float sourceBlockEndTime = 0f;
    private float clickCooldown = 0.5f; // jezeli gracz klika przycisk to wyłącza inne na czas cooldownu
    public QuarkType potionClickBonus = 0;
    public QuarkType potionSellBonus = 1;

    private bool skillCheckCooldown = false;
    int skillCheckCooldownCount = 0;

    private void Awake()
    {
        UpdateUI();
    }

    private void OnMouseDown()
    {
        if (Time.time >= sourceBlockEndTime) activeClickSource = ClickSource.None;
        if (activeClickSource != ClickSource.None && activeClickSource != ClickSource.Mouse && Time.time < sourceBlockEndTime) return;
        activeClickSource = ClickSource.Mouse;
        sourceBlockEndTime = Time.time + clickCooldown;
        PerformClick();
    }
    void click()
    {
        QuarkType value = 0;          
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        if (!skillCheckCooldown) SkillCheckManager();
        if (chance < characterClass.GetCriticalChance())
        {
            if(idleManager.potions[0].isActive && idleManager.potions[0].linkedFactory.name == GetCurrentFactory().name)
                    value += (((CBasevalue * CMultiplier * 3) + (CBasevalue * SkillMultiplier))) + potionClickBonus;
            else
            {
                value += ((CBasevalue * CMultiplier * 3) + (CBasevalue * SkillMultiplier)); //to do zmiany gdy będzie wchodzić temat balansu
            }

            if (characterClass.activeIdle)
                StartCoroutine(EnableActiveIdle());
            if (characterClass.noMatterWhat)
                characterClass.boostedChance = 0.00f;
            Debug.Log("Kryt " + chance);
        }
        else
        {
            if(idleManager.potions[0].isActive && idleManager.potions[0].linkedFactory.name == GetCurrentFactory().name)
                value += (((CBasevalue * CMultiplier) + (CBasevalue * SkillMultiplier))) + potionClickBonus;
            else
                value += ((CBasevalue * CMultiplier) + (CBasevalue * SkillMultiplier));
            if (characterClass.noMatterWhat)
                characterClass.boostedChance += 1.00f;
        }
        Currency += value;
        
    }

    void SkillCheckManager()
    {
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        float boostChance = 0;
        if (characterClass.symbiosis)
            boostChance = characterClass.GetCriticalChance();
        if (chance < characterClass.GetSkillCheckChance() + boostChance)
        {
            skillCheck.SetActive(true);
            skillCheck.GetComponent<SkillCheckScript>().StartSkillCheck();
            skillCheckCooldown = true;
        }      
    }

    void Start()
    {
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
        AutoSaveSystem.LoadGame();
        if (resources.ContainsKey("Resource1")) resources["Resource1"].value = resource1Value;
        if (resources.ContainsKey("Resource2")) resources["Resource2"].value = resource2Value;
        if (resources.ContainsKey("Resource3")) resources["Resource3"].value = resource3Value;
        UpdateUI();
    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier); //.Pow(GPower);

        if(skillCheckCooldown)
            skillCheckCooldownCount++;
        tickCount++;
        if (tickCount % 600 == 0)
            if(AutoSaveSystem.AutoSave)
                AutoSaveSystem.SaveGame();
        if (skillCheckCooldownCount % (600 - characterClass.skillCheckReduce) == 0)//zmienic przy balansie
        { 
            skillCheckCooldown = false;
            skillCheckCooldownCount = 0;
        }
    }

    void Update()
    {
        if (Time.time >= sourceBlockEndTime) activeClickSource = ClickSource.None;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (activeClickSource != ClickSource.None && activeClickSource != ClickSource.Space && Time.time < sourceBlockEndTime) return;

            activeClickSource = ClickSource.Space;
            sourceBlockEndTime = Time.time + clickCooldown;
            PerformClick();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (activeClickSource != ClickSource.None && activeClickSource != ClickSource.Enter && Time.time < sourceBlockEndTime)
                return;

            activeClickSource = ClickSource.Enter;
            sourceBlockEndTime = Time.time + clickCooldown;
            PerformClick();
            return;
        }
    }
    private void PerformClick()
    {
        if (!CanClickCurrentResource())
        {
            Debug.LogWarning($"Nie można klikać zasobu '{currentResource}' — fabryka nie jest odblokowana!");
            return;
        }
        if (!skillCheck.activeSelf) //nie lubie jak to wygląda, ale czasu nie ma broski
        {
            click();
            anim.SetTrigger("Clicked");
            UpdateUI();
        }
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

    public void AddCurrency(QuarkType currency)
    {
        Currency += currency;
        UpdateUI();
    }

    public void SubCurrency(QuarkType currency)
    {
        if (!characterClass.alwaysWinner)
        {
            if (Currency < currency)
                Currency = 0;
            else
                Currency -= currency;
        }
        UpdateUI();
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
            saveResourceValues();
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

    public QuarkType GetResourceValueDirect(string resourceName)
    {
        saveResourceValues();
        
        switch(resourceName)
        {
            case "Resource1":
                return resource1Value;
            case "Resource2":
                return resource2Value;
            case "Resource3":
                return resource3Value;
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
            saveResourceValues();

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
        }
        
        if (resourceName == currentResource)
        {
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

        double goldEarned = amountToSell * pricePerUnit * potionSellBonus;
        Gold += goldEarned;
        RemoveResource(resource, amountToSell);
        UpdateUI();
        return goldEarned;
    }

    private bool CanClickCurrentResource()
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
    public void saveResourceValues()
    {
        if (resources.ContainsKey("Resource1")) resource1Value = resources["Resource1"].value;
        if (resources.ContainsKey("Resource2")) resource2Value = resources["Resource2"].value;
        if (resources.ContainsKey("Resource3")) resource3Value = resources["Resource3"].value;
    }

    private IEnumerator EnableActiveIdle()
    {
        foreach (var f in idleManager.factories)
        {
            if (f.resource.name == currentResource)
            {
                f.productionMultiplier += 3;
                f.count += 4;
                yield return new WaitForSeconds(6f);
                f.productionMultiplier -= 3;
                f.count -= 4;
                break;
            }
        }       
    }

    public void EnableChickenDinner(bool effect)
    {
        if(characterClass.chickenDinner)
            StartCoroutine(ChickenDinnerEffect(effect));
    }

    public IEnumerator ChickenDinnerEffect(bool effect)
    {
        int mode = 1;
        if (!effect)
            mode = -1;
        if (!characterClass.alwaysWinner)
            mode = 0;
        SkillMultiplier += 10 * mode;
        yield return new WaitForSeconds(6f);
        SkillMultiplier -= 10 * mode;
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
}
