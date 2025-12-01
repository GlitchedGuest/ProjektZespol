using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
    [AutoSave] public QuarkType resource4Value = 0;
    [AutoSave] public QuarkType resource5Value = 0;
    [AutoSave] public QuarkType resource6Value = 0;

    [AutoSave] public QuarkType resource1Limit = 1000;
    [AutoSave] public QuarkType resource2Limit = 2500;
    [AutoSave] public QuarkType resource3Limit = 5000;
    [AutoSave] public QuarkType resource4Limit = 10000;
    [AutoSave] public QuarkType resource5Limit = 25000;
    [AutoSave] public QuarkType resource6Limit = 50000;
    public string currentResource = "Resource1";
    private enum ClickSource { None, Mouse, Space, Enter }
    private ClickSource activeClickSource = ClickSource.None;
    private float sourceBlockEndTime = 0f;
    private float clickCooldown = 0.5f; // jezeli gracz klika przycisk to wyłącza inne na czas cooldownu
    public QuarkType potionClickBonus = 0;
    public QuarkType potionSellBonus = 1;

    //Crit Generator
    [SerializeField] private GameObject CritGenerator;
    private CritVisualGenerator critGen;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource critSource;
    private void Awake()
    {
        critGen = CritGenerator.GetComponent<CritVisualGenerator>();
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
            Vector2 mousePos = Input.mousePosition;
            critGen.SpawnCrit(mousePos);
            critSource.Stop();
            critSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.Play();
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
        }      
    }

    void LoadResource(string name)
    {
        if (!resources.TryGetValue(name, out var res)) return;

        string p = char.ToLower(name[0]) + name[1..];
        var t = GetType();

        res.Limit = (QuarkType)t.GetField(p + "Limit").GetValue(this);
        res.value = (QuarkType)t.GetField(p + "Value").GetValue(this);
    }



    void Start()
    {
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
        AutoSaveSystem.LoadGame();
        LoadResource("Resource1");
        LoadResource("Resource2");
        LoadResource("Resource3");
        LoadResource("Resource4");
        LoadResource("Resource5");
        LoadResource("Resource6");
        Gold = 10000000000000000000000000000000.0f;
        UpdateUI();

    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier); //.Pow(GPower);

        tickCount++;
        if (tickCount % 600 == 0)
            if(AutoSaveSystem.AutoSave)
                AutoSaveSystem.SaveGame();
        if (tickCount % (600-characterClass.skillCheckReduce) == 0) //zmienic przy balansie
            SkillCheckManager();  
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
            case "Resource4":
                resource3Value = value;
                if (resources.ContainsKey("Resource4"))
                    resources["Resource4"].value = value;
                break;
            case "Resource5":
                resource3Value = value;
                if (resources.ContainsKey("Resource5"))
                    resources["Resource5"].value = value;
                break;
            case "Resource6":
                resource3Value = value;
                if (resources.ContainsKey("Resource6"))
                    resources["Resource6"].value = value;
                break;
        }
        
        if (resourceName == currentResource)
        {
            UpdateUI();
        }
    }
    public void SetLimitResource(string resource, QuarkType limitvalue) {
        if (resources.ContainsKey(resource))
        {
            resources[resource].Limit = limitvalue;
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

    //Incirement + add in to limit
    public void IncrementLimitResourceAll(QuarkType AddValue)
    {
        IncrementLimitResource("Resource1", AddValue);
        IncrementLimitResource("Resource2", AddValue);
        IncrementLimitResource("Resource3", AddValue);
        IncrementLimitResource("Resource4", AddValue);
        IncrementLimitResource("Resource5", AddValue);
        IncrementLimitResource("Resource6", AddValue);
    }

    //Incirement * multiply the limit
    public void MullLimitResourceAll(QuarkType MullValue)
    {
        SetLimitResource("Resource1", MullValue * GetLimitResource("Resource1"));
        SetLimitResource("Resource2", MullValue * GetLimitResource("Resource2"));
        SetLimitResource("Resource3", MullValue * GetLimitResource("Resource3"));
        SetLimitResource("Resource4", MullValue * GetLimitResource("Resource4"));
        SetLimitResource("Resource5", MullValue * GetLimitResource("Resource5"));
        SetLimitResource("Resource6", MullValue * GetLimitResource("Resource6"));
    }

    public void IncrementLimitResource(string resource, QuarkType AddValue)
    {
        if (resources.ContainsKey(resource))
        {
            resources[resource].Limit += AddValue;
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
