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

    public void RefreshUI(IItemManage iItem, int verticalSlot, Action<ItemData> onClick, Func<ItemData, bool> isSelected)
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

            // 강화 / 합성에서 선택된 아이템이면 체크 표시 및 클릭 불가
            bool selected = isSelected != null && isSelected(items[i]);
            slot.SetSelected(selected);

            // 상태 체크
            if (items[i].state == ItemState.SoldOut)
            {
                slot.SoldOut(true);
            }

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
        { Grade.Epic, Hex("#FFC508") },

        { Grade.AtkBuff, Hex("#EC0037") },
        { Grade.DefBuff, Hex("#00C468") }
    };
    }


    public void ClearAllSlot()
    {
        foreach (Transform child in _itemSlotParent)
        {
            Destroy(child.gameObject);
        }

    }

    // 아이콘 생성용 프리팹 정보 전송 (주로 상점등에서)
    public GameObject CreateIcon(ItemType type, Transform parent)
    {
        SetupPrefab();

        if (!_prefabDictionary.TryGetValue(type, out GameObject prefab))
        {
            Debug.LogWarning($"프리팹 없음: {type}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    // 컬러 정보 전송
    public Color GetGradeColor(Grade grade)
    {
        SetupPrefab();

        if (_gradeColor.TryGetValue(grade, out Color color))
        {
            return color;
        }

        return Color.white;
    }

    /*
      GameObject icon = _factory.CreateIcon(item.type, _iconTransform.transform);

        if (icon != null)
        {
            RectTransform rt = icon.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = _popupIconPosition.pos;
                rt.sizeDelta = _popupIconPosition.size;
            }

            ItemPrefab slot = icon.GetComponent<ItemPrefab>();
            if (slot != null)
            {
                Color gradeColor = _factory.GetGradeColor(item.grade);

                slot.Init(item, gradeColor, null);
                slot.SetButtonInteractable(false);
            }
        }
     */

}

