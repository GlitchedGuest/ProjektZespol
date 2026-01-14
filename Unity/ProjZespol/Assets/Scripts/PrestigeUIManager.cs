using UnityEngine;
using UnityEngine.UIElements;

public class PrestigeUIManager
{
    private VisualElement ui_ref;
    private VisualElement PrestigeRoot;

    private VisualElement ExpContainer;

    private VisualElement BarContainer;
    private VisualElement BarTexture;

    private Button PrestigeAccept;
    private Button PrestigeCancel;

    private AudioSource audioSource;

    private RaptorCore rp;

    private VisualElement contentPage;
    private Button PrestigeButtonMenu;

    public PrestigeUIManager(VisualElement ui, AudioSource audio, RaptorCore raptorcore)
    {
        rp = raptorcore;
        ui_ref = ui;
        audioSource = audio;
    }

    public void Initialize()
    {
        ExpContainer = ui_ref.Q<VisualElement>("ExpContainer");
        BarContainer = ExpContainer.Q<VisualElement>("background");
        BarTexture = ExpContainer.Q<VisualElement>("BarTexture");

        PrestigeRoot = ui_ref.Q<VisualElement>("Prestige");

        PrestigeAccept = PrestigeRoot.Q<Button>("Confirm");
        PrestigeCancel = PrestigeRoot.Q<Button>("Cancel");

        PrestigeAccept.clicked += PrestigeTrigger;
        PrestigeCancel.clicked += Hide;

        PrestigeAccept.clicked += audioSource.Play;
        PrestigeCancel.clicked += audioSource.Play;

        contentPage = ui_ref.Q<VisualElement>("Content7");

        PrestigeButtonMenu = ui_ref.Q<Button>("btn7");
        UpdateContentPage();
    }

    public void UpdateContentPage()
    {
        contentPage.Q<VisualElement>("PrestigeCount").Q<Label>("Num").text = rp.PrestigeCount.ToString();
        contentPage.Q<VisualElement>("OneClick").Q<Label>("Num").text = rp.AutoPrestige.ToString();
        contentPage.Q<VisualElement>("Auto").Q<Label>("Num").text = rp.JackPrestige.ToString();
        contentPage.Q<VisualElement>("Jack").Q<Label>("Num").text = rp.OneClickPrestige.ToString();
        
        if (rp.PrestigeCount != 0) PrestigeButtonMenu.style.display = DisplayStyle.Flex;

    }

    public void EnablePrestige() {
        ExpContainer.RegisterCallback<ClickEvent>(Trigger);
        BarContainer.style.unityBackgroundImageTintColor = new Color(1f, 0.6745f, 0f);
        BarTexture.style.unityBackgroundImageTintColor = new Color(1f, 0.6745f, 0f);

    }
    public void DisablePrestige() {
        ExpContainer.UnregisterCallback<ClickEvent>(Trigger);
        BarContainer.style.unityBackgroundImageTintColor = Color.white;
        BarTexture.style.unityBackgroundImageTintColor = Color.white;
    }

    public void Trigger(ClickEvent evt)
    {
        audioSource.Play();
        PrestigeRoot.style.display = DisplayStyle.Flex;
        evt.StopPropagation();
    }

    public void Hide() {
        audioSource.Play();
        PrestigeRoot.style.display = DisplayStyle.None;
    }
    public void PrestigeTrigger() {
        DisablePrestige();
        PrestigeSystem.ExecutePrestige();
        Hide();
    }


}
