using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManagerPotion : Singleton<ShopManagerPotion>, IItemManage
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
        AddItem(new ItemData("고기", ItemType.Sword, Grade.Rare, 5, 500, "힘이 세지는 고기"));
        AddItem(new ItemData("풀", ItemType.Ring, Grade.Epic, 1, 2000, "방어가 강해지는 풀"));
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