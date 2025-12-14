using UnityEngine;
using UnityEngine.UIElements;

public class ResourceShopManager
{
    private VisualElement ui;
    private RaptorCore raptorCore;
    private CharacterClass characterClass;
    private AudioSource audioSource;

    private Label currencyLabel;
    private Label goldLabel;
    private Slider currencySlider;
    private Label sliderText;
    private Button sell1;

    public ResourceShopManager(VisualElement root, RaptorCore core, CharacterClass charClass, AudioSource audio)
    {
        ui = root;
        raptorCore = core;
        characterClass = charClass;
        audioSource = audio;
    }

    public void Initialize()
    {
        currencyLabel = ui.Q<Label>("Currency");
        goldLabel = ui.Q<Label>("Money");
        currencySlider = ui.Q<Slider>("AmoutSlider");
        sliderText = ui.Q<Label>("Napis");
        sell1 = ui.Q<Button>("sell1");

        sell1.clicked += ClickSell1;
    }

    public void Update()
    {
        if (sliderText != null)
            sliderText.text = currencySlider.value.ToString() + "%";
    }

    private void ClickSell1()
    {
        double exp = raptorCore.SellManager.SellMaterials(currencySlider.value);
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
}