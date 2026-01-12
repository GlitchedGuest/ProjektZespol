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
    [SerializeField] private Sprite cuatroPotionSprite;
    [SerializeField] private Sprite cincoPotionSprite;
    [SerializeField] private Sprite seisPotionSprite;


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
        potionDisplay.style.marginBottom = 10;
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

        int activeCount = 0;
        foreach (var p in idleManager.potions)
        {
            if (p.isActive) activeCount++;
        }
        var icon = new VisualElement();
        if (activeCount < 4)
        {
            Sprite potionSprite = GetPotionSprite(potion.name);

            icon = new VisualElement();
            icon.style.width = 80;
            icon.style.height = 80;
            icon.style.marginBottom = 5;

            if (potionSprite != null)
            {
                icon.style.backgroundImage = new StyleBackground(potionSprite);
                icon.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
            }
            else
            {
                icon.style.backgroundColor = borderColor;
            }

            icon.style.borderTopLeftRadius = 5;
            icon.style.borderTopRightRadius = 5;
            icon.style.borderBottomLeftRadius = 5;
            icon.style.borderBottomRightRadius = 5;


            potionDisplay.Add(icon);
        }

        Label stackLabel = null;
        if (potion.boughtCount > 1)
        {
            stackLabel = new Label($"x{potion.boughtCount}");
            stackLabel.style.fontSize = 20;
            stackLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            stackLabel.style.color = Color.white;
            stackLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            stackLabel.style.marginBottom = 4;
            stackLabel.name = $"PotionStack{index}";
        }

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

        if (stackLabel != null)
            potionDisplay.Add(stackLabel);

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
            var stackLabel = activePotionsPanel.Q<Label>($"PotionStack{i}");
            if (stackLabel != null)
            {
                if (potion.boughtCount > 1)
                    stackLabel.text = $"x{potion.boughtCount}";
                else
                    stackLabel.style.display = DisplayStyle.None;
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
            case "Cuatro":
                return cuatroPotionSprite;
            case "Cinco":
                return cincoPotionSprite;
            case "Seis":
                return seisPotionSprite;
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
            case "Cuatro":
                return new Color(1f, 0.3f, 0.3f);
            case "Cinco":
                return new Color(0.3f, 0.3f, 1f);
            case "Seis":
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