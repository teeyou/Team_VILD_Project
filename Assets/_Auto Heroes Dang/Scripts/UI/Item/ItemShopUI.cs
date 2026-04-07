using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopUI : MonoBehaviour
{
    [SerializeField] private ItemSlotFactory _factory;
    [SerializeField] private RectTransform _contentRect;
    private int _verticalSlot = 3; // 세로 라인 수

    private IItemManage _currentTab;

    // 픽스된 값(이라 하드코딩)
    private LayoutData _equipLayout = new LayoutData(new Vector2(-474.02f, -113.44f), new Vector2(735.96f, 739.14f));
    private LayoutData _potionLayout = new LayoutData(new Vector2(-474.02f, 5.7225f), new Vector2(735.96f, 500.82f));

    private struct LayoutData
    {
        public Vector2 pos;
        public Vector2 size;

        public LayoutData(Vector2 pos, Vector2 size)
        {
            this.pos = pos;
            this.size = size;
        }
    }

    private void Start()
    {
        SetEquipShop();
    }

    private void OnDisable()
    {
        if (_currentTab != null)
            _currentTab.OnChanged -= Refresh;
    }

    public void SetEquipShop()
    {
        _verticalSlot = 3;
        ApplyLayout(_equipLayout);
        ChangeTab(ShopManagerEquip.Instance);
    }

    public void SetPotionShop()
    {
        _verticalSlot = 2;
        ApplyLayout(_potionLayout);
        ChangeTab(ShopManagerPotion.Instance);
    }

    private void ApplyLayout(LayoutData data)
    {
        _contentRect.anchoredPosition = data.pos;
        _contentRect.sizeDelta = data.size;
    }

    private void ChangeTab(IItemManage newProvider)
    {
        if (_currentTab != null)
            _currentTab.OnChanged -= Refresh;

        _currentTab = newProvider;

        _currentTab.OnChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        if (_currentTab == null) return;

        _factory.RefreshUI(_currentTab, _verticalSlot, OnItemClicked);
    }

    private void OnItemClicked(ItemData item)
    {
        Debug.Log($"[상점 구매] {item.name}");

        InventoryManager.Instance.AddItem(item);
        _currentTab.RemoveItem(item);
    }
}
