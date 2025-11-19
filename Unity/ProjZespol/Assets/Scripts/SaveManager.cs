using System;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AutoSaveAttribute : Attribute { }

public interface IBinarySaveable
{
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
            WriteAll(writer);
        }

        Debug.Log($"[AutoSaveSystem] Saved binary data to: {SavePath}");
    }

    public static void SaveGame(string Name)
    {
        using (var stream = File.Create(SavePathS(Name)))
        using (var writer = new BinaryWriter(stream))
        {
            WriteAll(writer);
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
            ReadAll(reader);
        }

        Debug.Log($"[AutoSaveSystem] Loaded binary data from: {SavePath}");
    }

    // ReadWrite Functions

    private static void WriteAll(BinaryWriter writer)
    {
        var behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);


        foreach (var behaviour in behaviours)
        {
            var type = behaviour.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<AutoSaveAttribute>() == null)
                    continue;

                var value = field.GetValue(behaviour);
                WriteValue(writer, value);
            }
        }
    }

    private static void ReadAll(BinaryReader reader)
    {
        var behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);


        foreach (var behaviour in behaviours)
        {
            var type = behaviour.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<AutoSaveAttribute>() == null)
                    continue;

                var current = field.GetValue(behaviour);
                var newValue = ReadValue(reader, current?.GetType());
                field.SetValue(behaviour, newValue);
            }
        }
    }

    // Helper Conversion methods to get data from stream to value or obj

    private static void WriteValue(BinaryWriter writer, object value)
    {
        writer.Write(value != null);

        if (value == null)
            return;

        switch (value)
        {
            case IBinarySaveable custom:
                custom.SerializeToStream(writer);
                break;

            case int i: writer.Write(i); break;
            case float f: writer.Write(f); break;
            case double d: writer.Write(d); break;
            case bool b: writer.Write(b); break;
            case string s: writer.Write(s); break;

            default:
                Debug.LogWarning($"[AutoSaveSystem] Unsupported type {value.GetType().Name} — skipping");
                break;
        }
    }

    private static object ReadValue(BinaryReader reader, Type expectedType)
    {
        if (!reader.ReadBoolean())
            return null;

        return expectedType switch
        {
            Type t when t == typeof(int) => reader.ReadInt32(),
            Type t when t == typeof(float) => reader.ReadSingle(),
            Type t when t == typeof(double) => reader.ReadDouble(),
            Type t when t == typeof(bool) => reader.ReadBoolean(),
            Type t when t == typeof(string) => reader.ReadString(),
            Type t when typeof(IBinarySaveable).IsAssignableFrom(t) => CreateAndDeserialize(t, reader),
            _ => DefaultValue(expectedType)
        };
    }
    private static object CreateAndDeserialize(Type type, BinaryReader reader)
    {
        var instance = (IBinarySaveable)Activator.CreateInstance(type);
        instance.DeserializeFromStream(reader);
        return instance;
    }

    private static object DefaultValue(Type t)
        => t?.IsValueType == true ? Activator.CreateInstance(t) : null;
}
