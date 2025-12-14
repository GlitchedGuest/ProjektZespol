using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FactoryUIManager
{
    private class FactoryUI
    {
        public Button toggleButton;
        public VisualElement detailsPanel;
        public Label nameLabel;
        public Label countLabel;
        public Label costLabel;
        public Button unlockButton;
        public Button[] buyButtons = new Button[4];
        public Button[] sellButtons = new Button[4];
        public bool isVisible = false;

        public Button upgradeMainBtn;
        public VisualElement upgradePanel;
        public Button upgradeProductionBtn;
        public Button upgradeMultiplierBtn;
        public Label productionLevelLabel;
        public Label multiplierLevelLabel;
        public bool upgradesVisible = false;
    }

    private class FactoryUpgradeLevels
    {
        public int productionLevel = 0;
        public int multiplierLevel = 0;
        public double baseProductionCost = 0;
        public double baseMultiplierCost = 0;
    }

    private VisualElement ui;
    private ScrollView scrollView;
    private List<FactoryUI> factoryUIs = new List<FactoryUI>();
    private Dictionary<int, FactoryUpgradeLevels> factoryUpgradeLevels = new Dictionary<int, FactoryUpgradeLevels>();
    private int numberOfFactories = 6;
    private int currentFactoryIndex = 0;

    private RaptorCore raptorCore;
    private IdleManager idleManager;
    private AudioSource audioSource;

    // Navigation
    private Button prevFactoryBtn;
    private Button nextFactoryBtn;
    private Label factoryLabel;
    private VisualElement currencyIcon;
    private VisualElement shopIcon;
    private SpriteRenderer clickerObject;
    private Sprite[] images;

    // Unlock buttons
    private Button unlockFactory2Btn;
    private Button unlockFactory3Btn;
    private Button unlockFactory4Btn;
    private Button unlockFactory5Btn;
    private Button unlockFactory6Btn;

    public FactoryUIManager(VisualElement root, RaptorCore core, IdleManager manager, AudioSource audio, 
        SpriteRenderer clicker, Sprite[] factoryImages)
    {
        ui = root;
        raptorCore = core;
        idleManager = manager;
        audioSource = audio;
        clickerObject = clicker;
        images = factoryImages;
    }

    public void Initialize()
    {
        scrollView = ui.Q<ScrollView>("ScrollView1");
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

        currencyIcon = ui.Q<VisualElement>("cbbleicon");
        shopIcon = ui.Q<VisualElement>("shopcbbleicon");
        factoryLabel = ui.Q<Label>("FactoryLabel");

        prevFactoryBtn = ui.Q<Button>("PrevFactoryBtn");
        nextFactoryBtn = ui.Q<Button>("NextFactoryBtn");

        if (prevFactoryBtn != null)
            prevFactoryBtn.clicked += CycleToPreviousFactory;
        if (nextFactoryBtn != null)
            nextFactoryBtn.clicked += CycleToNextFactory;

        InitializeUnlockButtons();
        InitializeFactories();
        SwitchToFactory(currentFactoryIndex);
    }

    private void InitializeUnlockButtons()
    {
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
                factoryUI.unlockButton.clicked += () => UnlockFactory(factoryIndexCopy);

            if (factoryUI.toggleButton != null)
                factoryUI.toggleButton.clicked += () => ToggleFactoryDetails(factoryIndex);

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

    public void Update()
    {
        UpdateFactoryUI();
        UpdateNavigationButtons();
        UpdateUnlockButtons();
    }

    private void ToggleFactoryDetails(int factoryIndex)
    {
        audioSource.Play();

        if (factoryIndex < 0 || factoryIndex >= factoryUIs.Count)
            return;

        var factoryUI = factoryUIs[factoryIndex];
        factoryUI.isVisible = !factoryUI.isVisible;

        if (factoryUI.detailsPanel != null)
            factoryUI.detailsPanel.style.display = factoryUI.isVisible ? DisplayStyle.Flex : DisplayStyle.None;
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
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

        while (playerGold >= currentCost && maxAmount < 1000)
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
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
                    factoryUI.productionLevelLabel.text = $"Poziom: {upgradeLevels.productionLevel}";

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
            button.SetEnabled(enabled);
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
            Debug.Log($"Pomyślnie odblokowano fabrykę {factoryIndex}");
        else
            Debug.Log($"Brak kasy na odblokowanie fabryki {factoryIndex}");

        UpdateFactoryUI();
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
        UpdateFactoryUI();
    }

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

        raptorCore.ResourceManager.SetCurrentResource(factory.resource.name);

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

        UpdateUnlockButton(unlockFactory2Btn, 1);
        UpdateUnlockButton(unlockFactory3Btn, 2);
        UpdateUnlockButton(unlockFactory4Btn, 3);
        UpdateUnlockButton(unlockFactory5Btn, 4);
        UpdateUnlockButton(unlockFactory6Btn, 5);
    }

    private void UpdateUnlockButton(Button button, int factoryIndex)
    {
        if (button == null) return;

        var factory = GetFactory(factoryIndex);
        if (factory != null)
        {
            if (factory.isUnlocked)
            {
                button.style.display = DisplayStyle.None;
            }
            else
            {
                button.style.display = DisplayStyle.Flex;
                button.text = $"Odblokuj {factory.name}\n{factory.unlockCost} Gold";
                button.SetEnabled(raptorCore.Gold >= factory.unlockCost);
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
        LayoutController.Instance.SetGoldText(raptorCore.Gold.ToString("F0"));
    }
}