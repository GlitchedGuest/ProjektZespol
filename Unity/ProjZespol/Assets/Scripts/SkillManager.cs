using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private RaptorCore raptorCore;
    private CharacterClass characterClass;
    private IdleManager idleManager;
    
    [SerializeField] private GameObject skillCheck;
    
    private bool skillCheckCooldown = false;
    private int skillCheckCooldownCount = 0;

    public bool SkillCheckCooldown => skillCheckCooldown;

    public void Initialize(RaptorCore _raptorCore, CharacterClass charClass, IdleManager idle, GameObject skillCheckObj)
    {
        raptorCore = _raptorCore;
        characterClass = charClass;
        idleManager = idle;
        skillCheck = skillCheckObj;
    }

    public void CheckAndTriggerSkillCheck()
    {
        if (!skillCheckCooldown && !characterClass.reactionTest)
        {
            TriggerSkillCheck();
        }
    }

    public void TriggerSkillCheck()
    {
        Debug.Log("SKILL CHECK");
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        float boostChance = 0;
        
        if (characterClass.symbiosis)
            boostChance = characterClass.GetCriticalChance();
            
        if (chance < characterClass.GetSkillCheckChance() + boostChance)
        {
            skillCheck.SetActive(true);
            skillCheck.GetComponent<SkillCheckScript>().StartSkillCheck();
            skillCheckCooldown = true;
        }      
    }

    public void UpdateSkillCheckCooldown()
    {
        if(skillCheckCooldown)
            skillCheckCooldownCount++;
            
        if (skillCheckCooldownCount % (600 - characterClass.skillCheckReduce) == 0)
        { 
            skillCheckCooldown = false;
            skillCheckCooldownCount = 0;
        }
    }

    public void CheckReactionTest(int tickCount)
    {
        if (characterClass.reactionTest && !skillCheckCooldown)
        {
            if (tickCount % 200 == 0)
                TriggerSkillCheck();
        }
    }

    public IEnumerator EnableActiveIdle()
    {
        string currentResource = raptorCore.ResourceManager.currentResource;
        
        foreach (var f in idleManager.factories)
        {
            if (f.resource.name == currentResource)
            {
                f.productionMultiplier += 3;
                f.count += 4;
                if(characterClass.activeIdle) characterClass.buffManager.ApplyBuff("Skill3A", f.count*f.productionMultiplier);
                yield return new WaitForSeconds(6f);
                f.productionMultiplier -= 3;
                f.count -= 4;
                if (characterClass.activeIdle) characterClass.buffManager.ApplyBuff("Skill3A", f.count * f.productionMultiplier);
                break;
            }
        }
    }
    public double GetActiveIdleValue()
    {
        string currentResource = raptorCore.ResourceManager.currentResource;

        foreach (var f in idleManager.factories)
        {
            if (f.resource.name == currentResource)
            {
               return (f.count * f.productionMultiplier);

            }
        }
        return 0;
    }

    public void EnableChickenDinner(bool effect)
    {
        if(characterClass.chickenDinner)
            StartCoroutine(ChickenDinnerEffect(effect));
    }

    public IEnumerator ChickenDinnerEffect(bool effect)
    {
        int mode = 1;
        if (!effect)
        {
            mode = -1;
            if (characterClass.alwaysWinner)
                mode = 0;
        }
        raptorCore.SkillMultiplier += 10 * mode;
        if (characterClass.chickenDinner) characterClass.buffManager.ApplyBuff("Skill2B", raptorCore.SkillMultiplier);
        yield return new WaitForSeconds(6f);
        raptorCore.SkillMultiplier -= 10 * mode;
        if (characterClass.chickenDinner) characterClass.buffManager.ApplyBuff("Skill2B", raptorCore.SkillMultiplier);
    }

    public bool IsSkillCheckActive()
    {
        return skillCheck.activeSelf;
    }

    public void RandomPotionEffect()
    {
        var chance = UnityEngine.Random.Range(0.00f, 100.00f);
        if (chance < characterClass.potionRandomChance)
        {
            var potionIndex = (int)UnityEngine.Random.Range(0f, 6f);
            var potion = idleManager.potions[potionIndex];
            potion.isActive = true;
            potion.timeRemaining += potion.duration;
            potion.linkedFactory = raptorCore.ResourceManager.GetCurrentFactory();
            idleManager.EnablePotionEffect(potion);
        }
    }

}