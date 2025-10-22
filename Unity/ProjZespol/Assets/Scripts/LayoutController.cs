using UnityEngine;
using UnityEngine.UIElements;

public class LayoutController : MonoBehaviour
{
    public static LayoutController Instance { get; private set; }
    
    [SerializeField] private RaptorCore raptorCore;
    
    
    public VisualElement ui;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Label text;
    public Label currencyLabel;
    public Label goldLabel;
    public Button sell1;
    public Button sell50;
    public Button sell100;

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
        sell50 = ui.Q<Button>("sell50");
        sell50.clicked += ClickSell50;
        sell100 = ui.Q<Button>("sell100");
        sell100.clicked += ClickSell100;


        OffSellButtons();
    }

    private void ClickBtn1()
    {
        audioSource.Play();
        text.style.display = DisplayStyle.None;
        sell1.style.display = DisplayStyle.Flex;
        sell50.style.display = DisplayStyle.Flex;
        sell100.style.display = DisplayStyle.Flex;
    }
    private void ClickBtn2()
    {
        audioSource.Play();
        text.style.display = DisplayStyle.Flex;
        text.text = "elo";
        OffSellButtons();
    }
    private void ClickBtn3()
    {
        audioSource.Play();
        text.style.display = DisplayStyle.Flex;
        text.text = "¿elo";
        OffSellButtons();
    }


    // przyciski zasobow
    private void ClickSell1()
    {
        raptorCore.SellMaterials(1);
    }
    private void ClickSell50()
    {
        raptorCore.SellMaterials(2);
    }
    private void ClickSell100()
    {
        raptorCore.SellMaterials(3);
    }

    public void OffSellButtons()
    {
        sell1.style.display = DisplayStyle.None;
        sell50.style.display = DisplayStyle.None;
        sell100.style.display = DisplayStyle.None;
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

    


}
