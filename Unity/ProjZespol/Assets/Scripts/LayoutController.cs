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

    private HashSet<string> learnedOneClickArmy = new();
    private HashSet<string> learnedJackOfAllClicks = new();
    private HashSet<string> learnedAutomatron = new();

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

        potionUIManager = new PotionUIManager(ui, raptorCore, idleManager, audioSource);
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
    }

    public void SetCurrencyText(string value)
    {
        resourceShopManager?.SetCurrencyText(value);
    }

    public void SetGoldText(string value)
    {
        resourceShopManager?.SetGoldText(value);
    }
    public void UpdateUI()
    {
        QuarkType currentResourceAmount = raptorCore.ResourceManager.GetResourceValue(raptorCore.ResourceManager.currentResource);
        LayoutController.Instance?.SetCurrencyText(currentResourceAmount.ToString());
        LayoutController.Instance?.SetGoldText(raptorCore.Gold.ToString());
    }
}