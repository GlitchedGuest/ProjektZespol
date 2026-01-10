using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuffManager
{
    private VisualElement buffBar;
    private Dictionary<string, BuffVisual> buffs = new();

    public BuffManager(VisualElement root, Texture2D[] buffs)
    {
        buffBar = root.Q<VisualElement>("BuffBar");
        if (buffBar == null)
            Debug.LogError("BuffBar Error");
        List<string> skills = new List<string>() { "Skill3A", "Skill2B", "Skill4A-tree3" , "Skill2A-tree2" , "Skill3B-tree2" , "Skill3D-tree2", "Skill4-tree2" };
        int j = 0;
        foreach(var i in skills)
        {
            RegisterBuff(i, buffs[j]);
            j++;
        }
    }

    public void RegisterBuff(string skillId, Texture2D icon)
    {
        if (buffs.ContainsKey(skillId)) return;

        var buff = new BuffVisual(skillId, icon);
        buffs.Add(skillId, buff);
        buffBar.Add(buff.root);
    }

    public void ApplyBuff(string skillId, double value)
    {
        if (!buffs.ContainsKey(skillId)) return;
        buffs[skillId].Show(value);
    }

    public void RemoveBuff(string skillId)
    {
        if (!buffs.ContainsKey(skillId)) return;
        buffs[skillId].Hide();
    }

}
