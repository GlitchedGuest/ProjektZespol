using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LayoutController : MonoBehaviour
{
    public static LayoutController Instance { get; private set; }
    public VisualTreeAsset tooltipAsset;

    [SerializeField] private SpriteRenderer clickerObject;
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private IdleManager idleManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource critSource;
    [SerializeField] private SkillPointsLimit skillPointsLimit;

    public Sprite[] images;
    public VisualElement ui;

    private FactoryUIManager factoryUIManager;
    private PotionUIManager potionUIManager;
    private OptionsManager optionsManager;
    private LevelUIManager levelUIManager;
    private ContentPageManager contentPageManager;
    private ResourceShopManager resourceShopManager;
    private SkillTreesUIManager skillTreesUIManager;
    public  PrestigeUIManager prestigeUIManager;
        
    private BuffManager buffManager;

    private HashSet<string> learnedOneClickArmy = new();
    private HashSet<string> learnedJackOfAllClicks = new();
    private HashSet<string> learnedAutomatron = new();


    public void ResetGui()
    {
        skillTreesUIManager.ClearTrees();
        factoryUIManager.ResetFactoryGUI();
        skillPointsLimit.UpdateButton();
        UpdateUI();
    }
    [SerializeField] private Texture2D[] bufficons;


    

    void Awake()
    {
        Instance = this;
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        InitializeManagers();
        InitializeTooltips();
        
    }

    private void InitializeManagers()
    {
        factoryUIManager = new FactoryUIManager(ui, raptorCore, idleManager, audioSource, clickerObject, images);
        factoryUIManager.Initialize();

        potionUIManager = new PotionUIManager(ui, raptorCore, idleManager, audioSource, characterClass);
        potionUIManager.Initialize();

        optionsManager = new OptionsManager(ui, audioSource, critSource);
        optionsManager.Initialize();

        levelUIManager = new LevelUIManager(ui, characterClass);
        levelUIManager.Initialize();

        contentPageManager = new ContentPageManager(ui, audioSource);
        contentPageManager.Initialize();

        resourceShopManager = new ResourceShopManager(ui, raptorCore, characterClass, audioSource);
        resourceShopManager.Initialize();

        skillTreesUIManager = new SkillTreesUIManager(ui, characterClass, learnedOneClickArmy, learnedJackOfAllClicks, learnedAutomatron, skillPointsLimit);
        skillTreesUIManager.InitializeAllTrees();

        prestigeUIManager = new PrestigeUIManager(ui, audioSource, raptorCore);
        prestigeUIManager.Initialize();

        buffManager = new BuffManager(ui,bufficons);
        characterClass.buffManager = buffManager;
    }

    private void InitializeTooltips()
    {
        Tooltip.Init(ui, tooltipAsset);
        ui.RegisterCallback<GeometryChangedEvent>(Tooltip.SizeRefresh);
    }

    public void Update()
    {
        factoryUIManager?.Update();
        potionUIManager?.Update();
        levelUIManager?.Update();
        resourceShopManager?.Update();
        skillTreesUIManager?.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionsManager.EnableOptions();
        }
        
    }

    public void SetCurrencyText(List<QuarkType> lista)
    {
        resourceShopManager?.SetCurrencyText(lista);
    }

    public void SetGoldText(string value)
    {
        resourceShopManager?.SetGoldText(value);
    }
    public void UpdateUI()
    {
        QuarkType Resource1 = raptorCore.ResourceManager.GetResourceValue("Resource1");
        QuarkType Resource2 = raptorCore.ResourceManager.GetResourceValue("Resource2");
        QuarkType Resource3 = raptorCore.ResourceManager.GetResourceValue("Resource3");
        QuarkType Resource4 = raptorCore.ResourceManager.GetResourceValue("Resource4");
        QuarkType Resource5 = raptorCore.ResourceManager.GetResourceValue("Resource5");
        QuarkType Resource6 = raptorCore.ResourceManager.GetResourceValue("Resource6");
        List<QuarkType> lista = new List<QuarkType>() { Resource1,Resource2,Resource3,Resource4,Resource5,Resource6};
        LayoutController.Instance?.SetCurrencyText(lista);
        LayoutController.Instance?.SetGoldText(raptorCore.Gold.ToString());
    }


}