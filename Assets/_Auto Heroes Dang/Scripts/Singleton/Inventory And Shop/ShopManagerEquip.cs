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
        AddItem(new ItemData("상점 검", ItemType.Sword, Grade.Rare, 5, 500, "상점 검"));
        AddItem(new ItemData("상점 반지", ItemType.Ring, Grade.Epic, 1, 2000, "상점 반지"));
    }


    public IReadOnlyList<ItemData> GetItems() => _items;


    public void AddItem(ItemData item)
    {
        _items.Add(item);
        OnChanged?.Invoke();
    }

    public void RemoveItem(ItemData item)
    {
        _items.Remove(item);
        OnChanged?.Invoke();
    }
}