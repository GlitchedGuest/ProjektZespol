using UnityEngine;
using UnityEngine.UIElements;

public class LayoutController : MonoBehaviour
{
    public static LayoutController Instance { get; private set; }
    
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private CharacterClass characterClass;
    
    
    public VisualElement ui;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Label text;
    public Label currencyLabel;
    public Label goldLabel;
    public Button sell1;
    public Button addfactory;
    public Slider currencySlider;

    public VisualElement ContentPage1; // karta zasobow
    public VisualElement ContentPage2;
    public VisualElement ContentPage3;

    //Lvlbar

    public ProgressBar LvlBar;
    public Label LvlNumber;


    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        Instance = this;
        ui = GetComponent<UIDocument>().rootVisualElement;
        
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

        // zasoby

        sell1 = ui.Q<Button>("sell1");
        sell1.clicked += ClickSell1;

        ContentPage1 = ui.Q<VisualElement>("Content");
        ContentPage2 = ui.Q<VisualElement>("Content2");
        ContentPage3 = ui.Q<VisualElement>("Content3");

        // lvlbar

        LvlNumber = ui.Q<Label>("lvlNumber");
        LvlBar = ui.Q<ProgressBar>("lvlprog");

        //factory button

        addfactory = ui.Q<Button>("addfactory");
        addfactory.clicked += AddFactory;
    }

    private void ClickBtn1()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.Flex;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.None;
    }
    private void ClickBtn2()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.Flex;
        ContentPage3.style.display = DisplayStyle.None;
    }
    private void ClickBtn3()
    {
        audioSource.Play();
        ContentPage1.style.display = DisplayStyle.None;
        ContentPage2.style.display = DisplayStyle.None;
        ContentPage3.style.display = DisplayStyle.Flex;
    }


    // przyciski zasobow
    private void ClickSell1()
    {
        double exp= raptorCore.SellMaterials(currencySlider.value);
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
        text.text = currencySlider.value.ToString()+"%";
        UpdateLvlBar();
    }
    private void UpdateLvlBar()
    {
        LvlNumber.text = characterClass.GetLevel().ToString();
        LvlBar.value = characterClass.GetCurrentExp();
        LvlBar.highValue = characterClass.GetMaxExpCap();

    }

    private void AddFactory()
    {
        audioSource.Play();
        //TODO
    }


}
