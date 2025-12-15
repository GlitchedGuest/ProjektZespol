using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PotionUIManager
{
    private class PotionUI
    {
        public VisualElement potionPanel;
        public Label nameLabel;
        public Label costLabel;
        public Label effectLabel;
        public Button buyButton;
    }

    private VisualElement ui;
    private ScrollView scrollView;
    private List<PotionUI> potionUIs = new List<PotionUI>();
    private int numberOfPotions = 6;

    private RaptorCore raptorCore;
    private IdleManager idleManager;
    private AudioSource audioSource;
    private CharacterClass characterClass;

    public PotionUIManager(VisualElement root, RaptorCore core, IdleManager manager, AudioSource audio, CharacterClass character)
    {
        ui = root;
        raptorCore = core;
        idleManager = manager;
        audioSource = audio;
        characterClass = character;
    }

    public void Initialize()
    {
        scrollView = ui.Q<ScrollView>("ScrollView2");
        scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
        scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
    
        InitializePotions();
    }

    private void InitializePotions()
    {
        for (int i = 1; i <= numberOfPotions; i++)
        {
            PotionUI potionUI = new PotionUI();

            potionUI.buyButton = ui.Q<Button>($"Potion{i}Buy");
            potionUI.nameLabel = ui.Q<Label>($"Potion{i}Name");
            potionUI.effectLabel = ui.Q<Label>($"Potion{i}Effect");
            potionUI.costLabel = ui.Q<Label>($"Potion{i}Cost");

            if (potionUI.nameLabel != null)
            {
                potionUI.nameLabel.style.whiteSpace = WhiteSpace.Normal;
                potionUI.nameLabel.style.flexWrap = Wrap.Wrap;
            }

            if (potionUI.effectLabel != null)
            {
                potionUI.effectLabel.style.whiteSpace = WhiteSpace.Normal;
                potionUI.effectLabel.style.flexWrap = Wrap.Wrap;
            }

            if (potionUI.costLabel != null)
            {
                potionUI.costLabel.style.whiteSpace = WhiteSpace.Normal;
                potionUI.costLabel.style.flexWrap = Wrap.Wrap;
            }

            int potionIndex = i - 1;
            int potionIndexCopy = potionIndex;

            if (potionUI.buyButton != null)
            {
                potionUI.buyButton.clicked += () => BuyPotion(potionIndexCopy);
            }

            potionUIs.Add(potionUI);
        }
    }

    public void Update()
    {
        UpdatePotionUI();
    }

    private void UpdatePotionUI()
    {
        if (idleManager == null || raptorCore == null) return;

        for (int i = 0; i < potionUIs.Count; i++)
        {
            var potionUI = potionUIs[i];
            var potion = idleManager.GetPotion(i);
            if (potion == null) continue;

            if (potionUI.nameLabel != null)
                potionUI.nameLabel.text = potion.name;
            if (potionUI.effectLabel != null)
                potionUI.effectLabel.text = potion.effectDescription;
            if (potionUI.costLabel != null)
                potionUI.costLabel.text = $"Koszt: {potion.cost1} szt. zasobu {potion.resourceType1.name}\n\t   {potion.cost2} szt. zasobu {potion.resourceType2.name}";

            if (potionUI.buyButton != null)
            {
                QuarkType playerResourceAmount1 = raptorCore.GetResourceValueDirect(potion.resourceType1.name);
                QuarkType playerResourceAmount2 = raptorCore.GetResourceValueDirect(potion.resourceType2.name);
                bool canAfford = (playerResourceAmount1 >= potion.cost1) && (playerResourceAmount2 >= potion.cost2);
                bool isNotActive = !potion.isActive;
                potionUI.buyButton.SetEnabled((canAfford && isNotActive) || characterClass.deathDose);
                if (potion.isActive && !characterClass.deathDose)
                {
                    potionUI.buyButton.text = "Aktywna";
                }
                else
                {
                    potionUI.buyButton.text = "Kup";
                }
            }
        }
    }

    private void BuyPotion(int potionIndex)
    {
        audioSource.Play();

        if (idleManager == null)
        {
            Debug.LogError("IdleManager nie jest przypisany!");
            return;
        }

        bool success = idleManager.BuyPotion(potionIndex);
        if (!success)
        {
            Debug.Log($"Brak środków na zakup mikstury {potionIndex}");
        }
        else
        {
            Debug.Log($"Zakupiono miksturę {potionIndex}");
        }

        UpdatePotionUI();
    }
}