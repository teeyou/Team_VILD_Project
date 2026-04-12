using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManagerPotion : Singleton<ShopManagerPotion>, IItemManage
{
    private List<ItemData> _items = new List<ItemData>();

    [SerializeField] private int _atkBuffAmount = 10; 
    [SerializeField] private int _defBuffAmount = 10; 

    public event Action OnChanged;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 테스트용
        AddItem(new ItemData("고기", ItemType.AtkBuff, Grade.AtkBuff, 5, 11, 0, "힘이 세지는 고기"));
        AddItem(new ItemData("풀", ItemType.DefBuff, Grade.DefBuff, 1, 11, 999, "방어가 강해지는 풀"));
    }


    public IReadOnlyList<ItemData> GetItems() => _items;


    public void AddItem(ItemData item)
    {
        _items.Add(item);
        OnChanged?.Invoke();
    }

    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].name == item.name)
            {
                ItemData temp = _items[i];
                temp.state = ItemState.SoldOut;
                _items[i] = temp;

                if (item.type == ItemType.AtkBuff)
                {
                    DataSource.Instance.IncreaseAtk(_atkBuffAmount, true);
                }
                else if (item.type == ItemType.DefBuff)
                {
                    DataSource.Instance.IncreaseDef(_defBuffAmount, true);
                }

                OnChanged?.Invoke();
                return;
            }
        }
    }

}