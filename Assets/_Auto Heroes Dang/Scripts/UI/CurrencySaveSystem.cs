using System;
using System.IO;
using UnityEngine;

[Serializable]
public class CurrencySaveData
{
    public int gold;
    public int gem;
}

public static class CurrencySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "currency.json");

    public static void Save(CurrencySaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static CurrencySaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            CurrencySaveData newData = new CurrencySaveData
            {
                gold = 0,
                gem = 0
            };

            Save(newData);
            return newData;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<CurrencySaveData>(json);
    }
}