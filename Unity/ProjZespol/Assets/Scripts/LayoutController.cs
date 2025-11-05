using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

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
    }

    public ScrollView scrollView;
    private List<FactoryUI> factoryUIs = new List<FactoryUI>();
    private int numberOfFactories = 9;

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

            int factoryIndex = i - 1;
            int factoryIndexCopy = factoryIndex;

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
        SetGoldText(raptorCore.Gold.ToString());
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
        SetGoldText(raptorCore.Gold.ToString());
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
        SetGoldText(raptorCore.Gold.ToString());
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
        SetGoldText(raptorCore.Gold.ToString());
    }

    private void UpdateFactoryUI()
    {
        if (idleManager == null) return;

        for (int i = 0; i < factoryUIs.Count; i++)
        {
            var factoryUI = factoryUIs[i];
            var factory = GetFactory(i);
            if (factory == null) continue;


            if (factory == null || factoryUI.countLabel == null || factoryUI.costLabel == null)
                continue;

            factoryUI.countLabel.text = $"Ilość: {factory.count}";
            factoryUI.costLabel.text = $"Koszt: {factory.currentCost}";
            factoryUI.costLabel.text = factory.isUnlocked ? $"Koszt: {factory.currentCost}" : $"Odblokuj za: {factory.unlockCost}";

            double playerGold = raptorCore.Gold;

            if (!factory.isUnlocked)
            {
                if (factoryUI.unlockButton != null)
                {
                    factoryUI.unlockButton.style.display = DisplayStyle.Flex;
                    factoryUI.unlockButton.SetEnabled(playerGold >= factory.unlockCost);
                }

                foreach (var btn in factoryUI.buyButtons)
                {
                    if (btn != null) btn.style.display = DisplayStyle.None;
                }
                foreach (var btn in factoryUI.sellButtons)
                {
                    if (btn != null) btn.style.display = DisplayStyle.None;
                }
            }
            else
            {
                if (factoryUI.unlockButton != null)
                {
                    factoryUI.unlockButton.style.display = DisplayStyle.None;
                }

                foreach (var btn in factoryUI.buyButtons)
                {
                    if (btn != null) btn.style.display = DisplayStyle.Flex;
                }

                foreach (var btn in factoryUI.sellButtons)
                {
                    if (btn != null) btn.style.display = DisplayStyle.Flex;
                }

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
        if (raptorCore == null || clickerObject == null) return;

        bool hasResource1Factory = true;
        bool hasResource2Factory = false;
        bool hasResource3Factory = false;

        for (int i = 0; i < factoryUIs.Count; i++)
        {
            var factory = GetFactory(i);
            if (factory == null) continue;

            string resName = factory.resource?.name;
            if (string.IsNullOrEmpty(resName)) continue;

            else if (resName == "Resource2" && factory.count > 0)
            {
                hasResource2Factory = true;
            }
            else if (resName == "Resource3" && factory.count > 0)
            {
                hasResource3Factory = true;
            }
        }

        string current = raptorCore.GetCurrentResourceName();
        string nextResource = current;

        if (current == "Resource1")
        {
            if (hasResource2Factory) nextResource = "Resource2";
            else if (hasResource3Factory) nextResource = "Resource3";
        }
        else if (current == "Resource2")
        {
            if (hasResource3Factory) nextResource = "Resource3";
            else if (hasResource1Factory) nextResource = "Resource1";
        }
        else if (current == "Resource3")
        {
            nextResource = "Resource1";
        }
        else
        {
            nextResource = "Resource1";
        }

        if (nextResource == current) return;

        raptorCore.SetCurrentResource(nextResource);
        resourceBtn.text = nextResource;

        //Color change is temporary until it's decided on which icons will be used
        Color color;
        switch (nextResource)
        {
            case "Resource1": color = Color.white; break;
            case "Resource2": color = Color.red; break;
            case "Resource3": color = Color.green; break;
            default: color = Color.white; break;
        }

        currencyIcon.style.unityBackgroundImageTintColor = new StyleColor(color);
        shopIcon.style.unityBackgroundImageTintColor = new StyleColor(color);
        clickerObject.color = color;

        UpdateResourceLockState();
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
        SetGoldText(raptorCore.Gold.ToString());
    }

    private void UpdateResourceLockState()
    {
        if (idleManager == null || raptorCore == null) return;

        bool hasResource2Factory = false;
        bool hasResource3Factory = false;

        for (int i = 0; i < factoryUIs.Count; i++)
        {
            var factory = GetFactory(i);
            if (factory == null) continue;

            string resName = factory.resource?.name;
            if (string.IsNullOrEmpty(resName)) continue;


            if (resName == "Resource2" && factory.count > 0)
            {
                hasResource2Factory = true;
            }
            else if (resName == "Resource3" && factory.count > 0)
            {
                hasResource3Factory = true;
            }

        }

        bool canUseResource1 = true;
        bool canUseResource2 = hasResource2Factory;
        bool canUseResource3 = hasResource3Factory;
        string currentResource = raptorCore.GetCurrentResourceName();

        bool isLocked = false;
        switch (currentResource)
        {
            case "Resource1":
                isLocked = !canUseResource1;
                break;
            case "Resource2":
                isLocked = !canUseResource2;
                break;
            case "Resource3":
                isLocked = !canUseResource3;
                break;
        }

        if (resourceBtn != null)
        {
            resourceBtn.SetEnabled(!isLocked);

            if (isLocked)
            {
                resourceBtn.text = $"{currentResource} (Zablokowany)";
                resourceBtn.style.opacity = 0.5f;
            }
            else
            {
                resourceBtn.text = currentResource;
                resourceBtn.style.opacity = 1f;
            }
        }
    }


}