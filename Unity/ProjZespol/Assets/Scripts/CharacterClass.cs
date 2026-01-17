using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterClass : MonoBehaviour, IBinarySaveable
{
    public int SaveKey => 003; //Saveid to keep order

    [HideInInspector]
    public BuffManager buffManager;
    public RaptorCore Raptorcore;
    [SerializeField] private SkillPointsLimit skillPointsLimit;
    //this is class for the "rpg" character
    public uint skillpoints = 0;
    private uint level = 1;
    private ulong currentExp = 0;
    private ulong maxExpCap = 100;
    private uint maxLvlCap = 20;
    //there will be other statistics like critical chance later etc
    private float criticalChance;
    public float boostedChance;
    private float skillCheckChance;
    public int skillCheckReduce;
    public double productionIdleBonus;
    public double productionIdlePedatorBonus;
    public double productionIdleAgressiveBonus;
    public double sellingBonus;
    public double sellingHardBonus;
    public double potionBoost;
    public double potionRandomChance;
    public ulong expBoost;
    public ulong komboBoost;
    //Important Skill Variables
    public bool activeIdle = false;
    public bool noMatterWhat = false;
    public bool chickenDinner = false;
    public bool alwaysWinner = false;
    public bool symbiosis = false;
    public bool mortalClicker = false;
    public bool championOfClicks = false;
    public bool reactionTest = false;
    public bool unskilledPredator = false;
    public bool oneForEveryone = false;
    public bool whatEyesDontSee = false;
    public bool passiveAgressive = false;
    public bool multitasking = false;
    public bool christmasBonus = false;
    public bool hungryWolf = false;
    public bool marketplaceGenius = false;
    public bool hardWorker = false;
    public bool deathDose = false;
    public bool failToWin = false;
    public bool failureGrind = false;
    public bool michealScott = false;

    public int T1 = 0;
    public int T2 = 0;
    public int T3 = 0;

    private void Start()
    {
        criticalChance = 15.5f;
        boostedChance = 0.00f;
        skillCheckChance = -1.0f;
        skillCheckReduce = 0;
        productionIdleBonus = 0;
        productionIdlePedatorBonus = 0;
        productionIdleAgressiveBonus = 0;
        sellingBonus = 1;
        sellingHardBonus = 1;
        potionBoost = 1;
        potionRandomChance = -1.0f;
        expBoost = 1;
        komboBoost = 0;
    }

    //Use this to gain exp from activities
    public void GainExp(ulong exp)
    {
        if (level < 20)
        {
            currentExp += exp * expBoost;
            while (currentExp > maxExpCap && level < maxLvlCap)
            {

                level++;
                skillpoints++;
                skillPointsLimit.UpdateButton();

                //MullValueFix  please use QUARKTYPE if we want no problems in conversion if you must then use ceil function
                Raptorcore.ResourceManager.MullLimitResourceAll(new QuarkType(120000000, 0));

                currentExp -= maxExpCap;
                NewLevelCap();

            }
        }
        if (level >= 20)
        {
            Raptorcore.layoutController.prestigeUIManager.EnablePrestige();
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
    public ulong GetCurrentExp() { return currentExp; }
    //use this to display how much exp need to level up
    public ulong GetMaxExpCap() { return maxExpCap; }
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
            NewLevelCap();
        }
    }

    private void UpdateTreeCounter(string id, bool revert)
    {
        int delta = revert ? -1 : 1;

        if (id.Contains("-tree3")) T3 += delta;
        else if (id.Contains("-tree2")) T2 += delta;
        else T1 += delta;
    }

    public void ApplySkillChanges(string id, bool revert)
    {
        UpdateTreeCounter(id, revert);


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
            case "Skill2-tree3": PushToTheLimit(revert); break;
            case "Skill3A-tree3": ReactionTest(revert); break;
            case "Skill4A-tree3": UnskilledPredator(revert); break;
            case "Skill5A-tree3": OneForEveryone(revert); break;
            case "Skill3B-tree3": WhatEyesDontSee(revert); break;
            case "Skill4B-tree3": PassiveAgressive(revert); break;
            case "Skill5B-tree3": Multitasking(revert); break;
            case "Skill5C-tree3": ChristmasBonus(revert); break;
            case "Skill6-tree3": HungryWolf(revert); break;
            case "Skill1-tree2": Shark(revert); break;
            case "Skill2A-tree2": MarketplaceGenius(revert); break;
            case "Skill3A-tree2": HardWorker(revert); break;
            case "Skill2B-tree2": Addict(revert); break;
            case "Skill3B-tree2": DeathDose(revert); break;
            case "Skill2C-tree2": LuckyBastard(revert); break;
            case "Skill3C-tree2": JustBastard(revert); break;
            case "Skill3D-tree2": FailToWin(revert); break;
            case "Skill4-tree2": FailureGrind(revert); break;
            case "Skill5-tree2": MichealScott(revert); break;
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

        if (buffManager == null) return;

        if (revert)
            buffManager.RemoveBuff("Skill3A");
        else
            if(Raptorcore.SkillManager.GetActiveIdleValue()>0) buffManager.ApplyBuff("Skill3A", Raptorcore.SkillManager.GetActiveIdleValue());
    }

    private void NoMatterWhat(bool revert)
    {
        noMatterWhat = !revert;
    }
    private void ChickenDinner(bool revert)
    {
        chickenDinner = !revert;

        if (buffManager == null) return;

        if (revert)
            buffManager.RemoveBuff("Skill2B");
        else
            if(Raptorcore.SkillMultiplier>0) buffManager.ApplyBuff("Skill2B", Raptorcore.SkillMultiplier);
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
        productionIdleBonus = revert ? 0f : 5.0f;
    }
    private void PushToTheLimit(bool revert)
    {
        if (!revert) Raptorcore.ResourceManager.IncrementSkillBoostResourceAll(true);
        else Raptorcore.ResourceManager.IncrementSkillBoostResourceAll(false);
    }
    private void ReactionTest(bool revert)
    {
        reactionTest = !revert;
    }
    private void UnskilledPredator(bool revert)
    {
        unskilledPredator = !revert;
        if (!revert) productionIdlePedatorBonus = 0f;
        if (buffManager == null) return;

        if (revert)
            buffManager.RemoveBuff("Skill4A-tree3");
        else
           if(productionIdlePedatorBonus > 0) buffManager.ApplyBuff("Skill4A-tree3", productionIdlePedatorBonus);
    }
    public void Pedator(bool reset)
    {
        if (unskilledPredator)
        {
            if (!reset) productionIdlePedatorBonus += 5.0f;
            else productionIdlePedatorBonus = 0f;
        }
    }
    private void OneForEveryone(bool revert)
    {
        oneForEveryone = !revert;
        if (!revert) productionIdleBonus = 5.0f;
    }
    private void WhatEyesDontSee(bool revert)
    {
        whatEyesDontSee = !revert;
    }
    private void PassiveAgressive(bool revert)
    {
        passiveAgressive = !revert;
        productionIdleAgressiveBonus = 0;
    }
    private void Multitasking(bool revert)
    {
        multitasking = !revert;
    }
    private void ChristmasBonus(bool revert)
    {
        christmasBonus = !revert;
    }
    private void HungryWolf(bool revert)
    {
        hungryWolf = !revert;
    }
    private void Shark(bool revert)
    {
        sellingBonus = revert ? 1f : 1.5f;
    }
    private void MarketplaceGenius(bool revert)
    {
        marketplaceGenius = !revert;

        if (buffManager == null) return;

        if (revert)
            buffManager.RemoveBuff("Skill2A-tree2");
        else
            if(sellingBonus > 0) buffManager.ApplyBuff("Skill2A-tree2", 0);
    }
    public void EnableGenius(bool reset)
    {
        if (reset)
            sellingHardBonus = 1;
        else if (!reset && hardWorker)
            sellingHardBonus += 1;
        if (marketplaceGenius && !reset)
            StartCoroutine(Genius());
    }
    private IEnumerator Genius()
    {
        sellingBonus += 1.0f;
        yield return new WaitForSeconds(6f);
        sellingBonus -= 1.0f;
    }
    private void HardWorker(bool revert)
    {
        hardWorker = !revert;
    }
    private void Addict(bool revert)
    {
        potionBoost = revert ? 1 : 2;
    }
    private void DeathDose(bool revert)
    {
        deathDose = !revert;
    }
    private void LuckyBastard(bool revert)
    {
        potionRandomChance = revert ? -1.0f : 20.0f;
    }
    private void JustBastard(bool revert)
    {
        potionRandomChance = revert ? 20.0f : 101.0f;
    }
    private void FailToWin(bool revert)
    {
        failToWin = !revert;

        if (buffManager == null) return;

        if (revert)
            buffManager.RemoveBuff("Skill3D-tree2");
        else
            if(expBoost > 1) buffManager.ApplyBuff("Skill3D-tree2", expBoost);
    }
    public void EnableExpBoost()
    {
        if (failToWin)
        {
            if (failureGrind)
                komboBoost += 2;
            StartCoroutine(Failure());
        }
    }
    private IEnumerator Failure()
    {
        expBoost = 2 + komboBoost;
        yield return new WaitForSeconds(6f);
        expBoost = 1;
    }
    private void FailureGrind(bool revert)
    {
        failureGrind = !revert;
        if (revert)
            komboBoost = 0;
        if (buffManager == null) return;

        if (revert)
            buffManager.RemoveBuff("Skill4-tree2");
        else
            if(komboBoost > 0) buffManager.ApplyBuff("Skill4-tree2", komboBoost);
    }
    private void MichealScott(bool revert)
    {
        michealScott = !revert;
    }

    public void SerializeToStream(BinaryWriter writer)
    {
        // Numeric stats
        writer.Write(skillpoints);
        writer.Write(level);
        writer.Write(currentExp);
        writer.Write(maxExpCap);
        writer.Write(maxLvlCap);
        writer.Write(criticalChance);
        writer.Write(boostedChance);
        writer.Write(skillCheckChance);
        writer.Write(skillCheckReduce);
        writer.Write(productionIdleBonus);
        writer.Write(productionIdlePedatorBonus);
        writer.Write(productionIdleAgressiveBonus);
        writer.Write(sellingBonus);
        writer.Write(sellingHardBonus);
        writer.Write(potionBoost);
        writer.Write(potionRandomChance);
        writer.Write(expBoost);
        writer.Write(komboBoost);

        // Boolean flags (packed)
        uint skillFlags = 0;
        skillFlags |= (activeIdle ? 1u << 0 : 0);
        skillFlags |= (noMatterWhat ? 1u << 1 : 0);
        skillFlags |= (chickenDinner ? 1u << 2 : 0);
        skillFlags |= (alwaysWinner ? 1u << 3 : 0);
        skillFlags |= (symbiosis ? 1u << 4 : 0);
        skillFlags |= (mortalClicker ? 1u << 5 : 0);
        skillFlags |= (championOfClicks ? 1u << 6 : 0);
        skillFlags |= (reactionTest ? 1u << 7 : 0);
        skillFlags |= (unskilledPredator ? 1u << 8 : 0);
        skillFlags |= (oneForEveryone ? 1u << 9 : 0);
        skillFlags |= (whatEyesDontSee ? 1u << 10 : 0);
        skillFlags |= (passiveAgressive ? 1u << 11 : 0);
        skillFlags |= (multitasking ? 1u << 12 : 0);
        skillFlags |= (christmasBonus ? 1u << 13 : 0);
        skillFlags |= (hungryWolf ? 1u << 14 : 0);
        skillFlags |= (marketplaceGenius ? 1u << 15 : 0);
        skillFlags |= (hardWorker ? 1u << 16 : 0);
        skillFlags |= (deathDose ? 1u << 17 : 0);
        skillFlags |= (failToWin ? 1u << 18 : 0);
        skillFlags |= (failureGrind ? 1u << 19 : 0);
        skillFlags |= (michealScott ? 1u << 20 : 0);

        writer.Write(skillFlags);
    } 


public void DeserializeFromStream(BinaryReader reader)
    {
        skillpoints = reader.ReadUInt32();
        level = reader.ReadUInt32();
        currentExp = reader.ReadUInt64();
        maxExpCap = reader.ReadUInt64();
        maxLvlCap = reader.ReadUInt32();
        criticalChance = reader.ReadSingle();
        boostedChance = reader.ReadSingle();
        skillCheckChance = reader.ReadSingle();
        skillCheckReduce = reader.ReadInt32();
        productionIdleBonus = reader.ReadDouble();
        productionIdlePedatorBonus = reader.ReadDouble();
        productionIdleAgressiveBonus = reader.ReadDouble();
        sellingBonus = reader.ReadDouble();
        sellingHardBonus = reader.ReadDouble();
        potionBoost = reader.ReadDouble();
        potionRandomChance = reader.ReadDouble();
        expBoost = reader.ReadUInt64();
        komboBoost = reader.ReadUInt64();

        // Boolean flags
        uint skillFlags = reader.ReadUInt32();
        activeIdle = (skillFlags & (1u << 0)) != 0;
        noMatterWhat = (skillFlags & (1u << 1)) != 0;
        chickenDinner = (skillFlags & (1u << 2)) != 0;
        alwaysWinner = (skillFlags & (1u << 3)) != 0;
        symbiosis = (skillFlags & (1u << 4)) != 0;
        mortalClicker = (skillFlags & (1u << 5)) != 0;
        championOfClicks = (skillFlags & (1u << 6)) != 0;
        reactionTest = (skillFlags & (1u << 7)) != 0;
        unskilledPredator = (skillFlags & (1u << 8)) != 0;
        oneForEveryone = (skillFlags & (1u << 9)) != 0;
        whatEyesDontSee = (skillFlags & (1u << 10)) != 0;
        passiveAgressive = (skillFlags & (1u << 11)) != 0;
        multitasking = (skillFlags & (1u << 12)) != 0;
        christmasBonus = (skillFlags & (1u << 13)) != 0;
        hungryWolf = (skillFlags & (1u << 14)) != 0;
        marketplaceGenius = (skillFlags & (1u << 15)) != 0;
        hardWorker = (skillFlags & (1u << 16)) != 0;
        deathDose = (skillFlags & (1u << 17)) != 0;
        failToWin = (skillFlags & (1u << 18)) != 0;
        failureGrind = (skillFlags & (1u << 19)) != 0;
        michealScott = (skillFlags & (1u << 20)) != 0;

        skillPointsLimit.UpdateButton();
    public void ResetCharacter()
    {
        activeIdle = false;
        noMatterWhat = false;
        chickenDinner = false;
        alwaysWinner = false;
        symbiosis = false;
        mortalClicker = false;
        championOfClicks = false;
        reactionTest = false;
        unskilledPredator = false;
        oneForEveryone = false;
        whatEyesDontSee = false;
        passiveAgressive = false;
        multitasking = false;
        christmasBonus = false;
        hungryWolf = false;
        marketplaceGenius = false;
        hardWorker = false;
        deathDose = false;
        failToWin = false;
        failureGrind = false;
        michealScott = false;

        skillpoints = 0;
        level = 1;
        currentExp = 0;
        maxExpCap = 100;
        maxLvlCap = 20;

        criticalChance = 15.5f;
        boostedChance = 0.00f;
        skillCheckChance = -1.0f;
        skillCheckReduce = 0;
        productionIdleBonus = 0;
        productionIdlePedatorBonus = 0;
        productionIdleAgressiveBonus = 0;
        sellingBonus = 1;
        sellingHardBonus = 1;
        potionBoost = 1;
        potionRandomChance = -1.0f;
        expBoost = 1;
        komboBoost = 0;


        T1 = 0;
        T2 = 0;
        T3 = 0;
}

    private void Update()
    {
        if(failToWin && expBoost>1) buffManager.ApplyBuff("Skill3D-tree2", expBoost);
        else buffManager.RemoveBuff("Skill3D-tree2");
        if (failureGrind && komboBoost > 0) buffManager.ApplyBuff("Skill4-tree2", komboBoost);
        else buffManager.RemoveBuff("Skill4-tree2");
        if (marketplaceGenius && sellingBonus > 0) buffManager.ApplyBuff("Skill2A-tree2", sellingBonus);
        else buffManager.RemoveBuff("Skill2A-tree2");
        if (unskilledPredator && productionIdlePedatorBonus > 0) buffManager.ApplyBuff("Skill4A-tree3", productionIdlePedatorBonus);
        else buffManager.RemoveBuff("Skill4A-tree3");
    }
}
