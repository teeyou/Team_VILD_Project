using System;
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

    // 인벤토리 UI에게 선택 상태 갱신을 알려주기 위한 이벤트
    public event Action OnSelectionChanged;

    private void Start()
    {
        _okButton.onClick.AddListener(EnhanceSelectedItem);
        ClearSlot();
    }

    private void OnDisable() // 0413 타 패널에 선택 남아있어 추가했습니다.
    {
        ClearSlot();
    }


    public bool HasItem()
    {
        return _selectedItem.HasValue;
    }

    // 현재 강화 슬롯에 들어가 있는 아이템인지 체크
    public bool HasSelectedItem(ItemData item)
    {
        return _selectedItem.HasValue && _selectedItem.Value.uniqueId == item.uniqueId;
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
        OnSelectionChanged?.Invoke();
    }

    public void ClearSlot()
    {
        _selectedItem = null;

        if (_currentIcon != null)
            Destroy(_currentIcon);

        _percentText.text = "- %";
        _gemCostText.text = "0";
        _goldCostText.text = "0";

        OnSelectionChanged?.Invoke();
    }

    private void RefreshUI()
    {
        if (!_selectedItem.HasValue)
        {
            if (_currentIcon != null)
                Destroy(_currentIcon);

            _percentText.text = "- %";
            _gemCostText.text = "0";
            _goldCostText.text = "0";
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
                // 강화 슬롯에 들어간 아이템은 다시 누르면 빠지도록 처리
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

        bool isSuccess = UnityEngine.Random.Range(0, 100) < successPercent;

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
        OnSelectionChanged?.Invoke();
    }
}