using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemEnhancementUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Transform _slotParent;
    [SerializeField] private TMP_Text _percentText;
    [SerializeField] private TMP_Text _gemCostText;
    [SerializeField] private TMP_Text _goldCostText;
    [SerializeField] private Button _okButton;

    [Header("Factory")]
    [SerializeField] private ItemSlotFactory _factory;

    private ItemData? _selectedItem;
    private GameObject _currentIcon;

    private void Start()
    {
        _okButton.onClick.AddListener(EnhanceSelectedItem);
        ClearSlot();
    }

    public bool HasItem()
    {
        return _selectedItem.HasValue;
    }

    public void SetItem(ItemData item)
    {
        Debug.Log($"SetItem 호출됨: {item.name}");

        if (!ItemForgeHelper.IsEquip(item.type))
        {
            Debug.Log("장비만 강화 가능");
            return;
        }

        _selectedItem = item;
        RefreshUI();
    }

    public void ClearSlot()
    {
        _selectedItem = null;

        if (_currentIcon != null)
            Destroy(_currentIcon);

        _percentText.text = "- %";
        _gemCostText.text = "0";
        _goldCostText.text = "0";
    }

    private void RefreshUI()
    {
        if (!_selectedItem.HasValue)
        {
            ClearSlot();
            return;
        }

        ItemData item = _selectedItem.Value;

        if (_currentIcon != null)
            Destroy(_currentIcon);

        _currentIcon = _factory.CreateIcon(item.type, _slotParent);

        if (_currentIcon != null)
        {
            ItemPrefab icon = _currentIcon.GetComponent<ItemPrefab>();
            if (icon != null)
            {
                icon.Init(item, _factory.GetGradeColor(item.grade), OnSelectedSlotClicked);
                icon.SetButtonInteractable(true);
            }
        }

        int successPercent = ItemForgeHelper.GetEnhanceSuccessPercent(item.level);
        int cost = ItemForgeHelper.GetEnhanceCost(item);

        _percentText.text = $"{successPercent} %";
        _goldCostText.text = cost.ToString("N0");
        _gemCostText.text = "0";
    }

    private void OnSelectedSlotClicked(ItemData item)
    {
        ClearSlot();
    }

    private void EnhanceSelectedItem()
    {
        if (!_selectedItem.HasValue)
            return;

        ItemData item = _selectedItem.Value;
        int cost = ItemForgeHelper.GetEnhanceCost(item);
        int successPercent = ItemForgeHelper.GetEnhanceSuccessPercent(item.level);

        if (DataSource.Instance.Gold < cost)
        {
            Debug.Log("골드 부족");
            return;
        }

        DataSource.Instance.UseGold(cost);

        bool isSuccess = Random.Range(0, 100) < successPercent;

        if (isSuccess)
        {
            ItemData enhanced = item;
            enhanced.level += 1;
            enhanced.value = Mathf.CeilToInt(enhanced.value * 1.2f);

            InventoryManager.Instance.ReplaceItem(item, enhanced);
            _selectedItem = enhanced;

            Debug.Log("강화 성공");
        }
        else
        {
            Debug.Log("강화 실패");
        }

        RefreshUI();
    }
}