using System;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RaptorCore : MonoBehaviour
{

    public QuarkType Currency = 0;

    //Click based
    QuarkType CBasevalue = 1;
    QuarkType CMultiplier = 1;


    //Click based
    QuarkType GBasevalue = 0;
    QuarkType GMultiplier = 1;



    [SerializeField] private Animator anim;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private GameObject skillCheck;
    public double Gold = 0;

    private void Awake()
    {
        UpdateUI();
    }

    private void OnMouseDown()
    {
        if (!skillCheck.activeSelf) //nie lubie jak to wygląda, ale czasu nie ma broski
        {
            this.click();
            anim.SetTrigger("Clicked");
            UpdateUI();
        }
    }
    void click()
    {

            QuarkType value = 0;
            SkillCheckManager();//bardzo temp rozwiązanie później raczej losowo w czasie będzie sie skill check pojawiać, a nie podczas klikania w obiekt
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

    void SkillCheckManager()
    {
        float chance = UnityEngine.Random.Range(0.00f, 100.00f);
        if (chance < characterClass.GetSkillCheckChance())
        {
            skillCheck.SetActive(true);
            skillCheck.GetComponent<SkillCheckScript>().StartSkillCheck();
        }      
    }

    void Start()
    {
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier); //.Pow(GPower);


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

    public void AddCurrency(QuarkType currency)
    {
        Currency += currency;
        UpdateUI();
    }

    public void SubCurrency(QuarkType currency)
    {
        if (Currency < currency)
            Currency = 0;
        else
            Currency -= currency;
        UpdateUI();
    }

}
