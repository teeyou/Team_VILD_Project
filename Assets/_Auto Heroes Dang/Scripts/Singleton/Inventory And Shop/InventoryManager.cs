using System;
using System.Collections.Generic;
using UnityEngine;

/*
 외부에서 호출 시 
 ItemData newSword = new ItemData(ItemType.Sword, Grade.Rare, 10, "아이템 설명");
 InventoryManager.Instance.AddItem(newSword);

 안에 들어가는 데이터는 ItemData F12
 */

public class InventoryManager : Singleton<InventoryManager>, IItemManage
{
    private List<ItemData> _items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => _items;

    public event Action OnChanged;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 테스트용
        AddItem(new ItemData("흔한 모자", ItemType.Hat, Grade.Common, 1, 11, 100, "평범한 모자"));
        AddItem(new ItemData("짱 센 갑옷", ItemType.Armor, Grade.Epic, 50, 11, 9999, "짱짱 센 갑옷"));
        AddItem(new ItemData("흔한 검", ItemType.Sword, Grade.Common, 1, 10, 200, "흔한 검"));
        AddItem(new ItemData("흔한 검", ItemType.Sword, Grade.Common, 1, 10, 200, "흔한 검"));
        AddItem(new ItemData("흔한 검", ItemType.Sword, Grade.Common, 1, 10, 200, "흔한 검"));
        AddItem(new ItemData("흔한 검", ItemType.Sword, Grade.Common, 1, 10, 200, "흔한 검"));
        AddItem(new ItemData("흔한 검", ItemType.Sword, Grade.Common, 1, 10, 200, "흔한 검"));
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

    public bool ReplaceItem(ItemData oldItem, ItemData newItem)
    {
        int index = _items.FindIndex(x =>
            x.name == oldItem.name &&
            x.type == oldItem.type &&
            x.grade == oldItem.grade &&
            x.level == oldItem.level &&
            x.value == oldItem.value &&
            x.price == oldItem.price &&
            x.description == oldItem.description &&
            x.state == oldItem.state);

        if (index < 0)
            return false;

        _items[index] = newItem;
        OnChanged?.Invoke();
        return true;
    }

    public bool Contains(ItemData item)
    {
        return _items.Contains(item);
    }

    public void Clear()
    {
        _items.Clear();
        OnChanged?.Invoke();
    }
}