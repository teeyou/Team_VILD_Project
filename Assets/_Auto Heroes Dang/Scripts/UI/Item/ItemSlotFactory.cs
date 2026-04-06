using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlotFactory : MonoBehaviour
{
    [SerializeField] private Transform _itemSlotParent;
    [SerializeField] private List<ItemPrefabData> _prefabList;

    [System.Serializable]
    private class ItemPrefabData
    {
        public ItemType type;
        public GameObject prefab;
    }

    [SerializeField] private GameObject _emptySlot;

    private Dictionary<ItemType, GameObject> _prefabDictionary;
    private Dictionary<Grade, Color> _gradeColor;       // 타입별 컬러


    private Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    public void RefreshUI(IItemManage iItem, int verticalSlot, Action<ItemData> onClick)
    {
        SetupPrefab();
        ClearAllSlot();

        var items = iItem.GetItems();

        int slotAmount = Math.Max((items.Count + verticalSlot - 1) / verticalSlot, 3) * verticalSlot;

        // 아이템 슬롯 생성
        for (int i = 0; i < items.Count; i++)
        {
            if (!_prefabDictionary.TryGetValue(items[i].type, out GameObject prefab))
            {
                Debug.LogWarning($"프리팹 없음: {items[i].type}");
                continue;
            }

            GameObject obj = Instantiate(prefab, _itemSlotParent);
            ItemPrefab slot = obj.GetComponent<ItemPrefab>();

            slot.Init(items[i], _gradeColor[items[i].grade], onClick);
        }

        // 빈 슬롯 채우기
        for (int i = 0; i < (slotAmount - items.Count); i++)
        {
            Instantiate(_emptySlot, _itemSlotParent);
        }
    }

    private void SetupPrefab()
    {
        if (_gradeColor != null) return;

        _prefabDictionary = new Dictionary<ItemType, GameObject>();

        foreach (ItemPrefabData data in _prefabList)
        {
            _prefabDictionary[data.type] = data.prefab;
        }

        _gradeColor = new Dictionary<Grade, Color>()
    {
        { Grade.Common, Hex("#B1B2B5") },
        { Grade.Uncommon, Hex("#00A2FF") },
        { Grade.Rare, Hex("#9A41F6") },
        { Grade.Elite, Hex("#F83BF9") },
        { Grade.Epic, Hex("#FFC508") }
    };
    }



    public void ClearAllSlot()
    {
        foreach (Transform child in _itemSlotParent)
        {
            Destroy(child.gameObject);
        }

    }
}

