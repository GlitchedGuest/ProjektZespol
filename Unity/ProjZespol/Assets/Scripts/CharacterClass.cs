using System;
using UnityEngine;

public class CharacterClass
{
    //this is class for the "rpg" character

    private uint level = 1;
    private ulong currentExp = 0;
    private ulong maxExpCap = 100;
    private uint maxLvlCap = 20;   
    //there will be other statistics like critical chance later etc


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

    //more methods for "rpg" character here
}
