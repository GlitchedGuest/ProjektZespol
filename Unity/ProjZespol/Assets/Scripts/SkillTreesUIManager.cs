using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillTreesUIManager {
    private VisualElement ui;
    private CharacterClass characterClass;
    public VisualTreeAsset tooltipAsset;

    private OneClickArmySkillTree oneClickArmyTree;
    private JackSkillTree jackTree;
    private AutomatronSkillTree automatronTree;

    private HashSet<string> learnedOneClick;
    private HashSet<string> learnedJack;
    private HashSet<string> learnedAutomatron;

    private SkillPointsLimit skillPointsLimit;

    public SkillTreesUIManager(VisualElement ui, CharacterClass characterClass, HashSet<string> learnedOneClick, HashSet<string> learnedJack, HashSet<string> learnedAutomatron, SkillPointsLimit skillPointsLimit)
    {
        this.ui = ui;
        this.characterClass = characterClass;

        this.learnedOneClick = learnedOneClick;
        this.learnedJack = learnedJack;
        this.learnedAutomatron = learnedAutomatron;
        this.skillPointsLimit = skillPointsLimit;
    }

    public void InitializeAllTrees()
    {
        InitializeOneClickArmyTree();
        InitializeJackTree();
        InitializeAutomatronTree();
    }

    public void Update()
    {
        oneClickArmyTree?.UpdateAll();
        jackTree?.UpdateAll();
        automatronTree?.UpdateAll();
    }

    private void InitializeOneClickArmyTree()
    {
        oneClickArmyTree = new OneClickArmySkillTree(ui, characterClass, learnedOneClick, skillPointsLimit);

        string[] skills =
        {
            "Skill1", "Skill2A", "Skill2B", "Skill3A", "Skill3B",
            "Skill4A", "Skill4B", "Skill5", "Skill6A", "Skill6B"
        };

        oneClickArmyTree.InitializeSkills(skills);

        oneClickArmyTree.RegisterTooltip("Skill1", "Skill Based Clicking", "Umożliwia pojawienie się skill checków(nie udany kosztuje gracza zwolnieniem idle produkcji)");
        oneClickArmyTree.RegisterTooltip("Skill2A", "Critical Mass", "Zwiększa szanse na kliki krytyczne");
        oneClickArmyTree.RegisterTooltip("Skill2B", "Chicken Dinner", "Udany skill check zwiększa ilość zbieranych punktów na chwilę(nie udany zmniejsza)");
        oneClickArmyTree.RegisterTooltip("Skill3A", "Active Idle", "Każdy klik krytyczny chwilowo zwiększa idle produkcje");
        oneClickArmyTree.RegisterTooltip("Skill3B", "Hungry For More", "Skill checki częściej się pojawiają");
        oneClickArmyTree.RegisterTooltip("Skill4A", "No Matter What", "Stakuje szanse na klik krytyczny(każde kliknięcie niekrytyczne zwiększa szanse na krytyczne)");
        oneClickArmyTree.RegisterTooltip("Skill4B", "Always Winner", "Skill checki nie mają negatywnych skutków po przegraniu");
        oneClickArmyTree.RegisterTooltip("Skill5", "Symbiosis", "Częstotliwość skill checka jest zależna od ilości klików krytycznych(im częściej są tym częściej skill checki)");
        oneClickArmyTree.RegisterTooltip("Skill6A", "Mortal Clicker", "Każdy kolejny skill check łączy się w kombos...");
        oneClickArmyTree.RegisterTooltip("Skill6B", "Champion Of Clicks", "Po trzech udanych skill checkach z rzędu...");

        Color c = Color.yellow;
        oneClickArmyTree.DrawLine("Skill1", "Skill2A", c);
        oneClickArmyTree.DrawLine("Skill1", "Skill2B", c);
        oneClickArmyTree.DrawLine("Skill2A", "Skill3A", c);
        oneClickArmyTree.DrawLine("Skill2B", "Skill3B", c);
        oneClickArmyTree.DrawLine("Skill3A", "Skill4A", c);
        oneClickArmyTree.DrawLine("Skill3B", "Skill4B", c);
        oneClickArmyTree.DrawLine("Skill4A", "Skill5", c);
        oneClickArmyTree.DrawLine("Skill4B", "Skill5", c);
        oneClickArmyTree.DrawLine("Skill5", "Skill6A", c);
        oneClickArmyTree.DrawLine("Skill5", "Skill6B", c);
    }

    private void InitializeJackTree()
    {
        jackTree = new JackSkillTree(ui, characterClass, learnedJack, skillPointsLimit);

        string[] skills =
        {
            "Skill1-tree2", "Skill2A-tree2", "Skill2B-tree2", "Skill2C-tree2",
            "Skill3A-tree2", "Skill3B-tree2", "Skill3C-tree2", "Skill3D-tree2",
            "Skill4-tree2", "Skill5-tree2"
        };

        jackTree.InitializeSkills(skills);

        jackTree.RegisterTooltip("Skill1-tree2", "Shark", "Można sprzedawać po wyższych cenach…");
        jackTree.RegisterTooltip("Skill2A-tree2", "Market-place Genius", "Udany skill check…");
        jackTree.RegisterTooltip("Skill2B-tree2", "Addict", "Potki mają zwiększoną skuteczność");
        jackTree.RegisterTooltip("Skill2C-tree2", "Lucky Bastard", "Nieudany skill check daje potkę");
        jackTree.RegisterTooltip("Skill3A-tree2", "Hard Worker", "Im więcej udanych…");
        jackTree.RegisterTooltip("Skill3B-tree2", "Death Dose", "Efekty potek się stakują");
        jackTree.RegisterTooltip("Skill3C-tree2", "Just Bastard", "Nieudany skill check = potka");
        jackTree.RegisterTooltip("Skill3D-tree2", "Fail To Win", "Nieudany skill check zwiększa exp");
        jackTree.RegisterTooltip("Skill4-tree2", "Failure Grind", "Im więcej nieudanych…");
        jackTree.RegisterTooltip("Skill5-tree2", "Micheal Scott", "Automatyczna sprzedaż…");

        Color c = Color.yellow;
        jackTree.DrawLine("Skill1-tree2", "Skill2A-tree2", c);
        jackTree.DrawLine("Skill1-tree2", "Skill2B-tree2", c);
        jackTree.DrawLine("Skill1-tree2", "Skill2C-tree2", c);
        jackTree.DrawLine("Skill2A-tree2", "Skill3A-tree2", c);
        jackTree.DrawLine("Skill2B-tree2", "Skill3B-tree2", c);
        jackTree.DrawLine("Skill2C-tree2", "Skill3C-tree2", c);
        jackTree.DrawLine("Skill2C-tree2", "Skill3D-tree2", c);
        jackTree.DrawLine("Skill3D-tree2", "Skill4-tree2", c);
        jackTree.DrawLine("Skill4-tree2", "Skill5-tree2", c);
        jackTree.DrawLine("Skill3C-tree2", "Skill5-tree2", c);
        jackTree.DrawLine("Skill3B-tree2", "Skill5-tree2", c);
        jackTree.DrawLine("Skill3A-tree2", "Skill5-tree2", c);
    }
    private void InitializeAutomatronTree()
    {
        automatronTree = new AutomatronSkillTree(ui, characterClass, learnedAutomatron, skillPointsLimit);

        string[] skills =
        {
            "Skill1-tree3", "Skill2-tree3", "Skill3A-tree3", "Skill3B-tree3",
            "Skill4A-tree3", "Skill4B-tree3", "Skill5A-tree3", "Skill5B-tree3",
            "Skill5C-tree3", "Skill6-tree3"
        };

        automatronTree.InitializeSkills(skills);

        automatronTree.RegisterTooltip("Skill1-tree3", "Entre-preneur", "Im więcej fabryk…");
        automatronTree.RegisterTooltip("Skill2-tree3", "Push to the limit", "Limit punktów rośnie");
        automatronTree.RegisterTooltip("Skill3A-tree3", "Reaction Test", "Skill checki mogą…");
        automatronTree.RegisterTooltip("Skill3B-tree3", "What Eyes Don't See", "Fabryki not in focus…");
        automatronTree.RegisterTooltip("Skill4A-tree3", "Unskilled Predator", "Nieudany check zwiększa idle…");
        automatronTree.RegisterTooltip("Skill4B-tree3", "Passive Agressive", "Im dłużej gracz nie kliknie…");
        automatronTree.RegisterTooltip("Skill5A-tree3", "One for Everyone", "Nieudany check buffuje wszystko");
        automatronTree.RegisterTooltip("Skill5B-tree3", "Multi-tasking", "Gracz może robić inne akcje");
        automatronTree.RegisterTooltip("Skill5C-tree3", "Christmas Bonus", "Bonus do fabryk not in focus");
        automatronTree.RegisterTooltip("Skill6-tree3", "Hungry Wolf", "Im mniej punktów tym większy bonus");

        Color c = Color.yellow;
        automatronTree.DrawLine("Skill1-tree3", "Skill2-tree3", c);
        automatronTree.DrawLine("Skill2-tree3", "Skill3A-tree3", c);
        automatronTree.DrawLine("Skill2-tree3", "Skill3B-tree3", c);
        automatronTree.DrawLine("Skill3A-tree3", "Skill4A-tree3", c);
        automatronTree.DrawLine("Skill3B-tree3", "Skill4B-tree3", c);
        automatronTree.DrawLine("Skill4A-tree3", "Skill5A-tree3", c);
        automatronTree.DrawLine("Skill4B-tree3", "Skill5B-tree3", c);
        automatronTree.DrawLine("Skill4B-tree3", "Skill5C-tree3", c);
        automatronTree.DrawLine("Skill5C-tree3", "Skill6-tree3", c);
    }

    public void SerializeToStream(BinaryWriter writer)
    {
        oneClickArmyTree.SerializeToStream(writer);
        jackTree.SerializeToStream(writer);
        automatronTree.SerializeToStream(writer);
    }

    public void DeserializeFromStream(BinaryReader reader)
    {
        oneClickArmyTree.DeserializeFromStream(reader);
        jackTree.DeserializeFromStream(reader);
        automatronTree.DeserializeFromStream(reader);
    }

}
