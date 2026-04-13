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
                    if (DataSource.Instance.atkPotionOn) return;

                    DataSource.Instance.atkPotionOn = true;
                    DataSource.Instance.IncreaseAtk(_atkBuffAmount, true);
                }
                else if (item.type == ItemType.DefBuff)
                {
                    if (DataSource.Instance.defPotionOn) return;

                    DataSource.Instance.defPotionOn = true;
                    DataSource.Instance.IncreaseDef(_defBuffAmount, true);
                }

                DataSource.Instance.Save(); // 변경내용 저장

                OnChanged?.Invoke();
                return;
            }
        }
    }

    public void InitializePotion()
    {
        _items.Clear();

        ItemData atkItem = new ItemData("고기", ItemType.AtkBuff, Grade.AtkBuff, 5, 11, 0, "힘이 세지는 고기");
        ItemData defItem = new ItemData("풀", ItemType.DefBuff, Grade.DefBuff, 1, 11, 999, "방어가 강해지는 풀");

        if (DataSource.Instance.atkPotionOn)
            atkItem.state = ItemState.SoldOut;

        if (DataSource.Instance.defPotionOn)
            defItem.state = ItemState.SoldOut;

        AddItem(atkItem);
        AddItem(defItem);
    }

}