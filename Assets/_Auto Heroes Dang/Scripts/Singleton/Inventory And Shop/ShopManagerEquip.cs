using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManagerEquip : Singleton<ShopManagerEquip>, IItemManage
{
    private List<ItemData> _items = new List<ItemData>();

    public event Action OnChanged;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 테스트용
        AddItem(new ItemData("상점 검", ItemType.Sword, Grade.Uncommon, 5, 11, 0, "상점 검"));
        AddItem(new ItemData("상점 검", ItemType.Armor, Grade.Uncommon, 5, 11, 0, "상점 검"));
        AddItem(new ItemData("상점 검", ItemType.Shoes, Grade.Uncommon, 5, 11, 0, "상점 검"));
        AddItem(new ItemData("상점 검", ItemType.Hat, Grade.Uncommon, 5, 11, 5000, "상점 검"));
        AddItem(new ItemData("상점 반지", ItemType.Ring, Grade.Uncommon, 1, 11, 2000, "상점 반지"));
        AddItem(new ItemData("상점 반지", ItemType.Ring, Grade.Epic, 1, 11, 2000, "상점 반지"));
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
}