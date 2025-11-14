using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ActivePotionsUI : MonoBehaviour
{
    [SerializeField] private IdleManager idleManager;
    [SerializeField] private UIDocument uiDocument;

    [Header("Potki Sprity")]
    [SerializeField] private Sprite unoPotionSprite;
    [SerializeField] private Sprite dosPotionSprite;
    [SerializeField] private Sprite tresPotionSprite;

    private VisualElement activePotionsPanel;
    private List<VisualElement> activePotionDisplays = new List<VisualElement>();

    private void Start()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (uiDocument != null)
        {
            var root = uiDocument.rootVisualElement;
            activePotionsPanel = root.Q<VisualElement>("ActivePotionsPanel");
        }
    }

    private void Update()
    {
        UpdateActivePotionsDisplay();
        UpdatePotionTimers();
    }

    private void UpdateActivePotionsDisplay()
    {
        if (activePotionsPanel == null || idleManager == null) return;

        int activeCount = 0;
        foreach (var potion in idleManager.potions)
        {
            if (potion.isActive) activeCount++;
        }

        if (activeCount != activePotionDisplays.Count)
        {
            RebuildPanel();
        }
    }

    private void RebuildPanel()
    {
        if (activePotionsPanel == null || idleManager == null) return;

        activePotionsPanel.Clear();
        activePotionDisplays.Clear();

        for (int i = 0; i < idleManager.potions.Count; i++)
        {
            var potion = idleManager.potions[i];
            if (potion != null && potion.isActive)
            {
                CreatePotionDisplay(potion, i);
            }
        }
    }

    private void CreatePotionDisplay(Potion potion, int index)
    {
        var potionDisplay = new VisualElement();
        potionDisplay.style.flexDirection = FlexDirection.Column;
        potionDisplay.style.alignItems = Align.Center;
        potionDisplay.style.marginBottom = 15;
        potionDisplay.style.paddingTop = 10;
        potionDisplay.style.backgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        potionDisplay.style.borderTopLeftRadius = 10;
        potionDisplay.style.borderTopRightRadius = 10;
        potionDisplay.style.borderBottomLeftRadius = 10;
        potionDisplay.style.borderBottomRightRadius = 10;
        potionDisplay.style.borderLeftWidth = 2;
        potionDisplay.style.borderRightWidth = 2;
        potionDisplay.style.borderTopWidth = 2;
        potionDisplay.style.borderBottomWidth = 2;
        
        Color borderColor = GetPotionColor(potion.name);
        potionDisplay.style.borderLeftColor = borderColor;
        potionDisplay.style.borderRightColor = borderColor;
        potionDisplay.style.borderTopColor = borderColor;
        potionDisplay.style.borderBottomColor = borderColor;

        Sprite potionSprite = GetPotionSprite(potion.name);

        var icon = new VisualElement();
        icon.style.width = 80;
        icon.style.height = 80;
        icon.style.marginBottom = 5;

        if (potionSprite != null)
        {
            icon.style.backgroundImage = new StyleBackground(potionSprite);
        }
        else
        {
            icon.style.backgroundColor = borderColor;
        }

        icon.style.borderTopLeftRadius = 5;
        icon.style.borderTopRightRadius = 5;
        icon.style.borderBottomLeftRadius = 5;
        icon.style.borderBottomRightRadius = 5;

        var nameLabel = new Label(potion.name);
        nameLabel.style.fontSize = 16;
        nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        nameLabel.style.color = Color.white;
        nameLabel.style.whiteSpace = WhiteSpace.Normal;
        nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        nameLabel.style.marginBottom = 5;

        var timerLabel = new Label($"{potion.timeRemaining:F1}s");
        timerLabel.style.fontSize = 20;
        timerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        timerLabel.style.color = new Color(0.3f, 1f, 0.3f);
        timerLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        timerLabel.name = $"PotionTimer{index}";

        potionDisplay.Add(icon);
        potionDisplay.Add(nameLabel);
        potionDisplay.Add(timerLabel);

        activePotionsPanel.Add(potionDisplay);
        activePotionDisplays.Add(potionDisplay);
    }

    private void UpdatePotionTimers()
    {
        if (idleManager == null || activePotionsPanel == null) return;

        for (int i = 0; i < idleManager.potions.Count; i++)
        {
            var potion = idleManager.potions[i];
            if (potion != null && potion.isActive)
            {
                var timerLabel = activePotionsPanel.Q<Label>($"PotionTimer{i}");
                if (timerLabel != null)
                {
                    timerLabel.text = $"{potion.timeRemaining:F1}s";

                    if (potion.timeRemaining < 5)
                    {
                        timerLabel.style.color = new Color(1f, 0.3f, 0.3f);
                    }
                    else if (potion.timeRemaining < 10)
                    {
                        timerLabel.style.color = new Color(1f, 1f, 0.3f);
                    }
                    else
                    {
                        timerLabel.style.color = new Color(0.3f, 1f, 0.3f);
                    }
                }
            }
        }
    }

    private Sprite GetPotionSprite(string potionName)
    {
        switch (potionName)
        {
            case "Uno":
                return unoPotionSprite;
            case "Dos":
                return dosPotionSprite;
            case "Tres":
                return tresPotionSprite;
            default:
                return null;
        }
    }

    private Color GetPotionColor(string potionName)
    {
        switch (potionName)
        {
            case "Uno":
                return new Color(1f, 0.3f, 0.3f);
            case "Dos":
                return new Color(0.3f, 0.3f, 1f);
            case "Tres":
                return new Color(0.3f, 1f, 0.3f);
            default:
                return new Color(0.8f, 0.3f, 0.8f);
        }
    }

    public void RefreshPanel()
    {
        RebuildPanel();
    }
}