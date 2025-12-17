using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RaptorCore : MonoBehaviour
{
    // Publiczne referencje do managerów - inne skrypty mogą ich używać bezpośrednio
    public ResourceManager ResourceManager { get; private set; }
    public SellingManager SellManager { get; private set; }
    public SkillManager SkillManager { get; private set; }
    [SerializeField] private Animator anim;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private GameObject skillCheck;
    [SerializeField] private IdleManager idleManager;

    //Crit Generator
    [SerializeField] private GameObject CritGenerator;
    private CritVisualGenerator critGen;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource critSource;

    [AutoSave] public QuarkType Currency
    {
        get => ResourceManager.GetResourceValue(ResourceManager.currentResource);
        set
        {
            ResourceManager.SetResourceValueDirect(ResourceManager.currentResource, value);
        }
    }

    //Click based
    [AutoSave] QuarkType CBasevalue = 1;
    [AutoSave] QuarkType CMultiplier = 1;
    public QuarkType SkillMultiplier = 0;

    //Idle/Generator based
    [AutoSave] QuarkType GBasevalue = 0;
    [AutoSave] QuarkType GMultiplier = 1;

    int tickCount = 0;
    
    [AutoSave] public double Gold = 0;

    public QuarkType potionClickBonus1 = 0;
    public QuarkType potionClickBonus2 = 0;

    private enum ClickSource { None, Mouse, Space, Enter }
    private ClickSource activeClickSource = ClickSource.None;
    private float sourceBlockEndTime = 0f;
    private float clickCooldown = 0.5f;

    public int clickingDebuff = 0;


    private void Awake()
    {
        ResourceManager = gameObject.AddComponent<ResourceManager>();
        SellManager = gameObject.AddComponent<SellingManager>();
        SkillManager = gameObject.AddComponent<SkillManager>();

                
        critGen = CritGenerator.GetComponent<CritVisualGenerator>();
    }

    private void Start()
    {
        ResourceManager.Initialize(idleManager);
        SellManager.Initialize(this, ResourceManager, idleManager, characterClass);
        SkillManager.Initialize(this, characterClass, idleManager, skillCheck);
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
        AutoSaveSystem.LoadGame();
        LayoutController.Instance?.UpdateUI();
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
        
        SkillManager.CheckAndTriggerSkillCheck();
        
        if (chance < characterClass.GetCriticalChance())
        {
            Factory currentFactory = ResourceManager.GetCurrentFactory();
            
            if(idleManager.potions[0].isActive && idleManager.potions[0].linkedFactory.name == currentFactory.name)
                value += potionClickBonus1 * characterClass.potionBoost;
            if(idleManager.potions[3].isActive && idleManager.potions[3].linkedFactory.name == currentFactory.name)
                value += potionClickBonus2 * characterClass.potionBoost;

            value += ((CBasevalue * CMultiplier * 3) + (CBasevalue * SkillMultiplier));

            if (characterClass.activeIdle)
                StartCoroutine(SkillManager.EnableActiveIdle());
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

            Factory currentFactory = ResourceManager.GetCurrentFactory();
            
            if(idleManager.potions[0].isActive && idleManager.potions[0].linkedFactory.name == currentFactory.name)
                value += potionClickBonus1 * characterClass.potionBoost;
            if(idleManager.potions[3].isActive && idleManager.potions[3].linkedFactory.name == currentFactory.name)
                value += potionClickBonus2 * characterClass.potionBoost;
            value += ((CBasevalue * CMultiplier) + (CBasevalue * SkillMultiplier));

            if (characterClass.noMatterWhat)
                characterClass.boostedChance += 1.00f;
        }
        Currency += value;
    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier);

        SkillManager.UpdateSkillCheckCooldown();
        
        tickCount++;
        
        if (tickCount % 600 == 0)
            if(AutoSaveSystem.AutoSave)
                AutoSaveSystem.SaveGame();
        
        SkillManager.CheckReactionTest(tickCount);
        
        if (characterClass.passiveAgressive)
            if (tickCount % 300 == 0)
            {
                characterClass.productionIdleAgressiveBonus += 10 - clickingDebuff;
                clickingDebuff = 0;
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
            if (activeClickSource != ClickSource.None && activeClickSource != ClickSource.Enter && Time.time < sourceBlockEndTime) return;

            activeClickSource = ClickSource.Enter;
            sourceBlockEndTime = Time.time + clickCooldown;
            PerformClick();
            return;
        }
    }

    private void PerformClick()
    {
        if (!ResourceManager.CanClickCurrentResource())
        {
            Debug.LogWarning($"Nie można klikać zasobu '{ResourceManager.currentResource}' - fabryka nie jest odblokowana!");
            return;
        }
        
        if (!SkillManager.IsSkillCheckActive())
        {
            click();
            anim.SetTrigger("Clicked");
            if(characterClass.passiveAgressive)
                characterClass.productionIdleAgressiveBonus = 0;
            if (characterClass.multitasking)
                clickingDebuff = 2;
            LayoutController.Instance?.UpdateUI();
        }
    }

    public void AddCurrency(QuarkType currency)
    {
        Currency += currency;
        LayoutController.Instance?.UpdateUI();
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
        LayoutController.Instance?.UpdateUI();
    }
    

}
