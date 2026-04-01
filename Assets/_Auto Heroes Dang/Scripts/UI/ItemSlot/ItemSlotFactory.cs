using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
외부에서 호출 시 

1. [SerializeField] private ItemSlotFactory 팩토리;

2. List<ItemSlotFactory.ItemData> 리스트이름 = new List<ItemSlotFactory.ItemData>()
        {
            new ItemSlotFactory.ItemData(이름, 등급, 레벨),
            new ItemSlotFactory.ItemData(ItemSlotFactory.ItemType.Sword, ItemSlotFactory.Grade.Rare, 10),
            new ItemSlotFactory.ItemData(ItemSlotFactory.ItemType.Hat, ItemSlotFactory.Grade.Common, 2),
        };
3. 팩토리.CreateItemSlot(리스트이름); <- 생성용
   팩토리.ClearAllSlot(리스트이름); <- 클리어용

 */

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
    [SerializeField] private int _count;

    private List<ItemData> _currentItemList = new List<ItemData>();

    private Dictionary<ItemType, GameObject> _prefabDictionary;
    private Dictionary<Grade, Color> _gradeColor;       // 타입별 컬러

    public enum ItemType
    {
        Sword,
        Hat,
        Armor,
        Shoes,
        Ring,
    }

    public enum Grade
    {
        Common,
        Uncommon,
        Rare,
        Elite,
        Epic,
    }

    public struct ItemData
    {
        public ItemType type;
        public Grade grade;
        public int level;

        public ItemData(ItemType type, Grade grade, int level)
        {
            this.type = type;
            this.grade = grade;
            this.level = level;
        }
    }


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

    private void Start()
    {

        ClearAllSlot();

        // ------------------------ 테스트용 추후 삭제 -----------------------
        List<ItemData> testItems = new List<ItemData>()
        {
            new ItemData(ItemType.Sword, Grade.Common, 3),
            new ItemData(ItemType.Hat, Grade.Rare, 5),
            new ItemData(ItemType.Armor, Grade.Uncommon, 6),
            new ItemData(ItemType.Shoes, Grade.Elite, 7),
            new ItemData(ItemType.Ring, Grade.Epic, 12),
        };

        CreateItemSlot(testItems);

        // -------------------------------------------------------------------

    }

    private Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    public void CreateItemSlot(List<ItemData> itemList)
    {
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
        for (int i = 0; i < (_count-itemList.Count); i++)
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

