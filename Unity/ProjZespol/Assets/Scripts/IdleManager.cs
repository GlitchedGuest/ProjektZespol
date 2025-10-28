using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleManager : MonoBehaviour
{
    [SerializeField] private RaptorCore raptorCore;
    [SerializeField] private float tickInterval = 1f; // 20 ticków/sek
    private Coroutine tickCoroutine;
    [SerializeField] private List<Factory> factories;

    private void Awake()
    {
        if (factories == null) factories = new List<Factory>();
        if (factories.Count == 0)
        {
            factories.Add(new Factory("Fabryka1"));
        }
        Debug.Log("IdleManager: dodano domyślną fabrykę.");
    }

    private void OnEnable()
    {
        if (tickCoroutine == null)
        {
            tickCoroutine = StartCoroutine(TickLoop());
        }
   }

    private void OnDisable()
    {
        if (tickCoroutine != null)
        {
            StopCoroutine(tickCoroutine);
            tickCoroutine = null;
        }
    }

    private IEnumerator TickLoop()
    {
        var wait = new WaitForSecondsRealtime(tickInterval);
        while (true)
        {
            if (raptorCore != null)
            {
                ProcessTick();
            }
            yield return wait;
        }
    }
    
    private void ProcessTick()
    {
        if (factories == null || factories.Count == 0) return;

        double totalProduction = 0.0;
        foreach (var f in factories)
        {
            if (f == null) continue;
            totalProduction += f.GetProduction();
        }

        raptorCore.Currency += Math.Ceiling(totalProduction);

 

        LayoutController.Instance?.SetCurrencyText(raptorCore.Currency.ToString());
    }

    public bool BuyFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            Debug.LogWarning($"BuyFactory: nieprawidlowy indeks {index}");
            return false;
        }

        var f = factories[index];
        double cost = f.currentCost;
        if (raptorCore.Gold >= cost)
        {
            raptorCore.Gold -= cost;
            f.count++;
            Debug.Log($"Kupiono fabrykę '{f.name}' (nowy count = {f.count}), koszt = {cost}");
            LayoutController.Instance?.SetCurrencyText(raptorCore.Currency.ToString());
            return true;
        }
        else
        {
            Debug.Log("BuyFactory: brak wystarczajacych srodkow");
            return false;
        }
    }

    public bool SellFactory(int index)
    {
        if (factories == null || index < 0 || index >= factories.Count)
        {
            Debug.LogWarning($"SellFactory: nieprawidlowy indeks {index}");
            return false;
        }

        var f = factories[index];
        if (f.count <= 0)
        {
            Debug.Log("SellFactory: brak fabryk do sprzedania");
            return false;
        }

        double sellValue = f.GetSellValue(); // obliczane na podstawie (count - 1)
        f.count--;
        raptorCore.Gold += sellValue;
        Debug.Log($"Sprzedano fabrykę '{f.name}' za {sellValue} (nowy count = {f.count})");
        LayoutController.Instance?.SetGoldText(raptorCore.Gold.ToString());
        return true;
    }
}