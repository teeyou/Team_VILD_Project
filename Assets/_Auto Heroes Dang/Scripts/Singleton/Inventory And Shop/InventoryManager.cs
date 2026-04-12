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

    private ItemData[] _equipments = new ItemData[Enum.GetValues(typeof(ItemType)).Length]; // 장비중 아이템.
                                                                                            // 무기, 모자, 상의, 신발, 반지, 순서. 

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
        int index = _items.FindIndex(x => x.uniqueId == item.uniqueId);

        if (index < 0)
            return;

        _items.RemoveAt(index);
        OnChanged?.Invoke();
    }

    public bool ReplaceItem(ItemData oldItem, ItemData newItem)
    {
        //int index = _items.FindIndex(x =>
        //    x.name == oldItem.name &&
        //    x.type == oldItem.type &&
        //    x.grade == oldItem.grade &&
        //    x.level == oldItem.level &&
        //    x.value == oldItem.value &&
        //    x.price == oldItem.price &&
        //    x.description == oldItem.description &&
        //    x.state == oldItem.state);
        //
        //if (index < 0)
        //    return false;
        //
        //_items[index] = newItem;
        //OnChanged?.Invoke();
        //return true;

        int index = _items.FindIndex(x => x.uniqueId == oldItem.uniqueId);

        if (index < 0)
            return false;

        _items[index] = newItem;
        OnChanged?.Invoke();
        return true;
    }

    public bool Contains(ItemData item)
    {
        //return _items.Contains(item);
        return _items.Exists(x => x.uniqueId == item.uniqueId);
    }

    public void Clear()
    {
        _items.Clear();
        OnChanged?.Invoke();
    }


    // ---------장비용 스크립트 추가 부분 0413 ---------------------

    public void EquipItem(ItemData item)
    {
        if (!IsEquipType(item.type)) return;
        int index = (int)item.type;

        // 기존 장비 해제
        if (_equipments[index].uniqueId != 0)
        {
            UnequipItem(_equipments[index]);
        }

        _equipments[index] = item;
        ApplyStat(item, true);
        RemoveItem(item);
        OnChanged?.Invoke();
    }

    public void UnequipItem(ItemData item)
    {
        if (!IsEquipType(item.type)) return;
        int index = (int)item.type;

        if (_equipments[index].uniqueId == 0)
            return;

        ApplyStat(item, false);
        _equipments[index] = default;
        AddItem(item);
        OnChanged?.Invoke();
    }

    // 스탯 반영
    private void ApplyStat(ItemData item, bool isEquip)
    {
        int value = isEquip ? item.value : -item.value;

        bool isAtk = item.type == ItemType.Sword || item.type == ItemType.Ring;

        if (isAtk)
        {
            DataSource.Instance.IncreaseAtk(value);
        }
        else
        {
            DataSource.Instance.IncreaseDef(value);
        }
    }

    // 버프 걸러내기용
    public bool IsEquipType(ItemType type)
    {
        return !(type == ItemType.AtkBuff || type == ItemType.DefBuff);
    }

    public bool IsEquipped(ItemData item)
    {
        if (!IsEquipType(item.type))
            return false;

        int index = (int)item.type;

        return _equipments[index].uniqueId == item.uniqueId;
    }

    // UI 보내는용
    public ItemData[] GetAllEquipData()
    {
        return _equipments;
    }

}