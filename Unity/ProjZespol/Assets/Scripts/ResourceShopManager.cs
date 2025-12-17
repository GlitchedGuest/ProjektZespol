using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceShopManager
{
    private VisualElement ui;
    private RaptorCore raptorCore;
    private CharacterClass characterClass;
    private AudioSource audioSource;

    private Label res1curr;
    private Label res2curr;
    private Label res3curr;
    private Label res4curr;
    private Label res5curr;
    private Label res6curr;


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
        res1curr = ui.Q<Label>("res1curr");
        res2curr = ui.Q<Label>("res2curr");
        res3curr = ui.Q<Label>("res3curr");
        res4curr = ui.Q<Label>("res4curr");
        res5curr = ui.Q<Label>("res5curr");
        res6curr = ui.Q<Label>("res6curr");

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

    public void SetCurrencyText(List<QuarkType> list)
    {
        if (list != null) {
            if (list[0] != null) res1curr.text = list[0].ToString();
            if (list[1] != null) res2curr.text = list[1].ToString();
            if (list[2] != null) res3curr.text = list[2].ToString();
            if (list[3] != null) res4curr.text = list[3].ToString();
            if (list[4] != null) res5curr.text = list[4].ToString();
            if (list[5] != null) res6curr.text = list[5].ToString();

        }
       // if (currencyLabel != null)
         //   currencyLabel.text = value;
    }

    public void SetGoldText(string value)
    {
        if (goldLabel != null)
            goldLabel.text = value;
    }
}