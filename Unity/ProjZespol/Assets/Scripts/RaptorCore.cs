using System;
using UnityEngine;

public class RaptorCore : MonoBehaviour
{

    [AutoSave] public QuarkType Currency = 0;

    //Click based
    [AutoSave] QuarkType CBasevalue = 1;
    [AutoSave] QuarkType CMultiplier = 1;


    //Click based
    [AutoSave] QuarkType GBasevalue = 0;
    [AutoSave] QuarkType GMultiplier = 1;


    int tickCount = 0;

    [SerializeField] private Animator anim;
    [SerializeField] private CharacterClass characterClass;
    [AutoSave] public double Gold = 0;

    private void Awake()
    {
        UpdateUI();
    }

    private void OnMouseDown()
    {
        this.click();
        anim.SetTrigger("Clicked");
        UpdateUI();
    }
    void click()
    {
        QuarkType value = 0;
        
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        if (chance < characterClass.GetCriticalChance())
        {
            value += (CBasevalue * CMultiplier * 3); //to do zmiany gdy będzie wchodzić temat balansu
            Debug.Log("Kryt " + chance);
        }
        else
            value = (CBasevalue * CMultiplier);
        Currency += value;
    }

    void Start()
    {
        
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
        AutoSaveSystem.LoadGame();
    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier); //.Pow(GPower);

        tickCount++;
        if (tickCount%600 == 0 )
            AutoSaveSystem.SaveGame();
    }

    void Update()
    {
        
    }
    void UpdateUI()
    {
        
        LayoutController.Instance?.SetCurrencyText(Currency.ToString());
        LayoutController.Instance?.SetGoldText(Gold.ToString());
    }

    public double SellMaterials(float sellValue)
    {
        var tmp = (Currency * (sellValue / 100f)).Ceil();
        Gold += tmp;
        Currency -= tmp;
        UpdateUI();
        return (double)tmp; // tymczasowe rozwiazanie zwracanie ilosci exp
    }

}
