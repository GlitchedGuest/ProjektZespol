using System;
using UnityEngine;

public static class PrestigeSystem
{
    private static RaptorCore _core;
    private static bool _initialized;
    public static void Init(RaptorCore core)
    {
        if (core == null)
            throw new ArgumentNullException(nameof(core));

        if (_initialized)
            return;

        _core = core;
        _initialized = true;
    }   

    public static bool CheckPrestige(double val)
    {
        return _core.Gold >= val;
    }

    public static void ExecutePrestige()
    {
        //Calculate Prestige

        _core.OneClickPrestige += _core.characterClass.T1;
        _core.JackPrestige += _core.characterClass.T2;
        _core.AutoPrestige += _core.characterClass.T3;
        _core.PrestigeCount++;

        //Great Reset
        _core.characterClass.ResetCharacter();
        _core.ResourceManager.ResetResources();
        _core.idleManager.ResetFactory();
        _core.layoutController.ResetGui();

        _core.ResetCore();

        _core.layoutController.prestigeUIManager.UpdateContentPage();
    }

}
