using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;

public class LayoutController : MonoBehaviour
{
    public static LayoutController Instance { get; private set; }
    
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private IdleManager idleManager;
    
    public VisualElement ui;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button btn5;
    public Label text;
    public Label currencyLabel;
    public Label goldLabel;
    public Button sell1;
    public Slider currencySlider;

    public VisualElement ContentPage1; // karta zasobow
    public VisualElement ContentPage2;
    public VisualElement ContentPage3;
    public VisualElement ContentPage4;
    public VisualElement ContentPage5;

    //Lvlbar
    public ProgressBar LvlBar;
    public Label LvlNumber;

    private class FactoryUI
    {
        public Button toggleButton;
        public VisualElement detailsPanel;
        public Label nameLabel;
        public Label countLabel;
        public Label costLabel;
        public Button[] buyButtons = new Button[4]; // 1, 5, 25, MAX
        public Button[] sellButtons = new Button[4]; // 1, 5, 25, MAX
        public bool isVisible = false;
    }

    public ScrollView scrollView;
    private List<FactoryUI> factoryUIs = new List<FactoryUI>();
    private int numberOfFactories = 3; // Can add more at later stages of development

    [SerializeField] private AudioSource audioSource;


    //drzewka
    private Label skillDescription;
    private Dictionary<string, Button> buttons = new();
    private HashSet<string> learned = new();

    private Label skillDescription2;
    private Dictionary<string, Button> buttons2 = new();
    private HashSet<string> learned2 = new();


    private Label skillDescription3;
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


    void Awake()
    {
        Instance = this;
        ui = GetComponent<UIDocument>().rootVisualElement;
        var value = PlayerPrefs.GetFloat("SfxVolume", 1f);
        audioSource.volume = value;

    }

    private void OnEnable()
    {
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


        // zasoby
        sell1 = ui.Q<Button>("sell1");
        sell1.clicked += ClickSell1;

        ContentPage1 = ui.Q<VisualElement>("Content");
        ContentPage2 = ui.Q<VisualElement>("Content2");
        ContentPage3 = ui.Q<VisualElement>("Content3");
        ContentPage4 = ui.Q<VisualElement>("Content4");
        ContentPage5 = ui.Q<VisualElement>("Content5");

        // lvlbar
        LvlNumber = ui.Q<Label>("lvlNumber");
        LvlBar = ui.Q<ProgressBar>("lvlprog");

        // Inicjalizacja fabryk
        scrollView = ui.Q<ScrollView>("ScrollView");
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        InitializeFactories();


        //drzewka tego typu

        skillDescription = ui.Q<Label>("Describe");
        skillDescription2 = ui.Q<Label>("Describe2");
        skillDescription3 = ui.Q<Label>("Describe3");

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

        
        RegisterSkillHover(skill1, "Umożliwia pojawienie się skill checków(nie udany kosztuje gracza zwolnieniem idle produkcji)",skillDescription);
        RegisterSkillHover(skill2A, "Zwiększa szanse na kliki krytyczne", skillDescription);
        RegisterSkillHover(skill2B, "Udany skill check zwiększa ilość zbieranych punktów na chwilę(nie udany zmniejsza)", skillDescription);

        RegisterSkillHover(skill3A, "Każdy klik krytyczny chwilowo zwiększa idle produkcje", skillDescription);
        RegisterSkillHover(skill3B, "Skill checki częściej się pojawiają", skillDescription);

        RegisterSkillHover(skill4A, "Stakuje szanse na klik krytyczny(każde kliknięcie niekrytyczne zwiększa szanse na krytyczne)", skillDescription);
        RegisterSkillHover(skill4B, "Skill checki nie mają negatywnych skutków po przegraniu", skillDescription);

        RegisterSkillHover(skill5, "Częstotliwość skill checka jest zależna od ilości klików krytycznych(im częściej są tym częściej skill checki)", skillDescription);

        RegisterSkillHover(skill6A, "Każdy kolejny skill check łączy się w kombos. Im większy kombos tym więcej punktów za klik(im większy kombos też trudniejsze skill checki)", skillDescription);
        RegisterSkillHover(skill6B, "Po trzech udanych skill checkach z rzędu przez krótki moment są same kliki krytyczne", skillDescription);

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

        RegisterSkillHover(skill1, "Można sprzedawać po wyższych cenach „punkty", skillDescription2);
        RegisterSkillHover(skill2, "Udany skill check chwilowo podwyższa ceny sprzedaży w sklepie", skillDescription2);
        RegisterSkillHover(skill3, "Potki mają zwiększoną skuteczność", skillDescription2);
        RegisterSkillHover(skill4, "Nieudany skill check daje możliwość zdobycia jednej losowej potki", skillDescription2);
        RegisterSkillHover(skill5, "Im więcej udanych skill checków tym ceny będą wyższe", skillDescription2);
        RegisterSkillHover(skill6, "Efekty potek się stakują", skillDescription2);
        RegisterSkillHover(skill7, "Nieudany skill check gwarantuje zdobycie jednej losowej potki", skillDescription2);
        RegisterSkillHover(skill8, "Nieudany skill check zwiększa chwilowo przyrost exp z akcji", skillDescription2);
        RegisterSkillHover(skill9, "im więcej nieudanych skill checków tym bonus jest większy", skillDescription2);
        RegisterSkillHover(skill10, "Automatyczna sprzedaż wszystkich punktów po osiągnięciu limitu fabryki", skillDescription2);

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

        RegisterSkillHover(skill1, "Im więcej fabryk gracz posiada tym większy bonus do idle dla fabryki in focus", skillDescription3);
        RegisterSkillHover(skill2, "Limit punktów na fabrykę zostaje zwiększony", skillDescription3);
        RegisterSkillHover(skill3, "Skill checki mogą się pojawiać w solowym momencie niezależnie od tego czy gracz kilka czy nie", skillDescription3);
        RegisterSkillHover(skill4, "Fabryki not in Focus mają większy bonus do produkcji Idle", skillDescription3);
        RegisterSkillHover(skill5, "Nieudany skill check zwiększa produkcje idle do momentu udanego skill checka(stackuje się)", skillDescription3);
        RegisterSkillHover(skill6, "Im dłużej gracz nie wykona akcji myszką tym więcej punktów zacznie się naliczać", skillDescription3);
        RegisterSkillHover(skill7, "Nieudany skill check zwiększa produkcje wszystkich fabryk", skillDescription3);
        RegisterSkillHover(skill8, "Gracz może robić inne akcje poza klikanie w obiekt", skillDescription3);
        RegisterSkillHover(skill9, "Bonus aplikuje się do każdej fabryki not in focus", skillDescription3);
        RegisterSkillHover(skill10, "Bonus zależy od ilość posiadanych już puntków(im mniej tym większy bonus)", skillDescription3);

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

            // Buy
            factoryUI.buyButtons[0] = ui.Q<Button>($"Factory{i}Buy1");
            factoryUI.buyButtons[1] = ui.Q<Button>($"Factory{i}Buy5");
            factoryUI.buyButtons[2] = ui.Q<Button>($"Factory{i}Buy25");
            factoryUI.buyButtons[3] = ui.Q<Button>($"Factory{i}BuyMax");

            // Sell
            factoryUI.sellButtons[0] = ui.Q<Button>($"Factory{i}Sell1");
            factoryUI.sellButtons[1] = ui.Q<Button>($"Factory{i}Sell5");
            factoryUI.sellButtons[2] = ui.Q<Button>($"Factory{i}Sell25");
            factoryUI.sellButtons[3] = ui.Q<Button>($"Factory{i}SellMax");

            int factoryIndex = i - 1;

            if (factoryUI.toggleButton != null)
            {
                factoryUI.toggleButton.clicked += () => ToggleFactoryDetails(factoryIndex);
            }

            // Buy
            if (factoryUI.buyButtons[0] != null)
                factoryUI.buyButtons[0].clicked += () => BuyFactory(factoryIndex, 1);
            if (factoryUI.buyButtons[1] != null)
                factoryUI.buyButtons[1].clicked += () => BuyFactory(factoryIndex, 5);
            if (factoryUI.buyButtons[2] != null)
                factoryUI.buyButtons[2].clicked += () => BuyFactory(factoryIndex, 25);
            if (factoryUI.buyButtons[3] != null)
                factoryUI.buyButtons[3].clicked += () => BuyFactoryMax(factoryIndex);

            // Sell
            if (factoryUI.sellButtons[0] != null)
                factoryUI.sellButtons[0].clicked += () => SellFactory(factoryIndex, 1);
            if (factoryUI.sellButtons[1] != null)
                factoryUI.sellButtons[1].clicked += () => SellFactory(factoryIndex, 5);
            if (factoryUI.sellButtons[2] != null)
                factoryUI.sellButtons[2].clicked += () => SellFactory(factoryIndex, 25);
            if (factoryUI.sellButtons[3] != null)
                factoryUI.sellButtons[3].clicked += () => SellFactoryMax(factoryIndex);

            factoryUIs.Add(factoryUI);
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
    }
    
    private void ClickBtn2()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.Flex;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.None;
    }
    
    private void ClickBtn3()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.Flex;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.None;
    }
    private void ClickBtn4()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.Flex;
        ContentPage5.style.display = DisplayStyle.None;
    }
    private void ClickBtn5()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
        ContentPage4.style.display = DisplayStyle.None;
        ContentPage5.style.display = DisplayStyle.Flex;

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

        UpdateAll();

    }

    private void UpdateLvlBar()
    {
        LvlNumber.text = characterClass.GetLevel().ToString();
        LvlBar.value = characterClass.GetCurrentExp();
        LvlBar.highValue = characterClass.GetMaxExpCap();
    }

    private void ToggleFactoryDetails(int factoryIndex)
    {
        audioSource.Play();

        if (factoryIndex < 0 || factoryIndex >= factoryUIs.Count)
        {
            return;
        }

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
                Debug.Log($"Kupiono tylko {i} z {amount} fabryk");
                break;
            }
        }

        UpdateFactoryUI();
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

        while (playerGold >= currentCost && maxAmount < 100000) // Limiter
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
    }

    private void UpdateFactoryUI()
    {
        if (idleManager == null) return;

        for (int i = 0; i < factoryUIs.Count; i++)
        {
            var factoryUI = factoryUIs[i];
            var factory = GetFactory(i);

            if (factory == null || factoryUI.countLabel == null || factoryUI.costLabel == null)
                continue;

            factoryUI.countLabel.text = $"Ilość: {factory.count}";
            factoryUI.costLabel.text = $"Koszt: {factory.currentCost:F2}";

            double playerGold = raptorCore.Gold;

            UpdateButton(factoryUI.buyButtons[0], playerGold >= factory.currentCost);
            UpdateButton(factoryUI.buyButtons[1], playerGold >= factory.currentCost * 5);
            UpdateButton(factoryUI.buyButtons[2], playerGold >= factory.currentCost * 10);
            UpdateButton(factoryUI.buyButtons[3], playerGold >= factory.currentCost);

            UpdateButton(factoryUI.sellButtons[0], factory.count >= 1);
            UpdateButton(factoryUI.sellButtons[1], factory.count >= 5);
            UpdateButton(factoryUI.sellButtons[2], factory.count >= 10);
            UpdateButton(factoryUI.sellButtons[3], factory.count > 0);
        }
        SetGoldText(raptorCore.Gold.ToString());
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
        if (idleManager == null)
            return null;

        var factoriesField = typeof(IdleManager).GetField("factories", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (factoriesField != null)
        {
            var factories = factoriesField.GetValue(idleManager) as List<Factory>;
            if (factories != null && index >= 0 && index < factories.Count)
            {
                return factories[index];
            }
        }

        return null;
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


}