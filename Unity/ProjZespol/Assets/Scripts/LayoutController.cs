using System;
using System.Collections.Generic;
using UnityEditor;
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

    public Sprite[] images;

    public VisualElement ui;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button btn5;
    public Button btn6;
    public Label text;
    public VisualElement currencyIcon;
    public Label currencyLabel;
    public Label goldLabel;
    public VisualElement shopIcon;
    public Button sell1;
    public Slider currencySlider;
    public VisualElement ContentPage1; // karta zasobow
    public VisualElement ContentPage2;
    public VisualElement ContentPage3;
    public VisualElement ContentPage4;
    public VisualElement ContentPage5;
    public VisualElement ContentPage6;

    //Lvlbar
    public VisualElement BarMask;
    public Label LvlNumber;
    public VisualElement ExpBarContainer;
    public VisualElement BarTexture;

    //Menu
    public VisualElement Menu;
    public VisualElement Options;
    public VisualElement MainMenu;
    public Button Optionbtn;
    public Button Resume;
    public Button Settings;
    public Button Exit;
    public Button BackSettings;

    private Slider musicSlider;
    private Slider sfxSlider;
    private Toggle fullScreen;
    private DropdownField resolutionList;
    private Resolution[] resolutions;

    //AutoSave
    private Button DeleteSaveButton;
    private Toggle AutoSavetoggle;

    private class FactoryUI
    {
        public Button toggleButton;
        public VisualElement detailsPanel;
        public Label nameLabel;
        public Label countLabel;
        public Label costLabel;
        public Button unlockButton;
        public Button[] buyButtons = new Button[4]; // 1, 5, 25, MAX
        public Button[] sellButtons = new Button[4]; // 1, 5, 25, MAX
        public bool isVisible = false;

        public Button upgradeMainBtn;
        public VisualElement upgradePanel;
        public Button upgradeProductionBtn;
        public Button upgradeMultiplierBtn;
        public Label productionLevelLabel;
        public Label multiplierLevelLabel;
        public bool upgradesVisible = false;
    }

    private class PotionUI
    {
        public VisualElement potionPanel;
        public Label nameLabel;
        public Label costLabel;
        public Label effectLabel;
        public Button buyButton;
    }
    private Dictionary<int, FactoryUpgradeLevels> factoryUpgradeLevels = new Dictionary<int, FactoryUpgradeLevels>();

    private class FactoryUpgradeLevels
    {
        public int productionLevel = 0;
        public int multiplierLevel = 0;
        public double baseProductionCost = 0;
        public double baseMultiplierCost = 0;
    }

    public ScrollView scrollView;
    public ScrollView scrollView2;
    private List<FactoryUI> factoryUIs = new List<FactoryUI>();
    private List<PotionUI> potionUIs = new List<PotionUI>();
    private int numberOfFactories = 6;
    private int numberOfPotions = 3;
    private int currentFactoryIndex = 0;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource critSource;

    //drzewka
    private Dictionary<string, Button> buttons = new();
    private HashSet<string> learned = new();

    private Dictionary<string, Button> buttons2 = new();
    private HashSet<string> learned2 = new();

    private Dictionary<string, Button> buttons3 = new();
    private HashSet<string> learned3 = new();


    Color lockedColor = new Color(0.3f, 0.3f, 0.3f);
    Color availableColor = new Color(0.8f, 0.6f, 0.3f);
    Color learnedColor = new Color(0.2f, 0.8f, 0.2f);


    private Dictionary<string, List<string>> dependencies = new()
    {
        { "Skill1", new List<string>() },
        { "Skill2A", new List<string> { "Skill1" } },
        { "Skill2B", new List<string> { "Skill1" } },
        { "Skill3A", new List<string> { "Skill2A" } },
        { "Skill3B", new List<string> { "Skill2B" } },
        { "Skill4A", new List<string> { "Skill3A" } },
        { "Skill4B", new List<string> { "Skill3B" } },
        { "Skill5", new List<string> { "Skill4A", "Skill4B" } }, 
        { "Skill6A", new List<string> { "Skill5" } },
        { "Skill6B", new List<string> { "Skill5" } },
    };

    private Dictionary<string, List<string>> dependencies2 = new()
    {
        { "Skill1-tree2", new List<string>() },
        { "Skill2A-tree2", new List<string> { "Skill1-tree2" } },
        { "Skill2B-tree2", new List<string> { "Skill1-tree2" } },
        { "Skill2C-tree2", new List<string> { "Skill1-tree2" } },
        { "Skill3A-tree2", new List<string> { "Skill2A-tree2" } },
        { "Skill3B-tree2", new List<string> { "Skill2B-tree2" } },
        { "Skill3C-tree2", new List<string> { "Skill2C-tree2" } },
        { "Skill3D-tree2", new List<string> { "Skill2C-tree2" } },
        { "Skill4-tree2", new List<string> { "Skill3D-tree2" } },
        { "Skill5-tree2", new List<string> { "Skill4-tree2", "Skill3C-tree2", "Skill3B-tree2", "Skill3A-tree2" } }, 
    };

    private Dictionary<string, List<string>> dependencies3 = new()
    {
        { "Skill1-tree3", new List<string>() },
        { "Skill2-tree3", new List<string> { "Skill1-tree3" } },
        { "Skill3A-tree3", new List<string> { "Skill2-tree3" } },
        { "Skill3B-tree3", new List<string> { "Skill2-tree3" } },
        { "Skill4A-tree3", new List<string> { "Skill3A-tree3" } },
        { "Skill4B-tree3", new List<string> { "Skill3B-tree3" } },

        { "Skill5A-tree3", new List<string> { "Skill4A-tree3" } },
        { "Skill5B-tree3", new List<string> { "Skill4B-tree3" } },
        { "Skill5C-tree3", new List<string> { "Skill4B-tree3" } },
        { "Skill6-tree3", new List<string> { "Skill5C-tree3" } }, 
    };

    private VisualElement skillTreeContainer;
    private VisualElement lineLayer;
    private VisualElement skillTreeContainer2;
    private VisualElement lineLayer2;
    private VisualElement skillTreeContainer3;
    private VisualElement lineLayer3;

    //zmiana fabryk
    public Button prevFactoryBtn;
    public Button nextFactoryBtn;
    public Label factoryLabel;

    //przyciski do odblokowania fabryk
    private Button unlockFactory2Btn;
    private Button unlockFactory3Btn;
    private Button unlockFactory4Btn;
    private Button unlockFactory5Btn;
    private Button unlockFactory6Btn;

    void Awake()
    {
        Instance = this;
        ui = GetComponent<UIDocument>().rootVisualElement;
        resolutions = Screen.resolutions;

    }

    private void OnEnable()
    {

        
        currencyIcon = ui.Q<VisualElement>("cbbleicon");
        currencyLabel = ui.Q<Label>("Currency");
        goldLabel = ui.Q<Label>("Money");
        currencySlider = ui.Q<Slider>("AmoutSlider");

        text = ui.Q<Label>("Napis");
        btn1 = ui.Q<Button>("btn");
        btn1.clicked += ClickBtn1;
        btn2 = ui.Q<Button>("btn2");
        btn2.clicked += ClickBtn2;
        btn3 = ui.Q<Button>("btn3");
        btn3.clicked += ClickBtn3;
        btn4 = ui.Q<Button>("btn4");
        btn4.clicked += ClickBtn4;
        btn5 = ui.Q<Button>("btn5");
        btn5.clicked += ClickBtn5;
        btn6 = ui.Q<Button>("btn6");
        btn6.clicked += ClickBtn6;


        // zasoby
        shopIcon = ui.Q<VisualElement>("shopcbbleicon");
        sell1 = ui.Q<Button>("sell1");
        sell1.clicked += ClickSell1;

        ContentPage1 = ui.Q<VisualElement>("Content");
        ContentPage2 = ui.Q<VisualElement>("Content2");
        ContentPage3 = ui.Q<VisualElement>("Content3");
        ContentPage4 = ui.Q<VisualElement>("Content4");
        ContentPage5 = ui.Q<VisualElement>("Content5");
        ContentPage6 = ui.Q<VisualElement>("Content6");

        // lvlbar
        LvlNumber = ui.Q<Label>("lvlNumber");
        BarMask = ui.Q<VisualElement>("BarMask");
        ExpBarContainer = ui.Q<VisualElement>("ExpBarContainer");
        BarTexture = ui.Q<VisualElement>("BarTexture");

        //fabryki
        scrollView = ui.Q<ScrollView>("ScrollView1");
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        InitializeFactories();

        //potiki
        scrollView2 = ui.Q<ScrollView>("ScrollView2");
        scrollView2.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView2.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        InitializePotions();

        //drzewka tego typu


        OneClickArmyTreeInit();
        JackSkillTreeInit();
        AutomatronSkillTreeInit();


        skillTreeContainer = ui.Q<VisualElement>("SkillTree");
        skillTreeContainer2 = ui.Q<VisualElement>("SkillTree2");
        skillTreeContainer3 = ui.Q<VisualElement>("SkillTree3");

        lineLayer = new VisualElement();
        LineLayerInit(lineLayer);
        lineLayer2 = new VisualElement();
        LineLayerInit(lineLayer2);
        lineLayer3 = new VisualElement();
        LineLayerInit(lineLayer3);
        

        skillTreeContainer.Insert(0, lineLayer);
        skillTreeContainer2.Insert(0, lineLayer2);
        skillTreeContainer3.Insert(0, lineLayer3);

        DrawButtons();


        //menu 
        Menu = ui.Q<VisualElement>("Menu");
        MainMenu = ui.Q<VisualElement>("Main");
        Options = ui.Q<VisualElement>("Options");
        Optionbtn = ui.Q<Button>("Optionbtn");
        Optionbtn.clicked += EnableOptions;
        Resume = ui.Q<Button>("Resume");
        Resume.clicked += ResumeGame;
        Settings= ui.Q<Button>("Settings");
        Exit = ui.Q<Button>("Exit");
        Exit.clicked += ExitGame;
        Settings.clicked+=LoadSettings;
        BackSettings = ui.Q<Button>("Back");
        BackSettings.clicked += ReturnSettings;

        musicSlider = ui.Q<Slider>("Music");
        sfxSlider = ui.Q<Slider>("Sfx");

        fullScreen = ui.Q<Toggle>("Fullscreen");
        resolutionList = ui.Q<DropdownField>("Res");



        fullScreen.RegisterValueChangedCallback(evt =>
        {
            SetFullScreen(evt.newValue);
        });

        resolutionList.choices.Clear();
        int currentRes = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";

            if (Screen.width == resolutions[i].width &&
                Screen.height == resolutions[i].height)
            {
                currentRes = i;
            }

            resolutionList.choices.Add(option);
        }

        if (resolutionList.choices.Count > 0)
        {
            resolutionList.value = resolutionList.choices[currentRes];
        }

        resolutionList.RegisterValueChangedCallback(evt =>
        {
            int index = resolutionList.choices.IndexOf(evt.newValue);
            SetResolution(index);
        });

        fullScreen.value = Screen.fullScreen;


        sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);
        musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);
        
        audioSource.volume = sfxSlider.value;
        critSource.volume= sfxSlider.value;

        DeleteSaveButton = ui.Q<Button>("DeleteSaveButton");
        AutoSavetoggle = ui.Q<Toggle>("AutoSavetoggle");

        DeleteSaveButton.clicked += AutoSaveSystem.DeleteSave;
        DeleteSaveButton.clicked += () => { audioSource.Play(); };
        AutoSavetoggle.value = AutoSaveSystem.AutoSave;
        AutoSavetoggle.RegisterValueChangedCallback(evt =>
        {
            AutoSaveSystem.AutoSave = evt.newValue;
            audioSource.Play();
        });


        Tooltip.Init(ui, tooltipAsset);
        
        ui.RegisterCallback<GeometryChangedEvent>(Tooltip.SizeRefresh);
        //zmiana fabryk
        prevFactoryBtn = ui.Q<Button>("PrevFactoryBtn");
        nextFactoryBtn = ui.Q<Button>("NextFactoryBtn");
        factoryLabel = ui.Q<Label>("FactoryLabel");
        if (prevFactoryBtn != null)
        {
            prevFactoryBtn.clicked += CycleToPreviousFactory;
        }
            
        if (nextFactoryBtn != null)
        {
            nextFactoryBtn.clicked += CycleToNextFactory;
        }

        //przyciski do odblokowania fabryk
        unlockFactory2Btn = ui.Q<Button>("UnlockFactory2Btn");
        unlockFactory3Btn = ui.Q<Button>("UnlockFactory3Btn");
        unlockFactory4Btn = ui.Q<Button>("UnlockFactory4Btn");
        unlockFactory5Btn = ui.Q<Button>("UnlockFactory5Btn");
        unlockFactory6Btn = ui.Q<Button>("UnlockFactory6Btn");

        if (unlockFactory2Btn != null)
            unlockFactory2Btn.clicked += () => UnlockFactoryFromButton(1);

        if (unlockFactory3Btn != null)
            unlockFactory3Btn.clicked += () => UnlockFactoryFromButton(2);

        if (unlockFactory4Btn != null)
            unlockFactory4Btn.clicked += () => UnlockFactoryFromButton(3);

        if (unlockFactory5Btn != null)
            unlockFactory5Btn.clicked += () => UnlockFactoryFromButton(4);

        if (unlockFactory6Btn != null)
            unlockFactory6Btn.clicked += () => UnlockFactoryFromButton(5);
    }

    void LineLayerInit(VisualElement lineLayer)
    {
        lineLayer.pickingMode = PickingMode.Ignore;
        lineLayer.style.position = Position.Absolute;
        lineLayer.style.left = 0;
        lineLayer.style.top = 0;
        lineLayer.style.right = 0;
        lineLayer.style.bottom = 0;
    }

    void DrawButtons()
    {
        Color color = Color.yellow;

        DrawLineBetweenButtons(ui.Q<Button>("Skill1"), ui.Q<Button>("Skill2A"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill1"), ui.Q<Button>("Skill2B"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2A"), ui.Q<Button>("Skill3A"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2B"), ui.Q<Button>("Skill3B"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3A"), ui.Q<Button>("Skill4A"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3B"), ui.Q<Button>("Skill4B"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill4A"), ui.Q<Button>("Skill5"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill4B"), ui.Q<Button>("Skill5"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill5"), ui.Q<Button>("Skill6A"), color, lineLayer);
        DrawLineBetweenButtons(ui.Q<Button>("Skill5"), ui.Q<Button>("Skill6B"), color, lineLayer);

        //drugie drzewko
        DrawLineBetweenButtons(ui.Q<Button>("Skill1-tree2"), ui.Q<Button>("Skill2A-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill1-tree2"), ui.Q<Button>("Skill2B-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill1-tree2"), ui.Q<Button>("Skill2C-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2A-tree2"), ui.Q<Button>("Skill3A-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2B-tree2"), ui.Q<Button>("Skill3B-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2C-tree2"), ui.Q<Button>("Skill3C-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2C-tree2"), ui.Q<Button>("Skill3D-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3D-tree2"), ui.Q<Button>("Skill4-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill4-tree2"), ui.Q<Button>("Skill5-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3C-tree2"), ui.Q<Button>("Skill5-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3B-tree2"), ui.Q<Button>("Skill5-tree2"), color, lineLayer2);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3A-tree2"), ui.Q<Button>("Skill5-tree2"), color, lineLayer2);

        //trzecie drzewko
        DrawLineBetweenButtons(ui.Q<Button>("Skill1-tree3"), ui.Q<Button>("Skill2-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2-tree3"), ui.Q<Button>("Skill3A-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill2-tree3"), ui.Q<Button>("Skill3B-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3A-tree3"), ui.Q<Button>("Skill4A-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill3B-tree3"), ui.Q<Button>("Skill4B-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill4A-tree3"), ui.Q<Button>("Skill5A-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill4B-tree3"), ui.Q<Button>("Skill5B-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill4B-tree3"), ui.Q<Button>("Skill5C-tree3"), color, lineLayer3);
        DrawLineBetweenButtons(ui.Q<Button>("Skill5C-tree3"), ui.Q<Button>("Skill6-tree3"), color, lineLayer3);

    }

    void DrawLineBetweenButtons(Button from, Button to, Color color, VisualElement targetLayer)
    {
        var line = new VisualElement();
        line.pickingMode = PickingMode.Ignore;
        line.generateVisualContent += ctx =>
        {
            if (from == null || to == null) return;

            var start = from.worldBound.center;
            var end = to.worldBound.center;

            var localStart = targetLayer.WorldToLocal(start);
            var localEnd = targetLayer.WorldToLocal(end);

            ctx.painter2D.strokeColor = color;
            ctx.painter2D.lineWidth = 3f;
            ctx.painter2D.BeginPath();
            ctx.painter2D.MoveTo(localStart);
            ctx.painter2D.LineTo(localEnd);
            ctx.painter2D.Stroke();
        };

        targetLayer.Add(line);
    }


    private void OneClickArmyTreeInit()
    {
       
        var skill1 = ui.Q<Button>("Skill1");
        var skill2A = ui.Q<Button>("Skill2A");
        var skill2B = ui.Q<Button>("Skill2B");

        var skill3A = ui.Q<Button>("Skill3A");
        var skill3B = ui.Q<Button>("Skill3B");

        var skill4A = ui.Q<Button>("Skill4A");
        var skill4B = ui.Q<Button>("Skill4B");

        var skill5 = ui.Q<Button>("Skill5");

        var skill6A = ui.Q<Button>("Skill6A");
        var skill6B = ui.Q<Button>("Skill6B");

        Tooltip.Register(skill1, "Skill Based Clicking", "Umożliwia pojawienie się skill checków(nie udany kosztuje gracza zwolnieniem idle produkcji)");
        
        Tooltip.Register(skill2A, "Critical Mass", "Zwiększa szanse na kliki krytyczne");
        Tooltip.Register(skill2B, "Chicken Dinner", "Udany skill check zwiększa ilość zbieranych punktów na chwilę(nie udany zmniejsza)");

        Tooltip.Register(skill3A, "Active Idle", "Każdy klik krytyczny chwilowo zwiększa idle produkcje");
        Tooltip.Register(skill3B, "Hungry For More", "Skill checki częściej się pojawiają");

        Tooltip.Register(skill4A, "No Matter What", "Stakuje szanse na klik krytyczny(każde kliknięcie niekrytyczne zwiększa szanse na krytyczne)");
        Tooltip.Register(skill4B, "Always Winner", "Skill checki nie mają negatywnych skutków po przegraniu");

        Tooltip.Register(skill5, "Symbiosis", "Częstotliwość skill checka jest zależna od ilości klików krytycznych(im częściej są tym częściej skill checki)");

        Tooltip.Register(skill6A, "Mortal Clicker", "Każdy kolejny skill check łączy się w kombos. Im większy kombos tym więcej punktów za klik(im większy kombos też trudniejsze skill checki)");
        Tooltip.Register(skill6B, "Champion Of Clicks", "Po trzech udanych skill checkach z rzędu przez krótki moment są same kliki krytyczne");

        string[] all = { "Skill1", "Skill2A", "Skill2B", "Skill3A", "Skill3B", "Skill4A", "Skill4B", "Skill5", "Skill6A", "Skill6B" };

        foreach (string id in all)
        {
            var btn = ui.Q<Button>(id);
            if (btn != null)
            {
                buttons[id] = btn;
                btn.RegisterCallback<PointerUpEvent>(evt => {
                    if (evt.button == 0) //tutaj bedzie trzeba inputa dac zamiast buttona0myszki
                        OnSkillClicked(id);
                    else if (evt.button == 1) //tutaj tez 
                        OnSkillUnlearned(id);
                });
                UpdateVisual(id);
            }
        }

    }

    void OnSkillUnlearned(string id)
    {
       
        if (!learned.Contains(id))
            return;

        foreach (var kvp in dependencies)
        {
            if (kvp.Value.Contains(id) && learned.Contains(kvp.Key))
            {
                return;
            }
        }

        learned.Remove(id);
        characterClass.ApplySkillChanges(id, true);
        UpdateAll();
    }




    void OnSkillClicked(string id)
    {
        if (learned.Contains(id))
            return;

        if (!CanUnlock(id))
            return;

        if (id == "Skill6A" && learned.Contains("Skill6B"))
            return;
        if (id == "Skill6B" && learned.Contains("Skill6A"))
            return;


        learned.Add(id);
        characterClass.ApplySkillChanges(id, false);
        UpdateVisual(id);
    }

    bool CanUnlock(string id)
    {
        if (!dependencies.ContainsKey(id)) return true;


        if (id == "Skill5")
        {
            foreach (var prereq in dependencies[id])
            {
                if (learned.Contains(prereq))
                    return true;
            }
            return false;
        }


        foreach (var prereq in dependencies[id])
        {
            if (!learned.Contains(prereq))
                return false;
        }

        return true;
    }

    void UpdateVisual(string id)
    {
        if (!buttons.ContainsKey(id)) return;
        var btn = buttons[id];

        if (learned.Contains(id))
        {
            btn.style.backgroundColor = learnedColor;
            return;
        }

        if ((learned.Contains("Skill6A") && id == "Skill6B") ||
            (learned.Contains("Skill6B") && id == "Skill6A"))
        {
            btn.style.backgroundColor = lockedColor;
            
            return;
        }

        if (CanUnlock(id))
        {
            btn.style.backgroundColor = availableColor;
        }
        else
        {
            btn.style.backgroundColor = lockedColor;
            

        }
    }





    private void JackSkillTreeInit()
    {

        var skill1 = ui.Q<Button>("Skill1-tree2");

        var skill2 = ui.Q<Button>("Skill2A-tree2");
        var skill3 = ui.Q<Button>("Skill2B-tree2");
        var skill4 = ui.Q<Button>("Skill2C-tree2");
        
        var skill5 = ui.Q<Button>("Skill3A-tree2");
        var skill6 = ui.Q<Button>("Skill3B-tree2");
        var skill7 = ui.Q<Button>("Skill3C-tree2");
        var skill8 = ui.Q<Button>("Skill3D-tree2");

        var skill9 = ui.Q<Button>("Skill4-tree2");
        
        var skill10 = ui.Q<Button>("Skill5-tree2");

        Tooltip.Register(skill1, "Shark", "Można sprzedawać po wyższych cenach „punkty”");

        Tooltip.Register(skill2, "Market-place Genius", "Udany skill check chwilowo podwyższa ceny sprzedaży w sklepie");
        Tooltip.Register(skill3, "Addict", "Potki mają zwiększoną skuteczność");
        Tooltip.Register(skill4, "Lucky Bastard", "Nieudany skill check daje możliwość zdobycia jednej losowej potki");
       
        Tooltip.Register(skill5, "Hard Worker", "Im więcej udanych skill checków tym ceny będą wyższe");
        Tooltip.Register(skill6, "Death Dose", "Efekty potek się stakują");
        Tooltip.Register(skill7, "Just Bastard", "Nieudany skill check gwarantuje zdobycie jednej losowej potki");
        Tooltip.Register(skill8, "Fail To Win", "Nieudany skill check zwiększa chwilowo przyrost exp z akcji");
        
        Tooltip.Register(skill9, "Failure Grind", "im więcej nieudanych skill checków tym bonus jest większy");
        
        Tooltip.Register(skill10, "Micheal Scott", "Automatyczna sprzedaż wszystkich punktów po osiągnięciu limitu fabryki");


        var all2 = new[] { "Skill1-tree2", "Skill2A-tree2", "Skill2B-tree2", "Skill2C-tree2","Skill3A-tree2", "Skill3B-tree2", "Skill3C-tree2", "Skill3D-tree2", "Skill4-tree2","Skill5-tree2"   };
        foreach (string id in all2)
        {
            var btn = ui.Q<Button>(id);
            if (btn != null)
            {
                buttons2[id] = btn;
                btn.RegisterCallback<PointerUpEvent>(evt =>
                {
                    if (evt.button == 0) // input TODO
                        OnSkillClicked_T2(id);
                    else if (evt.button == 1) // input TODO
                        OnSkillUnlearned_T2(id);
                });
                UpdateVisual_T2(id);
            }
        }

    }

    void OnSkillUnlearned_T2(string id)
    {
        if (!learned2.Contains(id))
            return;

        foreach (var kvp in dependencies2)
        {
            if (kvp.Value.Contains(id) && learned2.Contains(kvp.Key))
            {
                return;
            }
        }
        learned2.Remove(id);
        UpdateAll();
    }

    void OnSkillClicked_T2(string id)
    {
        if (learned2.Contains(id))
            return;

        if (!CanUnlock_T2(id))
            return;

        learned2.Add(id);
        UpdateVisual_T2(id);
    }

    bool CanUnlock_T2(string id)
    {
        if (!dependencies2.ContainsKey(id)) return true;


        if (id == "Skill5-tree2")
        {
            foreach (var prereq in dependencies2[id])
            {
                if (learned2.Contains(prereq))
                    return true;
            }
            return false;
        }


        foreach (var prereq in dependencies2[id])
        {
            if (!learned2.Contains(prereq))
                return false;
        }

        return true;
    }



    void UpdateVisual_T2(string id)
    {
        if (!buttons2.ContainsKey(id)) return;
        var btn = buttons2[id];

        

        if (learned2.Contains(id))
        {
            btn.style.backgroundColor = learnedColor;
            return;
        }

        if (CanUnlock_T2(id))
            btn.style.backgroundColor = availableColor;
        else
        {
            btn.style.backgroundColor = lockedColor;
           
        }
    }


    private void AutomatronSkillTreeInit()
    {

        var skill1 = ui.Q<Button>("Skill1-tree3");

        var skill2 = ui.Q<Button>("Skill2-tree3");

        var skill3 = ui.Q<Button>("Skill3A-tree3");
        var skill4 = ui.Q<Button>("Skill3B-tree3");
        
        var skill5 = ui.Q<Button>("Skill4A-tree3");
        var skill6 = ui.Q<Button>("Skill4B-tree3");

        var skill7 = ui.Q<Button>("Skill5A-tree3");
        var skill8 = ui.Q<Button>("Skill5B-tree3");
        var skill9 = ui.Q<Button>("Skill5C-tree3");

        var skill10 = ui.Q<Button>("Skill6-tree3");

        Tooltip.Register(skill1, "Entre-preneur", "Im więcej fabryk gracz posiada tym większy bonus do idle dla fabryki in focus");

        Tooltip.Register(skill2, "Push to the limit", "Limit punktów na fabrykę zostaje zwiększony");

        Tooltip.Register(skill3, "Reaction Test", "Skill checki mogą się pojawiać w solowym momencie niezależnie od tego czy gracz kilka czy nie");
        Tooltip.Register(skill4, "What Eyes Don’t See", "Fabryki not in Focus mają większy bonus do produkcji Idle");

        Tooltip.Register(skill5, "Unskilled Predator", "Nieudany skill check zwiększa produkcje idle do momentu udanego skill checka(stackuje się)");
        Tooltip.Register(skill6, "Passive Agressive", "Im dłużej gracz nie wykona akcji myszką tym więcej punktów zacznie się naliczać");

        Tooltip.Register(skill7, "One for Everyone", "Nieudany skill check zwiększa produkcje wszystkich fabryk");
        Tooltip.Register(skill8, "Multi-tasking", "Gracz może robić inne akcje poza klikanie w obiekt");
        Tooltip.Register(skill9, "Christmas Bonus", "Bonus aplikuje się do każdej fabryki not in focus");

        Tooltip.Register(skill10, "Hungry Wolf", "Bonus zależy od ilość posiadanych już puntków(im mniej tym większy bonus)");

        var all3 = new[] { "Skill1-tree3", "Skill2-tree3", "Skill3A-tree3", "Skill3B-tree3", "Skill4A-tree3", "Skill4B-tree3", "Skill5A-tree3", "Skill5B-tree3", "Skill5C-tree3", "Skill6-tree3" };
        foreach (string id in all3)
        {
            var btn = ui.Q<Button>(id);
            if (btn != null)
            {
                buttons3[id] = btn;
                btn.RegisterCallback<PointerUpEvent>(evt =>
                {
                    if (evt.button == 0) // input TODO
                        OnSkillClicked_T3(id);
                    else if (evt.button == 1) // input TODO
                        OnSkillUnlearned_T3(id);
                });
                UpdateVisual_T3(id);
            }
        }

    }


    void OnSkillClicked_T3(string id)
    {
        if (learned3.Contains(id))
            return;

        if (!CanUnlock_T3(id))
            return;

        learned3.Add(id);
        characterClass.ApplySkillChanges(id, false);
        UpdateVisual_T3(id);
    }

    void OnSkillUnlearned_T3(string id)
    {
        if (!learned3.Contains(id))
            return;

        foreach (var kvp in dependencies3)
        {
            if (kvp.Value.Contains(id) && learned3.Contains(kvp.Key))
            {
                return;
            }
        }

        learned3.Remove(id);
        characterClass.ApplySkillChanges(id, true);
        UpdateAll();
    }


    bool CanUnlock_T3(string id)
    {
        if (!dependencies3.ContainsKey(id)) return true;

        
        foreach (var prereq in dependencies3[id])
        {
            if (!learned3.Contains(prereq))
                return false;
        }

        return true;
    }

    void UpdateVisual_T3(string id)
    {
        if (!buttons3.ContainsKey(id)) return;
        var btn = buttons3[id];
        

        if (learned3.Contains(id))
        {
            btn.style.backgroundColor = learnedColor;
            return;
        }

        if (CanUnlock_T3(id))
            btn.style.backgroundColor = availableColor;
        else
        {
            btn.style.backgroundColor = lockedColor;
            
        }
    }



    void UpdateAll()
    {
        foreach (var id in buttons.Keys)
            UpdateVisual(id);
        foreach (var id in buttons2.Keys)
            UpdateVisual_T2(id);
        foreach (var id in buttons3.Keys)
            UpdateVisual_T3(id);
    }




    private void InitializeFactories()
    {
        for (int i = 1; i <= numberOfFactories; i++)
        {
            FactoryUI factoryUI = new FactoryUI();
            
            factoryUI.toggleButton = ui.Q<Button>($"Factory{i}Btn");
            factoryUI.detailsPanel = ui.Q<VisualElement>($"Factory{i}Details");
            factoryUI.nameLabel = ui.Q<Label>($"Factory{i}Name");
            factoryUI.countLabel = ui.Q<Label>($"Factory{i}Count");
            factoryUI.costLabel = ui.Q<Label>($"Factory{i}Cost");

            factoryUI.buyButtons[0] = ui.Q<Button>($"Factory{i}Buy1");
            factoryUI.buyButtons[1] = ui.Q<Button>($"Factory{i}Buy5");
            factoryUI.buyButtons[2] = ui.Q<Button>($"Factory{i}Buy25");
            factoryUI.buyButtons[3] = ui.Q<Button>($"Factory{i}BuyMax");

            factoryUI.sellButtons[0] = ui.Q<Button>($"Factory{i}Sell1");
            factoryUI.sellButtons[1] = ui.Q<Button>($"Factory{i}Sell5");
            factoryUI.sellButtons[2] = ui.Q<Button>($"Factory{i}Sell25");
            factoryUI.sellButtons[3] = ui.Q<Button>($"Factory{i}SellMax");

            factoryUI.unlockButton = ui.Q<Button>($"Factory{i}Unlock");

            factoryUI.upgradeMainBtn = ui.Q<Button>($"Factory{i}UpgradeBtn");
            factoryUI.upgradePanel = ui.Q<VisualElement>($"Factory{i}UpgradePanel");
            factoryUI.upgradeProductionBtn = ui.Q<Button>($"Factory{i}UpgradeProduction");
            factoryUI.upgradeMultiplierBtn = ui.Q<Button>($"Factory{i}UpgradeMultiplier");
            factoryUI.productionLevelLabel = ui.Q<Label>($"Factory{i}ProductionLevel");
            factoryUI.multiplierLevelLabel = ui.Q<Label>($"Factory{i}MultiplierLevel");

            int factoryIndex = i - 1;
            int factoryIndexCopy = factoryIndex;

            if (!factoryUpgradeLevels.ContainsKey(factoryIndex))
            {
                var factory = GetFactory(factoryIndex);
                factoryUpgradeLevels[factoryIndex] = new FactoryUpgradeLevels
                {
                    productionLevel = 0,
                    multiplierLevel = 0,
                    baseProductionCost = factory != null ? factory.currentCost * 5 : 100,
                    baseMultiplierCost = factory != null ? factory.currentCost * 10 : 200
                };
            }

            if (factoryUI.unlockButton != null)
            {
                factoryUI.unlockButton.clicked += () => UnlockFactory(factoryIndexCopy);
            }

            
            if (factoryUI.toggleButton != null)
            {
                factoryUI.toggleButton.clicked += () => ToggleFactoryDetails(factoryIndex);
            }

            if (factoryUI.buyButtons[0] != null)
                factoryUI.buyButtons[0].clicked += () => BuyFactory(factoryIndex, 1);
            if (factoryUI.buyButtons[1] != null)
                factoryUI.buyButtons[1].clicked += () => BuyFactory(factoryIndex, 5);
            if (factoryUI.buyButtons[2] != null)
                factoryUI.buyButtons[2].clicked += () => BuyFactory(factoryIndex, 25);
            if (factoryUI.buyButtons[3] != null)
                factoryUI.buyButtons[3].clicked += () => BuyFactoryMax(factoryIndex);

            if (factoryUI.sellButtons[0] != null)
                factoryUI.sellButtons[0].clicked += () => SellFactory(factoryIndex, 1);
            if (factoryUI.sellButtons[1] != null)
                factoryUI.sellButtons[1].clicked += () => SellFactory(factoryIndex, 5);
            if (factoryUI.sellButtons[2] != null)
                factoryUI.sellButtons[2].clicked += () => SellFactory(factoryIndex, 25);
            if (factoryUI.sellButtons[3] != null)
                factoryUI.sellButtons[3].clicked += () => SellFactoryMax(factoryIndex);

            if (factoryUI.upgradeMainBtn != null)
{
            factoryUI.upgradeMainBtn.clicked += () =>
            {
                factoryUI.upgradesVisible = !factoryUI.upgradesVisible;
                if (factoryUI.upgradePanel != null)
                    factoryUI.upgradePanel.style.display = factoryUI.upgradesVisible ? DisplayStyle.Flex : DisplayStyle.None;
            };
            }

            if (factoryUI.upgradeProductionBtn != null)
                factoryUI.upgradeProductionBtn.clicked += () => UpgradeFactoryBaseProduction(factoryIndex);

            if (factoryUI.upgradeMultiplierBtn != null)
                factoryUI.upgradeMultiplierBtn.clicked += () => UpgradeFactoryMultiplier(factoryIndex);

            factoryUIs.Add(factoryUI);
        }
    }

    private void InitializePotions()
    {
        for (int i = 1; i <= numberOfPotions; i++)
        {
            PotionUI potionUI = new PotionUI();

            potionUI.buyButton = ui.Q<Button>($"Potion{i}Buy");
            potionUI.nameLabel = ui.Q<Label>($"Potion{i}Name");
            potionUI.effectLabel = ui.Q<Label>($"Potion{i}Effect");
            potionUI.costLabel = ui.Q<Label>($"Potion{i}Cost");

            if (potionUI.nameLabel != null)
            {
                potionUI.nameLabel.style.whiteSpace = WhiteSpace.Normal;
                potionUI.nameLabel.style.flexWrap = Wrap.Wrap;
            }
            
            if (potionUI.effectLabel != null)
            {
                potionUI.effectLabel.style.whiteSpace = WhiteSpace.Normal;
                potionUI.effectLabel.style.flexWrap = Wrap.Wrap;
            }
            
            if (potionUI.costLabel != null)
            {
                potionUI.costLabel.style.whiteSpace = WhiteSpace.Normal;
                potionUI.costLabel.style.flexWrap = Wrap.Wrap;
            }

            int potionIndex = i - 1;
            int potionIndexCopy = potionIndex;

            if (potionUI.buyButton != null)
            {
                potionUI.buyButton.clicked += () => BuyPotion(potionIndexCopy);
            }

            potionUIs.Add(potionUI);
        }
    }

    private void ClickBtn1()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.Flex;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.None;
        ContentPage6.style.display = DisplayStyle.None;
    }
    
    private void ClickBtn2()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.Flex;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.None;
        ContentPage6.style.display = DisplayStyle.None;
    }
    
    private void ClickBtn3()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.Flex;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.None;
        ContentPage6.style.display = DisplayStyle.None;
    }
    private void ClickBtn4()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.Flex;
        ContentPage5.style.display = DisplayStyle.None;
        ContentPage6.style.display = DisplayStyle.None;
    }
    private void ClickBtn5()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.Flex;
        ContentPage6.style.display = DisplayStyle.None;
    }
    private void ClickBtn6()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.None;
        ContentPage6.style.display = DisplayStyle.Flex;
    }

    private void ClickSell1()
    {
        double exp = raptorCore.SellMaterials(currencySlider.value);
        characterClass.GainExp((ulong)exp);
        audioSource.Play();
    }

    public void SetCurrencyText(string value)
    {
        if (currencyLabel != null)
            currencyLabel.text = value;
    }

    public void SetGoldText(string value)
    {
        if (goldLabel != null)
            goldLabel.text = value;
    }

    public void Update()
    {
        text.text = currencySlider.value.ToString() + "%";
        UpdateLvlBar();
        UpdateFactoryUI();
        UpdatePotionUI();
        UpdateAll();
        UpdateNavigationButtons();
        UpdateUnlockButtons();
    }

    private void UpdateLvlBar()
    {
        LvlNumber.text = characterClass.GetLevel().ToString();

        BarTexture.style.minWidth = ExpBarContainer.resolvedStyle.width;
        BarTexture.style.maxWidth = ExpBarContainer.resolvedStyle.width;
        BarTexture.style.width = ExpBarContainer.resolvedStyle.width;


        float currentExp = characterClass.GetCurrentExp();
        float maxExp = characterClass.GetMaxExpCap();
        float progress = Mathf.Clamp01(currentExp / maxExp);

        BarMask.style.width = new Length(progress * 100f, LengthUnit.Percent);
    }

    private void ToggleFactoryDetails(int factoryIndex)
    {
        audioSource.Play();
        
        if (factoryIndex < 0 || factoryIndex >= factoryUIs.Count)
            return;

        var factoryUI = factoryUIs[factoryIndex];
        factoryUI.isVisible = !factoryUI.isVisible;

        if (factoryUI.detailsPanel != null)
        {
            factoryUI.detailsPanel.style.display = factoryUI.isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void BuyFactory(int factoryIndex, int amount)
    {
        audioSource.Play();
        
        if (idleManager == null)
        {
            Debug.LogError("IdleManager nie jest przypisany!");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            bool success = idleManager.BuyFactory(factoryIndex);
            if (!success)
            {
                Debug.Log($"Kupiono tylko {i} z {amount} fabryk (brak środków)");
                break;
            }
        }

        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }

    private void BuyFactoryMax(int factoryIndex)
    {
        audioSource.Play();
        
        if (idleManager == null)
        {
            Debug.LogError("IdleManager nie jest przypisany!");
            return;
        }

        var factory = GetFactory(factoryIndex);
        if (factory == null) return;

        int maxAmount = 0;
        double playerGold = raptorCore.Gold;
        double currentCost = factory.currentCost;

        while (playerGold >= currentCost && maxAmount < 1000) // limit bezpieczeństwa
        {
            playerGold -= currentCost;
            currentCost *= factory.costMultiplier;
            maxAmount++;
        }

        for (int i = 0; i < maxAmount; i++)
        {
            if (!idleManager.BuyFactory(factoryIndex))
                break;
        }

        Debug.Log($"Kupiono MAX: {maxAmount} fabryk");
        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }

    private void SellFactory(int factoryIndex, int amount)
    {
        audioSource.Play();
        
        if (idleManager == null)
        {
            Debug.LogError("IdleManager nie jest przypisany!");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            bool success = idleManager.SellFactory(factoryIndex);
            if (!success)
            {
                Debug.Log($"Sprzedano tylko {i} z {amount} fabryk (brak fabryk)");
                break;
            }
        }

        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }

    private void SellFactoryMax(int factoryIndex)
    {
        audioSource.Play();
        
        if (idleManager == null)
        {
            Debug.LogError("IdleManager nie jest przypisany!");
            return;
        }

        var factory = GetFactory(factoryIndex);
        if (factory == null) return;

        int soldCount = 0;
        while (factory.count > 0)
        {
            if (!idleManager.SellFactory(factoryIndex))
                break;
            soldCount++;
        }

        Debug.Log($"Sprzedano MAX: {soldCount} fabryk");
        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }

    private void UpdateFactoryUI()
    {
        if (idleManager == null || raptorCore == null) return;

        for (int i = 0; i < factoryUIs.Count; i++)
        {
            var factoryUI = factoryUIs[i];
            var factory = GetFactory(i);
            if (factory == null) continue;

            if (i != currentFactoryIndex)
            {
                if (factoryUI.toggleButton != null)
                    factoryUI.toggleButton.style.display = DisplayStyle.None;
                if (factoryUI.detailsPanel != null)
                    factoryUI.detailsPanel.style.display = DisplayStyle.None;
                continue;
            }

            if (factoryUI.toggleButton != null)
            {
                factoryUI.toggleButton.style.display = DisplayStyle.Flex;
                factoryUI.nameLabel.text = factory.name;
            }

            if (factoryUI.countLabel != null)
                factoryUI.countLabel.text = $"Ilość: {factory.count}";
            if (factoryUI.costLabel != null)
                factoryUI.costLabel.text = factory.isUnlocked
                    ? $"Koszt: {factory.currentCost}"
                    : $"Odblokuj za: {factory.unlockCost}";

            double playerGold = raptorCore.Gold;

            if (factoryUpgradeLevels.ContainsKey(i))
            {
                var upgradeLevels = factoryUpgradeLevels[i];

                double productionCost = CalculateUpgradeCost(upgradeLevels.baseProductionCost, upgradeLevels.productionLevel);

                if (!factory.isUnlocked)
                {
                    factoryUI.upgradePanel.style.display = DisplayStyle.None;
                }
                else
                {
                    factoryUI.upgradePanel.style.display = factoryUI.upgradesVisible ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (factoryUI.productionLevelLabel != null && factory.isUnlocked)
                {
                    factoryUI.productionLevelLabel.text = $"Poziom: {upgradeLevels.productionLevel}";
                }
                    
                
                factoryUI.multiplierLevelLabel.style.display = DisplayStyle.None;

                if (factoryUI.upgradeProductionBtn != null && factory.isUnlocked)
                {
                    factoryUI.upgradeProductionBtn.text = $"Zwiększ produkcję (+1) - {productionCost:F0} G";
                    factoryUI.upgradeProductionBtn.SetEnabled(playerGold >= productionCost);
                }
                factoryUI.upgradeMultiplierBtn.style.display = DisplayStyle.None;
            }


            if (!factory.isUnlocked)
            {
                if (factoryUI.unlockButton != null)
                    factoryUI.unlockButton.style.display = DisplayStyle.None;

                foreach (var btn in factoryUI.buyButtons)
                    if (btn != null) btn.style.display = DisplayStyle.None;
                foreach (var btn in factoryUI.sellButtons)
                    if (btn != null) btn.style.display = DisplayStyle.None;
            }
            else
            {
                if (factoryUI.unlockButton != null)
                    factoryUI.unlockButton.style.display = DisplayStyle.None;

                foreach (var btn in factoryUI.buyButtons)
                    if (btn != null) btn.style.display = DisplayStyle.Flex;
                foreach (var btn in factoryUI.sellButtons)
                    if (btn != null) btn.style.display = DisplayStyle.Flex;

                UpdateButton(factoryUI.buyButtons[0], playerGold >= factory.currentCost);
                UpdateButton(factoryUI.buyButtons[1], playerGold >= factory.currentCost * 5);
                UpdateButton(factoryUI.buyButtons[2], playerGold >= factory.currentCost * 25);
                UpdateButton(factoryUI.buyButtons[3], playerGold >= factory.currentCost);
                UpdateButton(factoryUI.sellButtons[0], factory.count >= 1);
                UpdateButton(factoryUI.sellButtons[1], factory.count >= 5);
                UpdateButton(factoryUI.sellButtons[2], factory.count >= 25);
                UpdateButton(factoryUI.sellButtons[3], factory.count > 0);
            }
        }

    }


    private void UpdateButton(Button button, bool enabled)
    {
        if (button != null)
        {
            button.SetEnabled(enabled);
        }
    }

    private Factory GetFactory(int index)
    {
        if (idleManager == null) return null;

        return idleManager.GetFactory(index);
    }

    private void UnlockFactory(int factoryIndex)
    {
        audioSource.Play();

        if (idleManager == null) return;

        bool success = idleManager.UnlockFactory(factoryIndex);

        if (success)
        {
            Debug.Log($"Pomyślnie odblokowano fabrykę {factoryIndex}");
        }
        else
        {
            Debug.Log($"Brak kasy na odblokowanie fabryki {factoryIndex}");
        }

        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }

    private double CalculateUpgradeCost(double baseCost, int currentLevel)
    {
        return baseCost * Mathf.Pow(1.5f, currentLevel);
    }
    private void UpgradeFactoryBaseProduction(int factoryIndex)
    {
        var factory = GetFactory(factoryIndex);
        if (factory == null || !factoryUpgradeLevels.ContainsKey(factoryIndex))
            return;

        var upgradeLevels = factoryUpgradeLevels[factoryIndex];
        double cost = CalculateUpgradeCost(upgradeLevels.baseProductionCost, upgradeLevels.productionLevel);

        if (raptorCore.Gold < cost)
        {
            Debug.Log("Za mało złota na upgrade produkcji");
            return;
        }

        factory.baseProduction += 1.0;
        raptorCore.Gold -= Math.Floor(cost);
        upgradeLevels.productionLevel++;

        audioSource.Play();
        Debug.Log($"Zwiększono baseProduction fabryki {factoryIndex} do {factory.baseProduction} (Poziom: {upgradeLevels.productionLevel})");
        SetGoldText(raptorCore.Gold.ToString("F0"));
        UpdateFactoryUI();
    }

    private void UpgradeFactoryMultiplier(int factoryIndex)
    {
        var factory = GetFactory(factoryIndex);
        if (factory == null || !factoryUpgradeLevels.ContainsKey(factoryIndex))
            return;

        var upgradeLevels = factoryUpgradeLevels[factoryIndex];
        double cost = CalculateUpgradeCost(upgradeLevels.baseMultiplierCost, upgradeLevels.multiplierLevel);

        if (raptorCore.Gold < cost)
        {
            Debug.Log("Za mało złota na upgrade mnożnika");
            return;
        }

        factory.productionMultiplier += 0.1;
        raptorCore.Gold -= Math.Floor(cost);
        upgradeLevels.multiplierLevel++;

        audioSource.Play();
        Debug.Log($"Zwiększono multiplier fabryki {factoryIndex} do {factory.productionMultiplier} (Poziom: {upgradeLevels.multiplierLevel})");
        SetGoldText(raptorCore.Gold.ToString("F0"));
        UpdateFactoryUI();
    }

    private void RegisterSkillHover(Button button, string description,Label label)
    {
        button.RegisterCallback<MouseEnterEvent>(evt =>
        {
            label.text = description;
        });

        button.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            label.text = "Najedź na skill, aby zobaczyć opis";
        });
    }
    
    private void EnableOptions()
    {
        audioSource.Play();
        Menu.style.display = DisplayStyle.Flex;
    }
    private void ResumeGame()
    {
        audioSource.Play();
        Menu.style.display = DisplayStyle.None;
    }
    private void ExitGame()
    {
        audioSource.Play();
        Application.Quit();
    }
    private void LoadSettings()
    {
        MainMenu.style.display = DisplayStyle.None;
        Options.style.display = DisplayStyle.Flex;
        audioSource.Play();
    }
    private void ReturnSettings()
    {
        MainMenu.style.display = DisplayStyle.Flex;
        Options.style.display = DisplayStyle.None;
        audioSource.Play();
    }

    private void OnSfxVolumeChanged(ChangeEvent<float> evt)
    {
        
        Debug.Log($"Głośność sfx: {evt.newValue}");
        audioSource.volume = evt.newValue;
        critSource.volume = evt.newValue;
        PlayerPrefs.SetFloat("SfxVolume", evt.newValue);
        PlayerPrefs.Save();
    }
    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        //musicSource.volume = evt.newValue;
        PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
        PlayerPrefs.Save();
        Debug.Log($"Głośność muzyki: {evt.newValue}");
    }
    private void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        

    }
    private void SetFullScreen(bool flag)
    {
        Screen.fullScreen = flag;
        
    }

    private void UpdatePotionUI()
    {
        if (idleManager == null || raptorCore == null) return;

        for (int i = 0; i < potionUIs.Count; i++)
        {
            var potionUI = potionUIs[i];
            var potion = idleManager.GetPotion(i);
            if (potion == null) continue;

            if (potionUI.nameLabel != null)
                potionUI.nameLabel.text = potion.name;
            if (potionUI.effectLabel != null)
                potionUI.effectLabel.text = potion.effectDescription;
            if (potionUI.costLabel != null)
                potionUI.costLabel.text = $"Koszt: {potion.cost} szt. zasobu {potion.resourceType.name}";

            if (potionUI.buyButton != null)
            {
                QuarkType playerResourceAmount = raptorCore.GetResourceValueDirect(potion.resourceType.name);
                bool canAfford = playerResourceAmount >= potion.cost;
                bool isNotActive = !potion.isActive;
                potionUI.buyButton.SetEnabled(canAfford && isNotActive);
                if (potion.isActive)
                {
                    potionUI.buyButton.text = "Aktywna";
                }
                else
                {
                    potionUI.buyButton.text = "Kup";
                }
            }
        }
    }

    private void BuyPotion(int potionIndex)
    {
        audioSource.Play();
        
        if (idleManager == null)
        {
            Debug.LogError("IdleManager nie jest przypisany!");
            return;
        }

        bool success = idleManager.BuyPotion(potionIndex);
        if (!success)
        {
            Debug.Log($"Brak środków na zakup mikstury {potionIndex}");
        }
        else
        {
            Debug.Log($"Zakupiono miksturę {potionIndex}");
        }

        UpdatePotionUI();
    }

    //guzik lewy górny róg
    private void CycleToPreviousFactory()
    {
        if (idleManager == null || idleManager.GetFactoryCount() == 0) return;

        audioSource.Play();
        int startIndex = currentFactoryIndex;
        int attempts = 0;
        
        do
        {
            currentFactoryIndex--;
            if (currentFactoryIndex < 0)
                currentFactoryIndex = idleManager.GetFactoryCount() - 1;
            
            attempts++;
            
            if (attempts >= idleManager.GetFactoryCount())
            {
                currentFactoryIndex = startIndex;
                return;
            }
            
        } while (!IsFactoryUnlocked(currentFactoryIndex));

        SwitchToFactory(currentFactoryIndex);
    }

    //guzik prawy górny róg
    private void CycleToNextFactory()
    {
        if (idleManager == null || idleManager.GetFactoryCount() == 0) return;

        audioSource.Play();
        int startIndex = currentFactoryIndex;
        int attempts = 0;
        
        do
        {
            currentFactoryIndex++;
            if (currentFactoryIndex >= idleManager.GetFactoryCount())
                currentFactoryIndex = 0;
            
            attempts++;
            
            if (attempts >= idleManager.GetFactoryCount())
            {
                currentFactoryIndex = startIndex;
                return;
            }
            
        } while (!IsFactoryUnlocked(currentFactoryIndex));
        
        SwitchToFactory(currentFactoryIndex);
    }

    private bool IsFactoryUnlocked(int factoryIndex)
    {
        var factory = GetFactory(factoryIndex);
        return factory != null && factory.isUnlocked;
    }

    private void SwitchToFactory(int factoryIndex)
    {
        var factory = GetFactory(factoryIndex);
        if (factory == null) return;

        raptorCore.SetCurrentResource(factory.resource.name);

        if (factoryLabel != null) factoryLabel.text = factory.name;

        Sprite Tex; 
        switch (factory.name)
        {
            case "F1": Tex = images[0]; break;
            case "F2": Tex = images[1]; break;
            case "F3": Tex = images[2]; break;
            case "F4": Tex = images[3]; break;
            case "F5": Tex = images[4]; break;
            case "F6": Tex = images[5]; break;
            default: Tex = images[0]; break;
        }

        currencyIcon.style.backgroundImage = new StyleBackground(Tex);
        shopIcon.style.backgroundImage = new StyleBackground(Tex);
        clickerObject.sprite = Tex;

        UpdateFactoryUI();
        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        if (prevFactoryBtn == null || nextFactoryBtn == null || idleManager == null) return;

        int unlockedCount = 0;
        for (int i = 0; i < idleManager.GetFactoryCount(); i++)
        {
            if (IsFactoryUnlocked(i))
                unlockedCount++;
        }

        bool hasMultipleUnlocked = unlockedCount > 1;
        
        prevFactoryBtn.SetEnabled(hasMultipleUnlocked);
        prevFactoryBtn.style.display = hasMultipleUnlocked ? DisplayStyle.Flex : DisplayStyle.None;
        nextFactoryBtn.SetEnabled(hasMultipleUnlocked);
        nextFactoryBtn.style.display = hasMultipleUnlocked ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void UpdateUnlockButtons()
    {
        if (idleManager == null || raptorCore == null) return;

        if (unlockFactory2Btn != null)
        {
            var factory2 = GetFactory(1);
            if (factory2 != null)
            {
                if (factory2.isUnlocked)
                {
                    unlockFactory2Btn.style.display = DisplayStyle.None;
                }
                else
                {
                    unlockFactory2Btn.style.display = DisplayStyle.Flex;
                    unlockFactory2Btn.text = $"Odblokuj {factory2.name}\n{factory2.unlockCost} Gold";
                    unlockFactory2Btn.SetEnabled(raptorCore.Gold >= factory2.unlockCost);
                }
            }
        }

        if (unlockFactory3Btn != null)
        {
            var factory3 = GetFactory(2);
            if (factory3 != null)
            {
                if (factory3.isUnlocked)
                {
                    unlockFactory3Btn.style.display = DisplayStyle.None;
                }
                else
                {
                    unlockFactory3Btn.style.display = DisplayStyle.Flex;
                    unlockFactory3Btn.text = $"Odblokuj {factory3.name}\n{factory3.unlockCost} Gold";
                    unlockFactory3Btn.SetEnabled(raptorCore.Gold >= factory3.unlockCost);
                }
            }
        }

        if (unlockFactory4Btn != null)
        {
            var factory4 = GetFactory(3);
            if (factory4 != null)
            {
                if (factory4.isUnlocked)
                {
                    unlockFactory4Btn.style.display = DisplayStyle.None;
                }
                else
                {
                    unlockFactory4Btn.style.display = DisplayStyle.Flex;
                    unlockFactory4Btn.text = $"Odblokuj {factory4.name}\n{factory4.unlockCost} Gold";
                    unlockFactory4Btn.SetEnabled(raptorCore.Gold >= factory4.unlockCost);
                }
            }
        }

        if (unlockFactory5Btn != null)
        {
            var factory5 = GetFactory(4);
            if (factory5 != null)
            {
                if (factory5.isUnlocked)
                {
                    unlockFactory5Btn.style.display = DisplayStyle.None;
                }
                else
                {
                    unlockFactory5Btn.style.display = DisplayStyle.Flex;
                    unlockFactory5Btn.text = $"Odblokuj {factory5.name}\n{factory5.unlockCost} Gold";
                    unlockFactory5Btn.SetEnabled(raptorCore.Gold >= factory5.unlockCost);
                }
            }
        }

        if (unlockFactory6Btn != null)
        {
            var factory6 = GetFactory(5);
            if (factory6 != null)
            {
                if (factory6.isUnlocked)
                {
                    unlockFactory6Btn.style.display = DisplayStyle.None;
                }
                else
                {
                    unlockFactory6Btn.style.display = DisplayStyle.Flex;
                    unlockFactory6Btn.text = $"Odblokuj {factory6.name}\n{factory6.unlockCost} Gold";
                    unlockFactory6Btn.SetEnabled(raptorCore.Gold >= factory6.unlockCost);
                }
            }
        }
    }
    private void UnlockFactoryFromButton(int factoryIndex)
    {
        audioSource.Play();

        if (idleManager == null) return;

        var factory = GetFactory(factoryIndex);
        if (factory == null) return;

        if (factory.isUnlocked)
        {
            Debug.Log($"Fabryka {factory.name} jest już odblokowana!");
            return;
        }

        bool success = idleManager.UnlockFactory(factoryIndex);

        if (success)
        {
            Debug.Log($"Pomyślnie odblokowano fabrykę {factory.name}");
             currentFactoryIndex = factoryIndex;
            SwitchToFactory(factoryIndex);
        }
        else
        {
            Debug.Log($"Brak złota na odblokowanie fabryki {factory.name}");
        }

        UpdateFactoryUI();
        UpdateUnlockButtons();
        UpdateNavigationButtons();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }
}