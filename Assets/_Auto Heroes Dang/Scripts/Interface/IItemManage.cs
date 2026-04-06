using System;
using System.Collections.Generic;

public interface IItemManage
{
    IReadOnlyList<ItemData> GetItems();
    event Action OnChanged;

    void AddItem(ItemData item);
    void RemoveItem(ItemData item);
}