using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    private List<ItemData> _shopItems = new List<ItemData>();
    public IReadOnlyList<ItemData> ShopItems => _shopItems;

    public event Action OnShopChanged;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AddItem(new ItemData("상점 검", ItemType.Sword, Grade.Rare, 5, 500, "상점 검"));
        AddItem(new ItemData("상점 반지", ItemType.Ring, Grade.Epic, 1, 2000, "상점 반지"));
    }

    public void AddItem(ItemData item)
    {
        _shopItems.Add(item);
        OnShopChanged?.Invoke();
    }

    public void RemoveItem(ItemData item)
    {
        _shopItems.Remove(item);
        OnShopChanged?.Invoke();
    }

    public void Clear()
    {
        _shopItems.Clear();
        OnShopChanged?.Invoke();
    }
}