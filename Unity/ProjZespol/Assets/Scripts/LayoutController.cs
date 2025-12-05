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

    public Sprite[] images;
    public VisualElement ui;

    private FactoryUIManager factoryUIManager;
    private PotionUIManager potionUIManager;
    private OptionsManager optionsManager;
    private LevelUIManager levelUIManager;
    private ContentPageManager contentPageManager;
    private ResourceShopManager resourceShopManager;

    // Skill Trees
    private SkillTreeManager oneClickArmyTree;
    private SkillTreeManager jackTree;
    private SkillTreeManager automatronTree;

    // Learned skills sets (współdzielone z drzewkami)
    private HashSet<string> learnedOneClick = new();
    private HashSet<string> learnedJack = new();
    private HashSet<string> learnedAutomatron = new();

    void Awake()
    {
        Instance = this;
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        InitializeManagers();
        InitializeSkillTrees();
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
    }

    private void InitializeSkillTrees()
    {
        oneClickArmyTree = new OneClickArmySkillTree(ui, characterClass, learnedOneClick);
        InitializeOneClickArmyTree();

        jackTree = new JackSkillTree(ui, characterClass, learnedJack);
        InitializeJackTree();

        automatronTree = new AutomatronSkillTree(ui, characterClass);
        InitializeAutomatronTree();
    }

    private void InitializeOneClickArmyTree()
    {
        string[] skills = { "Skill1", "Skill2A", "Skill2B", "Skill3A", "Skill3B", "Skill4A", "Skill4B", "Skill5", "Skill6A", "Skill6B" };
        oneClickArmyTree.InitializeSkills(skills);

        // Register tooltips
        oneClickArmyTree.RegisterTooltip("Skill1", "Skill Based Clicking", "Umożliwia pojawienie się skill checków(nie udany kosztuje gracza zwolnieniem idle produkcji)");
        oneClickArmyTree.RegisterTooltip("Skill2A", "Critical Mass", "Zwiększa szanse na kliki krytyczne");
        oneClickArmyTree.RegisterTooltip("Skill2B", "Chicken Dinner", "Udany skill check zwiększa ilość zbieranych punktów na chwilę(nie udany zmniejsza)");
        oneClickArmyTree.RegisterTooltip("Skill3A", "Active Idle", "Każdy klik krytyczny chwilowo zwiększa idle produkcje");
        oneClickArmyTree.RegisterTooltip("Skill3B", "Hungry For More", "Skill checki częściej się pojawiają");
        oneClickArmyTree.RegisterTooltip("Skill4A", "No Matter What", "Stakuje szanse na klik krytyczny(każde kliknięcie niekrytyczne zwiększa szanse na krytyczne)");
        oneClickArmyTree.RegisterTooltip("Skill4B", "Always Winner", "Skill checki nie mają negatywnych skutków po przegraniu");
        oneClickArmyTree.RegisterTooltip("Skill5", "Symbiosis", "Częstotliwość skill checka jest zależna od ilości klików krytycznych(im częściej są tym częściej skill checki)");
        oneClickArmyTree.RegisterTooltip("Skill6A", "Mortal Clicker", "Każdy kolejny skill check łączy się w kombos. Im większy kombos tym więcej punktów za klik(im większy kombos też trudniejsze skill checki)");
        oneClickArmyTree.RegisterTooltip("Skill6B", "Champion Of Clicks", "Po trzech udanych skill checkach z rzędu przez krótki moment są same kliki krytyczne");

        // Draw lines
        Color lineColor = Color.yellow;
        oneClickArmyTree.DrawLine("Skill1", "Skill2A", lineColor);
        oneClickArmyTree.DrawLine("Skill1", "Skill2B", lineColor);
        oneClickArmyTree.DrawLine("Skill2A", "Skill3A", lineColor);
        oneClickArmyTree.DrawLine("Skill2B", "Skill3B", lineColor);
        oneClickArmyTree.DrawLine("Skill3A", "Skill4A", lineColor);
        oneClickArmyTree.DrawLine("Skill3B", "Skill4B", lineColor);
        oneClickArmyTree.DrawLine("Skill4A", "Skill5", lineColor);
        oneClickArmyTree.DrawLine("Skill4B", "Skill5", lineColor);
        oneClickArmyTree.DrawLine("Skill5", "Skill6A", lineColor);
        oneClickArmyTree.DrawLine("Skill5", "Skill6B", lineColor);
    }

    private void InitializeJackTree()
    {
        string[] skills = { "Skill1-tree2", "Skill2A-tree2", "Skill2B-tree2", "Skill2C-tree2", "Skill3A-tree2", "Skill3B-tree2", "Skill3C-tree2", "Skill3D-tree2", "Skill4-tree2", "Skill5-tree2" };
        jackTree.InitializeSkills(skills);

        // Register tooltips
        jackTree.RegisterTooltip("Skill1-tree2", "Shark", "Można sprzedawać po wyższych cenach „punkty” w sklepie zasobów");
        jackTree.RegisterTooltip("Skill2A-tree2", "Market-place Genius", "Udany skill check chwilowo podwyższa ceny sprzedaży w sklepie");
        jackTree.RegisterTooltip("Skill2B-tree2", "Addict", "Potki mają zwiększoną skuteczność");
        jackTree.RegisterTooltip("Skill2C-tree2", "Lucky Bastard", "Nieudany skill check daje możliwość zdobycia jednej losowej potki");
        jackTree.RegisterTooltip("Skill3A-tree2", "Hard Worker", "Im więcej udanych skill checków tym ceny będą wyższe");
        jackTree.RegisterTooltip("Skill3B-tree2", "Death Dose", "Efekty potek się stakują");
        jackTree.RegisterTooltip("Skill3C-tree2", "Just Bastard", "Nieudany skill check gwarantuje zdobycie jednej losowej potki");
        jackTree.RegisterTooltip("Skill3D-tree2", "Fail To Win", "Nieudany skill check zwiększa chwilowo przyrost exp z akcji");
        jackTree.RegisterTooltip("Skill4-tree2", "Failure Grind", "im więcej nieudanych skill checków tym bonus jest większy");
        jackTree.RegisterTooltip("Skill5-tree2", "Micheal Scott", "Automatyczna sprzedaż wszystkich punktów po osiągnięciu limitu fabryki");

        // Draw lines
        Color lineColor = Color.yellow;
        jackTree.DrawLine("Skill1-tree2", "Skill2A-tree2", lineColor);
        jackTree.DrawLine("Skill1-tree2", "Skill2B-tree2", lineColor);
        jackTree.DrawLine("Skill1-tree2", "Skill2C-tree2", lineColor);
        jackTree.DrawLine("Skill2A-tree2", "Skill3A-tree2", lineColor);
        jackTree.DrawLine("Skill2B-tree2", "Skill3B-tree2", lineColor);
        jackTree.DrawLine("Skill2C-tree2", "Skill3C-tree2", lineColor);
        jackTree.DrawLine("Skill2C-tree2", "Skill3D-tree2", lineColor);
        jackTree.DrawLine("Skill3D-tree2", "Skill4-tree2", lineColor);
        jackTree.DrawLine("Skill4-tree2", "Skill5-tree2", lineColor);
        jackTree.DrawLine("Skill3C-tree2", "Skill5-tree2", lineColor);
        jackTree.DrawLine("Skill3B-tree2", "Skill5-tree2", lineColor);
        jackTree.DrawLine("Skill3A-tree2", "Skill5-tree2", lineColor);
    }

    private void InitializeAutomatronTree()
    {
        string[] skills = { "Skill1-tree3", "Skill2-tree3", "Skill3A-tree3", "Skill3B-tree3", "Skill4A-tree3", "Skill4B-tree3", "Skill5A-tree3", "Skill5B-tree3", "Skill5C-tree3", "Skill6-tree3" };
        automatronTree.InitializeSkills(skills);

        // Register tooltips
        automatronTree.RegisterTooltip("Skill1-tree3", "Entre-preneur", "Im więcej fabryk gracz posiada tym większy bonus do idle dla fabryki in focus");
        automatronTree.RegisterTooltip("Skill2-tree3", "Push to the limit", "Limit punktów na fabrykę zostaje zwiększony");
        automatronTree.RegisterTooltip("Skill3A-tree3", "Reaction Test", "Skill checki mogą się pojawiać w solowym momencie niezależnie od tego czy gracz kilka czy nie");
        automatronTree.RegisterTooltip("Skill3B-tree3", "What Eyes Don't See", "Fabryki not in Focus mają większy bonus do produkcji Idle");
        automatronTree.RegisterTooltip("Skill4A-tree3", "Unskilled Predator", "Nieudany skill check zwiększa produkcje idle do momentu udanego skill checka(stackuje się)");
        automatronTree.RegisterTooltip("Skill4B-tree3", "Passive Agressive", "Im dłużej gracz nie wykona akcji myszką tym więcej punktów zacznie się naliczać");
        automatronTree.RegisterTooltip("Skill5A-tree3", "One for Everyone", "Nieudany skill check zwiększa produkcje wszystkich fabryk");
        automatronTree.RegisterTooltip("Skill5B-tree3", "Multi-tasking", "Gracz może robić inne akcje poza klikanie w obiekt");
        automatronTree.RegisterTooltip("Skill5C-tree3", "Christmas Bonus", "Bonus aplikuje się do każdej fabryki not in focus");
        automatronTree.RegisterTooltip("Skill6-tree3", "Hungry Wolf", "Bonus zależy od ilość posiadanych już puntków(im mniej tym większy bonus)");

        // Draw lines
        Color lineColor = Color.yellow;
        automatronTree.DrawLine("Skill1-tree3", "Skill2-tree3", lineColor);
        automatronTree.DrawLine("Skill2-tree3", "Skill3A-tree3", lineColor);
        automatronTree.DrawLine("Skill2-tree3", "Skill3B-tree3", lineColor);
        automatronTree.DrawLine("Skill3A-tree3", "Skill4A-tree3", lineColor);
        automatronTree.DrawLine("Skill3B-tree3", "Skill4B-tree3", lineColor);
        automatronTree.DrawLine("Skill4A-tree3", "Skill5A-tree3", lineColor);
        automatronTree.DrawLine("Skill4B-tree3", "Skill5B-tree3", lineColor);
        automatronTree.DrawLine("Skill4B-tree3", "Skill5C-tree3", lineColor);
        automatronTree.DrawLine("Skill5C-tree3", "Skill6-tree3", lineColor);
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

        // Update skill trees
        oneClickArmyTree?.UpdateAll();
        jackTree?.UpdateAll();
        automatronTree?.UpdateAll();
    }

    public void SetCurrencyText(string value)
    {
        resourceShopManager?.SetCurrencyText(value);
    }

    public void SetGoldText(string value)
    {
        resourceShopManager?.SetGoldText(value);
    }
}