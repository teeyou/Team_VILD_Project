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
        /*
        // 템복사 방지를 위해 DataSource의 MakeInventoryData()의 널부분으로 변경. 이곳은 테스트 용도로만 활용.
        
        AddItem(new ItemData("흔한 모자", ItemType.Hat, Grade.Common, 1, 10, 100, "평범한 모자 \nDef +10"));
        AddItem(new ItemData("흔한 검", ItemType.Sword, Grade.Common, 1, 10, 200, "흔한 검 \nAtk +10"));
        AddItem(new ItemData("흔한 갑주", ItemType.Armor, Grade.Common, 1, 10, 200, "흔한 갑주 \nDef +10"));
        AddItem(new ItemData("흔한 반지", ItemType.Ring, Grade.Common, 1, 10, 200, "흔한 반지 \nAtk +10"));
        AddItem(new ItemData("흔한 신발", ItemType.Shoes, Grade.Common, 1, 10, 200, "흔한 신발 \nDef +10"));

        */

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
        for (int i = 0; i < _equipments.Length; i++)
        {
            _equipments[i] = default;
        }
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

    // 저장용
    public ItemData[] GetEquipmentData()
    {
        ItemData[] copy = new ItemData[_equipments.Length];
        Array.Copy(_equipments, copy, _equipments.Length);
        return copy;
    }

    public void SetEquipments(ItemData[] equipments)
    {
        if (equipments == null)
        {
            Debug.LogWarning("장비 null");
            return;
        }

        _equipments = new ItemData[equipments.Length];
        Array.Copy(equipments, _equipments, equipments.Length);

        OnChanged?.Invoke();
    }

}