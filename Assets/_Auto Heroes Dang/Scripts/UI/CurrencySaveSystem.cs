using System.IO;
using UnityEngine;
using System;

[Serializable]
public class CurrencySaveData
{
    public int gem;
    public int gold;
}

public static class CurrencySaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "currency.json");

    public static void Save(int gem, int gold)
    {
        CurrencySaveData data = new CurrencySaveData
        {
            gem = gem,
            gold = gold
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"재화 저장 완료 : {SavePath}");
    }

    public static CurrencySaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            CurrencySaveData defaultData = new CurrencySaveData
            {
                gem = 10,
                gold = 1000
            };

            string json = JsonUtility.ToJson(defaultData, true);
            File.WriteAllText(SavePath, json);

            Debug.Log($"재화 저장 파일이 없어 기본값 생성 : {SavePath}");
            return defaultData;
        }

        string loadedJson = File.ReadAllText(SavePath);
        CurrencySaveData loadedData = JsonUtility.FromJson<CurrencySaveData>(loadedJson);

        // Debug.Log($"재화 불러오기 완료 : {SavePath}");
        return loadedData;
    }
}