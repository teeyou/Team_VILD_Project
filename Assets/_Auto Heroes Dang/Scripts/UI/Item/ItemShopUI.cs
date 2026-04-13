using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUI : MonoBehaviour
{
    [SerializeField] private ItemSlotFactory _factory;
    [SerializeField] private RectTransform _contentRect;
    private int _verticalSlot = 3; // 세로 라인 수

    private IItemManage _currentTab;

    // 팝업에 넣을 내용
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _iconTransform;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _money;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _cancelButton;

    private ItemData _selectedItem;


    // 픽스된 값(이라 하드코딩)
    private LayoutData _equipLayout = new LayoutData(new Vector2(-474.02f, -113.44f), new Vector2(735.96f, 739.14f));
    private LayoutData _potionLayout = new LayoutData(new Vector2(-474.02f, 5.7225f), new Vector2(735.96f, 500.82f));
    private LayoutData _popupIconPosition = new LayoutData(new Vector2(-180f, 15f), new Vector2(207f, 207f));

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
        if(_panel.activeSelf) _panel.SetActive(false);

        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(ClosePanel);

        SetEquipShop();
    }

    private void OnEnable()
    {
        if (_currentTab != null)
            _currentTab.OnChanged += Refresh;

        Refresh();
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
        {
            _currentTab.OnChanged -= Refresh;
        }

        _currentTab = newProvider;

        _currentTab.OnChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        if (_currentTab == null) return;

        _factory.RefreshUI(_currentTab, _verticalSlot, OnItemClicked, null);
    }

    private void OnItemClicked(ItemData item)
    {
        _selectedItem = item;

        _panel.SetActive(true);

        _nameText.text = item.name;
        _descriptionText.text = item.FullDescription;
        _money.text = item.price.ToString("N0");

        _buyButton.interactable = (item.state != ItemState.SoldOut && DataSource.Instance.Gold >= item.price); 
        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(OnBuyClicked);

        foreach (Transform child in _iconTransform.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject icon = _factory.CreateIcon(item.type, _iconTransform.transform);

        if (icon != null)
        {
            RectTransform rt = icon.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = _popupIconPosition.pos;
                rt.sizeDelta = _popupIconPosition.size;
            }

            ItemPrefab slot = icon.GetComponent<ItemPrefab>();
            if (slot != null)
            {
                Color gradeColor = _factory.GetGradeColor(item.grade);

                slot.Init(item, gradeColor, null);
                slot.SetButtonInteractable(false);
            }
        }

    }

    private void ClosePanel()
    {
        _panel.SetActive(false);
        _selectedItem = default;
    }

    private void OnBuyClicked()
    {
        ItemData item = _selectedItem;
        DataSource.Instance.UseGold(item.price);

        if (item.type != ItemType.AtkBuff && item.type != ItemType.DefBuff)
        {
            InventoryManager.Instance.AddItem(item);
        }


        _currentTab.RemoveItem(item);

        ClosePanel();
    }

}
