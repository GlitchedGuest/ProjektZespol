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
}
