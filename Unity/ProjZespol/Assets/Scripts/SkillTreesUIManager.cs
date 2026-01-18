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

    public void ClearTrees() {
        oneClickArmyTree.ClearTree();
        jackTree.ClearTree();
        automatronTree.ClearTree();
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

        oneClickArmyTree.RegisterTooltip("Skill1", "Skill Based Clicking", "Randomly while clicking skill check will appear. Successful skill check will increase resources. Failed will decrease it.", "\"Work for more work. Right?\" ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill2A", "Critical Mass", "Increases chance for critical click to 30%.", "\"This will be monumental!\" ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill2B", "Chicken Dinner", "Successful skill check will temporally increase resources per click. Failed will decrease.", "\"You're a madman. You know that!\" ~Clair");
        oneClickArmyTree.RegisterTooltip("Skill3A", "Active Idle", "Every critical click will temporally increase idle production.", "\"Time isn't linear. It's a ball of wibbly wobbly stuff.\" ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill3B", "Hungry For More", "Skill checks will appear more often.", "I don't care. ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill4A", "No Matter What", "Every non critical click will increase chance for next critical click.", "\"You promised me all time and space not the end of the world!\" ~Clair");
        oneClickArmyTree.RegisterTooltip("Skill4B", "Always Winner", "Skill checks have no negative effects when failed.", "\"I won't lose.\" ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill5", "Symbiosis", "Increases chance for skill check.", "\"You and me the time lords victorious\" ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill6A", "Mortal Clicker", "Every successful skill check will increase resource per click. Buff is reseted when skill check is failed.", "\"I'm sorry.\" ~Doctor");
        oneClickArmyTree.RegisterTooltip("Skill6B", "Champion Of Clicks", "After 3 successful skill checks chance for critical click will be temporally increased to 100%.", "\"The mass of Temporal Generator is critical i can sto...\" ~Doctor");

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

        jackTree.RegisterTooltip("Skill1-tree2", "Shark", "Increaces selling prices for resources.", "\"I am Night Warrior of magic.\" ~Gunther Terran");
        jackTree.RegisterTooltip("Skill2A-tree2", "Market-place Genius", "Successful skill check increases temporally selling prices for resources.", "\"I hear his voice. It's beautiful.\" ~Daniel");
        jackTree.RegisterTooltip("Skill2B-tree2", "Addict", "Potions are more efficient.", "\"Whole council has rotten to the core.\" ~Gunther Terran");
        jackTree.RegisterTooltip("Skill2C-tree2", "Lucky Bastard", "Failed skill check has a chance to give random potion effect.", "\"We won't be slaves to them.\" ~Daniel");
        jackTree.RegisterTooltip("Skill3A-tree2", "Hard Worker", "Every skill check will increase selling price in shop. Failed skill check will reset it.", "\"Create the barriers, we can't let them pass through.\" ~Gunther Terran");
        jackTree.RegisterTooltip("Skill3B-tree2", "Death Dose", "Potions effect can be stacked", "\"They're weak, break their minds, then theirs bodies.\" ~Daniel");
        jackTree.RegisterTooltip("Skill3C-tree2", "Just Bastard", "Failed skill check guarantee random potion.", "\"Losing this war doesn't matter, losing our existance does.\" ~Gunther Terran");
        jackTree.RegisterTooltip("Skill3D-tree2", "Fail To Win", "Failed skill check gives temporally boost to experience.", "\"GUNHTER SHOW YOURSELF YOU COWARD!\" ~Daniel");
        jackTree.RegisterTooltip("Skill4-tree2", "Failure Grind", "Every failed skill check increases experience boost. Failed skill check reset boost.", "\"There is only one solution.\" ~Gunther Terran");
        jackTree.RegisterTooltip("Skill5-tree2", "Micheal Scott", "Resource will be automatically sold when it reach its limit.", "\"Avekna Ur Trakta\" ~Gunther Terran");

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

        automatronTree.RegisterTooltip("Skill1-tree3", "Entrepreneur", "The more factories you have, the greater bonus to idle is.", "\"Wakey, Wakey! Little Brother.\" ~Herald of Darkness");
        automatronTree.RegisterTooltip("Skill2-tree3", "Push to the limit", "Increases maximum limit of all resources.", "\"I see everything, everywhere, everywhen.\" ~Champion of Light");
        automatronTree.RegisterTooltip("Skill3A-tree3", "Reaction Test", "Skill checks can appear at random instead while clicking.", "\"He's no longer here, I am what's left.\" ~Herald of Darkness");
        automatronTree.RegisterTooltip("Skill3B-tree3", "What Eyes Don't See", "Factories not in focus produce more resources.", "\"Hear me Lux, I'm ready.\" ~Champion of Light");
        automatronTree.RegisterTooltip("Skill4A-tree3", "Unskilled Predator", "Failed skill check boost idle production. Effect can stack.", "\"How's your family? Little Brother.\" ~Herald of Darkness");
        automatronTree.RegisterTooltip("Skill4B-tree3", "Passive Agressive", "The longer player does not click, the more resources factory in focus produce.", "\"In the darkness night there is light.\" ~Champion of Light");
        automatronTree.RegisterTooltip("Skill5A-tree3", "One for Everyone", "Idle production of all factories is greatly increased", "\"Tick tock Jack!\" ~Herald of Darkness");
        automatronTree.RegisterTooltip("Skill5B-tree3", "Multi-tasking", "Player can click but idle bonus is increasing in slower pace.", "\"Let's end it once and for all!\" ~Champion of Light");
        automatronTree.RegisterTooltip("Skill5C-tree3", "Christmas Bonus", "Idle bonus is applied to all factories.", "\"DEVOUR DEVOUR DEVOUR!\" ~Herald of Darkness");
        automatronTree.RegisterTooltip("Skill6-tree3", "Hungry Wolf", "The less resource factory has, more resource it's producing.", "\"Let's do it together, brother.\" ~Champion of Light");

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
