using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;


// 현재 스테이지
// 캐릭터들 현재 위치
// 캐릭터들 스텟 정보
// 스테이지 클리어 여부


// 포션 사용 유무
// 인벤토리 장비 아이템

[Serializable]
public class SaveData
{
    public int mainCharacterIdx;
    public List<int> playerCharacterList;
    public PosRotData mainTransform;
    public List<PosRotData> subTransformList;
    public float cartPosition;
    public int currentWayPointIdx;
    public int currentStage;
    public List<PlayerRuntimeData> playerRuntimeDataList;
    public bool isFirstPoint;
    public bool isStageClear;
    public bool isSpawnPossible;
    public List<ItemData> inventoryItems;   // 0413 아이템 및 장비 추가
    public ItemData[] equipments;
    public bool atkPotionOn;
    public bool defPotionOn;
}

public class SaveSystem
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "AutoHeroesSaveData.json");

    public void Save()
    {
        SaveData data = new SaveData
        {
            mainCharacterIdx = DataSource.Instance.MainCharacterIdx,
            playerCharacterList = DataSource.Instance.GetCharacterList(),
            mainTransform = DataSource.Instance.MainCharacterPosRot,
            subTransformList = DataSource.Instance.GetSubTransformList(),
            cartPosition = DataSource.Instance.CartPosition,
            currentWayPointIdx = DataSource.Instance.CurrentIdx,
            currentStage = (int)GameManager.Instance.CurrentStage,
            playerRuntimeDataList = DataSource.Instance.GetPlayerRuntimeDataList(),
            isFirstPoint = GameManager.Instance.IsFirstPoint,
            isStageClear = GameManager.Instance.IsStageClear,
            isSpawnPossible = FieldManager.Instance.IsSpawnPossible,
            inventoryItems = new List<ItemData>(InventoryManager.Instance.Items), //0413 아이템 및 장비 추가 부분
            equipments = InventoryManager.Instance.GetEquipmentData(),
            atkPotionOn = DataSource.Instance.atkPotionOn,
            defPotionOn = DataSource.Instance.defPotionOn

        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("게임 데이터 저장 완료");
        Debug.Log($"저장 경로 : {SavePath}");
    }

    public SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("불러올 파일 없음");
            return null;
        }

        string loadedJson = File.ReadAllText(SavePath);
        SaveData loadedData = JsonUtility.FromJson<SaveData>(loadedJson);

        SyncNextItemId(loadedData);

        Debug.Log("게임 데이터 로드");

        return loadedData;
    }

    private void SyncNextItemId(SaveData loadedData)
    {
        int maxId = 0;

        if (loadedData.inventoryItems != null)
        {
            for (int i = 0; i < loadedData.inventoryItems.Count; i++)
            {
                maxId = Mathf.Max(maxId, loadedData.inventoryItems[i].uniqueId);
            }
        }

        if (loadedData.equipments != null)
        {
            for (int i = 0; i < loadedData.equipments.Length; i++)
            {
                maxId = Mathf.Max(maxId, loadedData.equipments[i].uniqueId);
            }
        }

        ItemIdGenerator.SetNextId(maxId + 1);
        Debug.Log($"다음 아이템 ID 동기화: {maxId + 1}");
    }
}
