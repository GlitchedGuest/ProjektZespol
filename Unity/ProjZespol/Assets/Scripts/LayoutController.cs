using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class LayoutController : MonoBehaviour
{
    public static LayoutController Instance { get; private set; }

    [SerializeField] private SpriteRenderer clickerObject;
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private IdleManager idleManager;
    
    public VisualElement ui;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button resourceBtn;
    public Label text;
    public VisualElement currencyIcon;
    public Label currencyLabel;
    public Label goldLabel;
    public VisualElement shopIcon;
    public Button sell1;
    public Slider currencySlider;
    public VisualElement ContentPage1; // karta zasobow
    public VisualElement ContentPage2;
    public VisualElement ContentPage3;

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
        public Button unlockButton;
        public Button[] buyButtons = new Button[4]; // 1, 5, 25, MAX
        public Button[] sellButtons = new Button[4]; // 1, 5, 25, MAX
        public bool isVisible = false;

        public Button upgradeMainBtn;
        public VisualElement upgradePanel;
        public Button upgradeProductionBtn;
        public Button upgradeMultiplierBtn;
        public Label productionLevelLabel;
        public Label multiplierLevelLabel;
        public bool upgradesVisible = false;
    }
    private Dictionary<int, FactoryUpgradeLevels> factoryUpgradeLevels = new Dictionary<int, FactoryUpgradeLevels>();

    private class FactoryUpgradeLevels
    {
        public int productionLevel = 0;
        public int multiplierLevel = 0;
        public double baseProductionCost = 0;
        public double baseMultiplierCost = 0;
    }

    public ScrollView scrollView;
    private List<FactoryUI> factoryUIs = new List<FactoryUI>();
    private int numberOfFactories = 3;
    private int currentFactoryIndex = 0;

    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        currencyIcon = ui.Q<VisualElement>("cbbleicon");
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
        resourceBtn = ui.Q<Button>("ResourceButton");
        resourceBtn.clicked += CycleCurrency;


        // zasoby
        shopIcon = ui.Q<VisualElement>("shopcbbleicon");
        sell1 = ui.Q<Button>("sell1");
        sell1.clicked += ClickSell1;

        ContentPage1 = ui.Q<VisualElement>("Content");
        ContentPage2 = ui.Q<VisualElement>("Content2");
        ContentPage3 = ui.Q<VisualElement>("Content3");

        // lvlbar
        LvlNumber = ui.Q<Label>("lvlNumber");
        LvlBar = ui.Q<ProgressBar>("lvlprog");

        scrollView = ui.Q<ScrollView>("ScrollView");
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        InitializeFactories();
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
            {
                factoryUI.unlockButton.clicked += () => UnlockFactory(factoryIndexCopy);
            }

            
            if (factoryUI.toggleButton != null)
            {
                factoryUI.toggleButton.clicked += () => ToggleFactoryDetails(factoryIndex);
            }

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
            return;

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
                Debug.Log($"Kupiono tylko {i} z {amount} fabryk (brak środków)");
                break;
            }
        }

        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
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

        while (playerGold >= currentCost && maxAmount < 1000) // limit bezpieczeństwa
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
        SetGoldText(raptorCore.Gold.ToString("F0"));
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
        SetGoldText(raptorCore.Gold.ToString("F0"));
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
        SetGoldText(raptorCore.Gold.ToString("F0"));
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
                //double multiplierCost = CalculateUpgradeCost(upgradeLevels.baseMultiplierCost, upgradeLevels.multiplierLevel);

                if (!factory.isUnlocked)
                {
                    factoryUI.upgradePanel.style.display = DisplayStyle.None;
                }
                else
                {
                    factoryUI.upgradePanel.style.display = factoryUI.upgradesVisible ? DisplayStyle.Flex : DisplayStyle.None;
                }

                if (factoryUI.productionLevelLabel != null && factory.isUnlocked)
                {
                    factoryUI.productionLevelLabel.text = $"Poziom: {upgradeLevels.productionLevel}";
                }
                    
                
                factoryUI.multiplierLevelLabel.style.display = DisplayStyle.None;
                //if (factoryUI.multiplierLevelLabel != null)
                //    factoryUI.multiplierLevelLabel.text = $"Poziom: {upgradeLevels.multiplierLevel}";

                if (factoryUI.upgradeProductionBtn != null && factory.isUnlocked)
                {
                    factoryUI.upgradeProductionBtn.text = $"Zwiększ produkcję (+1) - {productionCost:F0} G";
                    factoryUI.upgradeProductionBtn.SetEnabled(playerGold >= productionCost);
                }
                factoryUI.upgradeMultiplierBtn.style.display = DisplayStyle.None;
                //if (factoryUI.upgradeMultiplierBtn != null)
                //{
                //    factoryUI.upgradeMultiplierBtn.text = $"Zwiększ mnożnik (+0.1x) - {multiplierCost:F0} G";
                //    factoryUI.upgradeMultiplierBtn.SetEnabled(playerGold >= multiplierCost);
                //}
            }


            if (!factory.isUnlocked)
            {
                if (factoryUI.unlockButton != null)
                {
                    factoryUI.unlockButton.style.display = DisplayStyle.Flex;
                    factoryUI.unlockButton.SetEnabled(playerGold >= factory.unlockCost);
                }

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

        UpdateResourceLockState();
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
        if (idleManager == null) return null;

        return idleManager.GetFactory(index);
    }
    
    private void CycleCurrency()
    {
         if (idleManager == null || idleManager.GetFactoryCount() == 0) return;

        currentFactoryIndex++;
        if (currentFactoryIndex >= idleManager.GetFactoryCount())
            currentFactoryIndex = 0;

        var factory = GetFactory(currentFactoryIndex);
        if (factory == null) return;

        raptorCore.SetCurrentResource(factory.resource.name);
        resourceBtn.text = factory.name;

        Color color;
        switch (factory.name)
        {
            case "F1": color = Color.white; break;
            case "F2": color = Color.red; break;
            case "F3": color = Color.green; break;
            default: color = Color.white; break;
        }

        currencyIcon.style.unityBackgroundImageTintColor = new StyleColor(color);
        shopIcon.style.unityBackgroundImageTintColor = new StyleColor(color);
        clickerObject.color = color;

        UpdateFactoryUI();
    }

    private void UnlockFactory(int factoryIndex)
    {
        audioSource.Play();

        if (idleManager == null) return;

        bool success = idleManager.UnlockFactory(factoryIndex);

        if (success)
        {
            Debug.Log($"Pomyślnie odblokowano fabrykę {factoryIndex}");
        }
        else
        {
            Debug.Log($"Brak kasy na odblokowanie fabryki {factoryIndex}");
        }

        UpdateFactoryUI();
        SetGoldText(raptorCore.Gold.ToString("F0"));
    }

    private void UpdateResourceLockState()
    {

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
        SetGoldText(raptorCore.Gold.ToString("F0"));
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
        SetGoldText(raptorCore.Gold.ToString("F0"));
        UpdateFactoryUI();
    }

}