using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
    [SerializeField] private ItemSlotFactory _factory;
    [SerializeField] private int _verticalSlot = 5;

    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _enhancementPanel;

    private void OnEnable()
    {
        InventoryManager.Instance.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        _factory.RefreshUI(InventoryManager.Instance, _verticalSlot, OnItemClicked);
    }

    private void OnItemClicked(ItemData item)
    {
        Debug.Log($"인벤토리 클릭: {item.name}");
        // if else로 상점이면 아이템 정보 표기, 인벤토리면 장착
        if (_shopPanel.activeSelf)
        {
        }
        else if (_inventoryPanel.activeSelf)
        {
        }
        else if (_enhancementPanel.activeSelf)
        {
        }


    }
}
