using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManagerEquip : Singleton<ShopManagerEquip>, IItemManage
{
    private List<ItemData> _items = new List<ItemData>();

    public event Action OnChanged;

    private static int _stageNum; // 게임 껏다 켜면 상점 리세마라 가능하게.
    private bool _started = true;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _stageNum = (int)GameManager.Instance.CurrentStage;
        SetShopItem();
    }


    public IReadOnlyList<ItemData> GetItems() => _items;


    public void AddItem(ItemData item)
    {
        _items.Add(item);
        OnChanged?.Invoke();
    }

    public void RemoveItem(ItemData item)
    {
        _items.Remove(item); // 이 부분 삭제 시, 아이템이 없어지지 않고 솔드아웃 체크만 되어 있습니다.
        OnChanged?.Invoke();
    }

    public void SetShopItem()
    {
        int currentNum = (int)GameManager.Instance.CurrentStage;
        if (_stageNum == currentNum && !_started) return;

        int count = 5 + UnityEngine.Random.Range(0,4); // 모호한 참조

        _items.Clear();

        for (int i = 0; i < count; i++)
        {
            int rand = UnityEngine.Random.Range(0, 100);

            Grade grade;

            if (rand <= 40)
                grade = Grade.Common;
            else if (rand <= 70)
                grade = Grade.Uncommon;
            else if (rand <= 90)
                grade = Grade.Rare;
            else if (rand <= 98)
                grade = Grade.Elite;
            else
                grade = Grade.Epic;

            int typeRand = UnityEngine.Random.Range(0, 5);
            ItemType type = (ItemType)typeRand;

            AddItemGrade(grade, type);
        }

        _stageNum = currentNum;

        _started = false;
    }


    public void AddItemGrade(Grade grade, ItemType type)
    {
        int value = ItemDescription.GetBaseValue(grade);
        int price = ItemDescription.GetPrice(grade);
        string dis = ItemDescription.GetBaseDescription(grade);

        string name;
        switch (type)
        {
            case ItemType.Sword: name = "검"; break;
            case ItemType.Hat: name = "모자"; break;
            case ItemType.Armor: name = "갑주"; break;
            case ItemType.Shoes: name = "신발"; break;
            case ItemType.Ring: name = "반지"; break;
            default: name = "모자"; break;
        }

        AddItem(new ItemData($"{ItemName.GetPrefix(grade)} {name}", type, grade, 1, value, price, dis));

    }

}