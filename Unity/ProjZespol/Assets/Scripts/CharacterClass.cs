using System;
using System.Collections;
using UnityEngine;

public class CharacterClass:MonoBehaviour
{
    //this is class for the "rpg" character

    private uint level = 1;
    private ulong currentExp = 0;
    private ulong maxExpCap = 100;
    private uint maxLvlCap = 20;
    //there will be other statistics like critical chance later etc
    private float criticalChance = 15.5f;
    public float boostedChance = 0.00f;
    private float skillCheckChance = -1.0f;
    public int skillCheckReduce = 0;
    public double productionIdleBonus = 0;
    //Important Skill Variables
    public bool activeIdle = false;
    public bool noMatterWhat = false;
    public bool chickenDinner = false;
    public bool alwaysWinner = false;
    public bool symbiosis = false;
    public bool mortalClicker = false;
    public bool championOfClicks = false;
    //Use this to gain exp from activities
    public void GainExp(ulong exp)
    {
        if (level < 20)
        {       
            currentExp += exp;
            while(currentExp > maxExpCap && level < maxLvlCap)
            {
                level++;
                currentExp -= maxExpCap;
                NewLevelCap();
            }
        }

    }

    //This can be changed later
    private void NewLevelCap()
    {
        if (level == 20)
        {
            maxExpCap = 0;
            currentExp = 0;
        }
        else
        {
            ulong flair = (ulong)UnityEngine.Random.Range((float)Math.Pow(level, 5), (float)Math.Pow(level, 6));
            maxExpCap += (ulong)Math.Pow(level, 7) + flair;
        }
    }

    //use this to display current exp
    public ulong GetCurrentExp() {  return currentExp; }
    //use this to display how much exp need to level up
    public ulong GetMaxExpCap() {  return maxExpCap; }
    //use this to display current level
    public uint GetLevel() { return level; }
    //If you need to set character to specific level
    public float GetCriticalChance() { return criticalChance + boostedChance; }
    //If you need critical chance
    public float GetSkillCheckChance() { return skillCheckChance; }
    //If you need skillCheck chance

    public void SetLevel(uint _level)
    {
        if (_level > maxLvlCap)
            Debug.Log("Ziomeczku za wysoki level");
        else
        {
            level = _level;
            NewLevelCap() ;
        }
    }

    public void ApplySkillChanges(string id, bool revert)
    {
        switch (id)
        {
            case "Skill1": SkillBasedClicking(revert); break;
            case "Skill2A": CriticalMass(revert); break;
            case "Skill3A": ActiveIdle(revert); break;
            case "Skill4A": NoMatterWhat(revert); break;
            case "Skill2B": ChickenDinner(revert); break;
            case "Skill3B": HungryForMore(revert); break;
            case "Skill4B": AlwaysWinner(revert); break;
            case "Skill5": Symbiosis(revert); break;
            case "Skill6A": MortalClicker(revert); break;
            case "Skill6B": ChampionOfClicks(revert); break;
            case "Skill1-tree3": Entrepreneur(revert); break;
        }

    }

    private void SkillBasedClicking(bool revert)
    {
        skillCheckChance = revert ? -1.0f : 10.0f;
    }
    private void CriticalMass(bool revert)
    {
        criticalChance = revert ? 15.5f : 30.0f;
    }
    private void ActiveIdle(bool revert)
    {
        activeIdle = !revert;
    }
    private void NoMatterWhat(bool revert)
    {
        noMatterWhat = !revert;
    }
    private void ChickenDinner(bool revert)
    {
        chickenDinner = !revert;
    }
    private void HungryForMore(bool revert)
    {
        skillCheckReduce = revert ? 0 : 300;
    }
    private void AlwaysWinner(bool revert)
    {
        alwaysWinner = !revert;
    }
    private void Symbiosis(bool revert)
    {
        skillCheckChance = revert ? 10.0f : 30.0f;
    }
    private void MortalClicker(bool revert)
    {
        mortalClicker = !revert;
    }
    private void ChampionOfClicks(bool revert)
    {
        championOfClicks = !revert;
    }

    public void EnableAlanWake()
    {
        StartCoroutine(AlanWake());
        championOfClicks = false;
    }

    private IEnumerator AlanWake()
    {
        float prevChance = criticalChance;
        criticalChance = 100.0f;
        yield return new WaitForSeconds(6f);
        criticalChance = prevChance;
        championOfClicks = true;
    }

    private void Entrepreneur(bool revert)
    {
        productionIdleBonus = revert ? 0f : 30.0f;
    }
}
