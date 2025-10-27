using System;
using UnityEngine;

public class RaptorCore : MonoBehaviour
{

    public QuarkType Currency = 0;

    //Click based
    QuarkType CBasevalue = 1;
    QuarkType CMultiplier = 1;
    double CPower = 1;

    //Click based
    QuarkType GBasevalue = 0;
    QuarkType GMultiplier = 1;
    double GPower = 1;


    [SerializeField] private Animator anim;
    public double Gold = 0;

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
        Currency += (CBasevalue * CMultiplier).Pow(CPower);
        
    }

    void Start()
    {
        Time.fixedDeltaTime = 0.05f; // 20 ticks a second
    }

    void FixedUpdate()
    {
        Currency += (GBasevalue * GMultiplier).Pow(GPower);
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
        double tmp;
        if (sellValue == 100) {
            Gold += Currency;
            tmp = Currency;
            Currency = 0;
        }
        else
        {
            tmp = Currency * Math.Round((sellValue / 100f), 2);
            Gold += tmp;
            Currency -= tmp;
        }

        UpdateUI();
        return tmp; // tymczasowe rozwiazanie zwracanie ilosci exp
    }

}
