using UnityEngine;
using UnityEngine.UIElements;

public class LevelUIManager
{
    private VisualElement ui;
    private CharacterClass characterClass;

    private VisualElement barMask;
    private Label lvlNumber;
    private VisualElement expBarContainer;
    private VisualElement barTexture;

    public LevelUIManager(VisualElement root, CharacterClass charClass)
    {
        ui = root;
        characterClass = charClass;
    }

    public void Initialize()
    {
        lvlNumber = ui.Q<Label>("lvlNumber");
        barMask = ui.Q<VisualElement>("BarMask");
        expBarContainer = ui.Q<VisualElement>("ExpBarContainer");
        barTexture = ui.Q<VisualElement>("BarTexture");
    }

    public void Update()
    {
        UpdateLvlBar();
    }

    private void UpdateLvlBar()
    {
        lvlNumber.text = characterClass.GetLevel().ToString();

        barTexture.style.minWidth = expBarContainer.resolvedStyle.width;
        barTexture.style.maxWidth = expBarContainer.resolvedStyle.width;
        barTexture.style.width = expBarContainer.resolvedStyle.width;

        float currentExp = characterClass.GetCurrentExp();
        float maxExp = characterClass.GetMaxExpCap();
        float progress = Mathf.Clamp01(currentExp / maxExp);

        barMask.style.width = new Length(progress * 100f, LengthUnit.Percent);
    }
}