using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public interface IBinarySaveable
{
    int SaveKey { get; }
    void SerializeToStream(BinaryWriter writer);
    void DeserializeFromStream(BinaryReader reader);
}

public static class AutoSaveSystem
{
    //default path for save.
    public static string SaveFileName { get; set; } = "autosave.dat";

    private static readonly string path =
       Path.Combine(Application.persistentDataPath, "MetaData.dat");

    private static bool loaded = false;
    private static bool _AutoSave;
    
    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
    private static string SavePathS(string FileName) => Path.Combine(Application.persistentDataPath, FileName);

    public static bool AutoSave
    {
        get
        {
            LazyGetAutoSave();
            return _AutoSave;
        }
        set
        {
            _AutoSave = value;
            SetAutoSave(_AutoSave);
        }
    }

    public static void LazyGetAutoSave()
    {
        if (loaded) return;
        loaded = true;
        _AutoSave = GetAutoSave();
    }

    private static bool GetAutoSave()
    {
        if (!File.Exists(path))
            return true;               

        byte b = File.ReadAllBytes(path)[0];
        return b == 1;
    }

    private static void SetAutoSave(bool state)
    {
        byte value = state ? (byte)1 : (byte)0;
        File.WriteAllBytes(path, new byte[] { value });
    }

    public static void DeleteSave()
    {
        File.Delete(SavePath);
    }




    // Exposed Save and load
    public static void SaveGame()
    {
        using (var stream = File.Create(SavePath))
        using (var writer = new BinaryWriter(stream))
        {
            SaveAll(writer);
        }

        Debug.Log($"[AutoSaveSystem] Saved binary data to: {SavePath}");
    }

    public static void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning($"[AutoSaveSystem] No save file found at: {SavePath}");
            return;
        }

        using (var stream = File.OpenRead(SavePath))
        using (var reader = new BinaryReader(stream))
        {
            LoadAll(reader);
        }

        Debug.Log($"[AutoSaveSystem] Loaded binary data from: {SavePath}");
    }

    // ReadWrite Functions

    // Save all IBinarySaveable MonoBehaviours in the scene
    public static void SaveAll(BinaryWriter writer)
    {
        var saveables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsSortMode.None).OfType<IBinarySaveable>().OrderBy(s => s.SaveKey); ;

        foreach (var behaviour in saveables)
        {
            behaviour.SerializeToStream(writer);
        }
    }

    // Load all IBinarySaveable MonoBehaviours in the scene
    public static void LoadAll(BinaryReader reader)
    {
        var saveables = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsSortMode.None).OfType<IBinarySaveable>().OrderBy(s => s.SaveKey); ;

        foreach (var behaviour in saveables)
        {
            behaviour.DeserializeFromStream(reader);
        }
    }

}
