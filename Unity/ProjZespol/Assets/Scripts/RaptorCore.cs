using System;
using UnityEngine;

public class RaptorCore : MonoBehaviour
{

    QuarkType Currency = 0;

    //Click based
    QuarkType CBasevalue = 1;
    QuarkType CMultiplier = 1;
    double CPower = 1;

    //Click based
    QuarkType GBasevalue = 0;
    QuarkType GMultiplier = 1;
    double GPower = 1;


    [SerializeField] private Animator anim;
    private double Gold = 0;

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

    public void SellMaterials(int sellValue)
    {
        
        if (Currency >= 1)
        {
            switch (sellValue)
            {
                case 1:
                    Currency -= 1;
                    Gold += 1; // narazie wpisalem 1 bo nie wiem co innego
                    break;
                case 2:
                    double tmp = Currency * 0.5;
                    Gold += tmp;
                    Currency -= tmp;
                    break;
                case 3:
                    Gold += Currency;
                    Currency = 0;
                    break;
            }
            UpdateUI();
        }
    }

}
