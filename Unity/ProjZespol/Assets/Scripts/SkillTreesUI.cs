using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

#region  SkillTreeManager
public abstract class SkillTreeManager
{
    protected VisualElement ui;
    protected CharacterClass characterClass;

    protected Dictionary<string, Button> buttons = new();
    protected HashSet<string> learned = new();
    protected Dictionary<string, List<string>> dependencies = new();

    protected VisualElement skillTreeContainer;
    protected VisualElement lineLayer;

    protected Color lockedColor = new Color(0.3f, 0.3f, 0.3f);
    protected Color availableColor = new Color(0.8f, 0.6f, 0.3f);
    protected Color learnedColor = new Color(0.2f, 0.8f, 0.2f);

    protected SkillPointsLimit skillPointsLimit;

    public SkillTreeManager(
        VisualElement root,
        CharacterClass charClass,
        Dictionary<string, List<string>> treeDependencies,
        string containerName,
        HashSet<string> learnedSet,
        SkillPointsLimit skillPointsLimit)
    {
        ui = root;
        characterClass = charClass;

        dependencies = treeDependencies;
        learned = learnedSet;

        skillTreeContainer = ui.Q<VisualElement>(containerName);

        lineLayer = new VisualElement();
        InitializeLineLayer(lineLayer);
        skillTreeContainer.Insert(0, lineLayer);
        this.skillPointsLimit = skillPointsLimit;
    }

    private void InitializeLineLayer(VisualElement layer)
    {
        layer.pickingMode = PickingMode.Ignore;
        layer.style.position = Position.Absolute;
        layer.style.left = 0;
        layer.style.top = 0;
        layer.style.right = 0;
        layer.style.bottom = 0;
    }

    public void InitializeSkills(string[] skillIds)
    {
        foreach (string id in skillIds)
        {
            var btn = ui.Q<Button>(id);
            if (btn != null)
            {
                buttons[id] = btn;

                btn.RegisterCallback<PointerUpEvent>(evt =>
                {
                    if (evt.button == 0)
                        OnSkillClicked(id);
                    else if (evt.button == 1)
                        OnSkillUnlearned(id);
                });

                UpdateVisual(id);
            }
        }
    }

    public void RegisterTooltip(string skillId, string title, string description)
    {
        var btn = ui.Q<Button>(skillId);
        if (btn != null)
        {
            Tooltip.Register(btn, title, description);
        }
    }

    public void DrawLine(string fromId, string toId, Color color)
    {
        DrawLineBetweenButtons(ui.Q<Button>(fromId), ui.Q<Button>(toId), color, lineLayer);
    }

    public void DrawAllLines()
    {
        lineLayer.Clear();

        foreach (var kvp in dependencies)
        {
            string child = kvp.Key;
            foreach (string parent in kvp.Value)
            {
                DrawLine(parent, child, Color.white);
            }
        }
    }

    private void DrawLineBetweenButtons(Button from, Button to, Color color, VisualElement targetLayer)
    {
        var line = new VisualElement();
        line.pickingMode = PickingMode.Ignore;

        line.generateVisualContent += ctx =>
        {
            if (from == null || to == null) return;

            var start = from.worldBound.center;
            var end = to.worldBound.center;

            var localStart = targetLayer.WorldToLocal(start);
            var localEnd = targetLayer.WorldToLocal(end);

            ctx.painter2D.strokeColor = color;
            ctx.painter2D.lineWidth = 3f;
            ctx.painter2D.BeginPath();
            ctx.painter2D.MoveTo(localStart);
            ctx.painter2D.LineTo(localEnd);
            ctx.painter2D.Stroke();
        };

        targetLayer.Add(line);
    }

    protected abstract void OnSkillClicked(string id);
    protected abstract bool CanUnlock(string id);
    protected abstract void UpdateVisual(string id);
    protected virtual void OnSkillUnlearned(string id)
    {
        if (!learned.Contains(id)) return;

        foreach (var kvp in dependencies)
        {
            if (kvp.Value.Contains(id) && learned.Contains(kvp.Key))
                return;
        }

        learned.Remove(id);
        skillPointsLimit.IncSkillPoints();
        if (characterClass != null)
            characterClass.ApplySkillChanges(id, true);

        UpdateAll();
    }

    public void UpdateAll()
    {
        foreach (var id in buttons.Keys)
            UpdateVisual(id);
    }

    public void SerializeToStream(BinaryWriter writer)
    {
        writer.Write(learned.Count);
        foreach (var item in learned)
        {
            writer.Write(item);
        }
    }

    public void DeserializeFromStream(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        var learned = new HashSet<string>();
        for (int i = 0; i < count; i++)
        {
            learned.Add(reader.ReadString());
        }
        UpdateAll();
    }
}
#endregion
#region OneClickArmySkillTree
public class OneClickArmySkillTree : SkillTreeManager
{
    public OneClickArmySkillTree(VisualElement root, CharacterClass charClass, HashSet<string> learnedSet,SkillPointsLimit skillPointsLimit): base(root, charClass, GetDependencies(), "SkillTree", learnedSet,skillPointsLimit) { }

    private static Dictionary<string, List<string>> GetDependencies()
    {
        return new Dictionary<string, List<string>>
        {
            { "Skill1", new List<string>() },
            { "Skill2A", new List<string>{ "Skill1" } },
            { "Skill2B", new List<string>{ "Skill1" } },
            { "Skill3A", new List<string>{ "Skill2A" } },
            { "Skill3B", new List<string>{ "Skill2B" } },
            { "Skill4A", new List<string>{ "Skill3A" } },
            { "Skill4B", new List<string>{ "Skill3B" } },
            { "Skill5", new List<string>{ "Skill4A", "Skill4B" } },
            { "Skill6A", new List<string>{ "Skill5" } },
            { "Skill6B", new List<string>{ "Skill5" } },
        };
    }

    protected override void OnSkillClicked(string id)
    {
        if (characterClass.skillpoints < 1)return;
        if (learned.Contains(id)) return;
        if (!CanUnlock(id)) return;

        if (id == "Skill6A" && learned.Contains("Skill6B")) return;
        if (id == "Skill6B" && learned.Contains("Skill6A")) return;
        skillPointsLimit.DecSkillPoints();
        learned.Add(id);
        characterClass.ApplySkillChanges(id, false);
        UpdateVisual(id);
    }

    protected override bool CanUnlock(string id)
    {
        if (!dependencies.ContainsKey(id)) return true;

        if (id == "Skill5")
        {
            foreach (var prereq in dependencies[id])
                if (learned.Contains(prereq)) return true;

            return false;
        }

        foreach (var prereq in dependencies[id])
            if (!learned.Contains(prereq)) return false;

        return true;
    }

    protected override void UpdateVisual(string id)
    {
        var btn = buttons[id];

        if (learned.Contains(id))
        {
            btn.style.backgroundColor = learnedColor;
            return;
        }

        if ((learned.Contains("Skill6A") && id == "Skill6B") || (learned.Contains("Skill6B") && id == "Skill6A"))
        {
            btn.style.backgroundColor = lockedColor;
            return;
        }

        if (CanUnlock(id))
        {
            if (characterClass.skillpoints < 1)
            {
                btn.style.backgroundColor = lockedColor;

            }
            else
            {
                btn.style.backgroundColor = availableColor;
            }
        }
        else
        {
            btn.style.backgroundColor = lockedColor;
        }
    }


}
#endregion
#region JackOfAllClicksSkillTree
public class JackSkillTree : SkillTreeManager
{
    public JackSkillTree(VisualElement root, CharacterClass charClass, HashSet<string> learnedSet, SkillPointsLimit skillPointsLimit) : base(root, charClass, GetDependencies(), "SkillTree2", learnedSet, skillPointsLimit) { }
    private static Dictionary<string, List<string>> GetDependencies()
    {
        return new Dictionary<string, List<string>>
        {
            { "Skill1-tree2", new List<string>() },
            { "Skill2A-tree2", new List<string>{ "Skill1-tree2" } },
            { "Skill2B-tree2", new List<string>{ "Skill1-tree2" } },
            { "Skill2C-tree2", new List<string>{ "Skill1-tree2" } },
            { "Skill3A-tree2", new List<string>{ "Skill2A-tree2" } },
            { "Skill3B-tree2", new List<string>{ "Skill2B-tree2" } },
            { "Skill3C-tree2", new List<string>{ "Skill2C-tree2" } },
            { "Skill3D-tree2", new List<string>{ "Skill2C-tree2" } },
            { "Skill4-tree2", new List<string>{ "Skill3D-tree2" } },
            { "Skill5-tree2", new List<string>{ "Skill4-tree2", "Skill3C-tree2", "Skill3B-tree2", "Skill3A-tree2" } },
        };
    }

    protected override void OnSkillClicked(string id)
    {
        if (characterClass.skillpoints < 1) return;
        if (learned.Contains(id)) return;
        if (!CanUnlock(id)) return;

        skillPointsLimit.DecSkillPoints();
        learned.Add(id);
        
        if (characterClass != null)
            characterClass.ApplySkillChanges(id, false);

        UpdateVisual(id);
    }
    protected override bool CanUnlock(string id)
    {
        if (!dependencies.ContainsKey(id)) return true;

        if (id == "Skill5-tree2")
        {
            foreach (var prereq in dependencies[id])
            {
                if (learned.Contains(prereq))
                    return true;
            }
            return false;
        }
        foreach (var prereq in dependencies[id])
        {
            if (!learned.Contains(prereq))
                return false;
        }
        return true;
    }
    protected override void UpdateVisual(string id)
    {
        if (!buttons.ContainsKey(id)) return;
        var btn = buttons[id];

        if (learned.Contains(id))
        {
            btn.style.backgroundColor = learnedColor;
            return;
        }

        if (CanUnlock(id))
        {
            if (characterClass.skillpoints < 1)
            {
                btn.style.backgroundColor = lockedColor;
            }
            else
            {
                btn.style.backgroundColor = availableColor;
            }
        }
        else
        {
            btn.style.backgroundColor = lockedColor;
        }
    }
}
#endregion
#region AutomatronSkillTree
public class AutomatronSkillTree : SkillTreeManager
{
    public AutomatronSkillTree(VisualElement root, CharacterClass charClass, HashSet<string> learnedSet, SkillPointsLimit skillPointsLimit) : base(root, charClass, GetDependencies(), "SkillTree3", learnedSet, skillPointsLimit) { }
    private static Dictionary<string, List<string>> GetDependencies()
    {
        return new Dictionary<string, List<string>>
        {
            { "Skill1-tree3", new List<string>() },
            { "Skill2-tree3", new List<string>{ "Skill1-tree3" } },
            { "Skill3A-tree3", new List<string>{ "Skill2-tree3" } },
            { "Skill3B-tree3", new List<string>{ "Skill2-tree3" } },
            { "Skill4A-tree3", new List<string>{ "Skill3A-tree3" } },
            { "Skill4B-tree3", new List<string>{ "Skill3B-tree3" } },
            { "Skill5A-tree3", new List<string>{ "Skill4A-tree3" } },
            { "Skill5B-tree3", new List<string>{ "Skill4B-tree3" } },
            { "Skill5C-tree3", new List<string>{ "Skill4B-tree3" } },
            { "Skill6-tree3", new List<string>{ "Skill5C-tree3" } },
        };
    }

    protected override void OnSkillClicked(string id)
    {
        if (characterClass.skillpoints < 1) return;
        if (learned.Contains(id)) return;
        if (!CanUnlock(id)) return;

        skillPointsLimit.DecSkillPoints();
        learned.Add(id);

        if (characterClass != null)
            characterClass.ApplySkillChanges(id, false);

        UpdateVisual(id);
    }
    protected override bool CanUnlock(string id)
    {
        if (!dependencies.ContainsKey(id)) return true;

        foreach (var prereq in dependencies[id])
        {
            if (!learned.Contains(prereq))
                return false;
        }
        return true;
    }
    protected override void UpdateVisual(string id)
    {
        if (!buttons.ContainsKey(id)) return;
        var btn = buttons[id];

        if (learned.Contains(id))
        {
            btn.style.backgroundColor = learnedColor;
            return;
        }

        if (CanUnlock(id))
        {
            if (characterClass.skillpoints < 1)
            {
                btn.style.backgroundColor = lockedColor;
            }
            else
            {
                btn.style.backgroundColor = availableColor;
            }
        }
        else
        {
            btn.style.backgroundColor = lockedColor;
        }
    }
}
#endregion