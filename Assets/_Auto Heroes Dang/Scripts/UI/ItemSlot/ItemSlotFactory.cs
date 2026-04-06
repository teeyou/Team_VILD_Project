using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private int _verticalSlot; // 가로에 들어가는 아이템 갯수
    private int _slotAmount;

    private List<ItemData> _currentItemList = new List<ItemData>();

    private Dictionary<ItemType, GameObject> _prefabDictionary;
    private Dictionary<Grade, Color> _gradeColor;       // 타입별 컬러


    private void Awake()
    {
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

    private void OnEnable()
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        ClearAllSlot();
        CreateItemSlot(new List<ItemData>(InventoryManager.Instance.Items));
    }

    private Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    // 아이템 슬롯 채우기 (데이터, 가로에 들어가는 슬롯 개수(3~5))
    public void CreateItemSlot(List<ItemData> itemList)
    {
        _slotAmount = Math.Max((itemList.Count + _verticalSlot - 1) / _verticalSlot, 3) * _verticalSlot;

        _currentItemList = new List<ItemData>(itemList);

        for (int i = 0; i < itemList.Count; i++)
        {
            if (!_prefabDictionary.TryGetValue(itemList[i].type, out GameObject prefab))
            {
                Debug.LogWarning($"해당 타입 프리팹을 찾을 수 없음 -> {itemList[i].type}");
                continue;
            }

            GameObject Slot = Instantiate(prefab, _itemSlotParent);

            Image colorImage = Slot.transform.Find("Color").GetComponent<Image>();
            colorImage.color = _gradeColor[itemList[i].grade];

            TMP_Text levelText = Slot.transform.Find("Level").GetComponent<TMP_Text>();
            levelText.text = $"Lv. {itemList[i].level}";
        }

        // 남은 슬롯 채우기
        for (int i = 0; i < (_slotAmount - itemList.Count); i++)
        {
            Instantiate(_emptySlot, _itemSlotParent);
        }

    }

    public void ClearAllSlot()
    {
        foreach (Transform child in _itemSlotParent)
        {
            Destroy(child.gameObject);
        }

        _currentItemList.Clear();
    }

}

