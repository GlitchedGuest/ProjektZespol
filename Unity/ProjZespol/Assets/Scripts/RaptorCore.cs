using System;
using System.Xml.Serialization;
using System.Collections.Generic;
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
    private string currentResource = "Resource1";
    private enum ClickSource { None, Mouse, Space, Enter }
    private ClickSource activeClickSource = ClickSource.None;
    private float sourceBlockEndTime = 0f;
    private float clickCooldown = 0.5f; // jezeli gracz klika przycisk to wyłącza inne na czas cooldownu

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
            SkillCheckManager();//bardzo temp rozwiązanie później raczej losowo w czasie będzie sie skill check pojawiać, a nie podczas klikania w obiekt
            float chance = UnityEngine.Random.Range(0.00f, 100.00f);
            if (chance < characterClass.GetCriticalChance())
            {
                value += (CBasevalue * CMultiplier * 3); //to do zmiany gdy będzie wchodzić temat balansu
                Debug.Log("Kryt " + chance);
            }
            else
                value = (CBasevalue * CMultiplier);
            Currency += value;
        
    }

    void SkillCheckManager()
    {
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        if (chance < characterClass.GetSkillCheckChance())
        {
            skillCheck.SetActive(true);
            skillCheck.GetComponent<SkillCheckScript>().StartSkillCheck();
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

        tickCount++;
        if (tickCount % 600 == 0)
            AutoSaveSystem.SaveGame();
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
        if (Currency < currency)
            Currency = 0;
        else
            Currency -= currency;
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
}
