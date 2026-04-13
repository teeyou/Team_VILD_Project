using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCombinationUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform _leftSlotParent;
    [SerializeField] private Transform _rightSlotParent;
    [SerializeField] private Transform _resultSlotParent;
    [SerializeField] private TMP_Text _percentText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Button _okButton;

    [Header("Factory")]
    [SerializeField] private ItemSlotFactory _factory;

    private ItemData? _leftItem;
    private ItemData? _rightItem;

    private GameObject _leftIcon;
    private GameObject _rightIcon;
    private GameObject _resultIcon;

    // 인벤토리 UI에게 선택 상태 갱신을 알려주기 위한 이벤트
    public event Action OnSelectionChanged;

    private void Start()
    {
        _okButton.onClick.AddListener(FuseItems);
        RefreshUI();
    }

    private void OnDisable() // 0413 타 패널에 선택 남아있어 추가했습니다.
    {
        ClearAll();
    }

    // 현재 합성 슬롯에 들어가 있는 아이템인지 체크
    public bool HasSelectedItem(ItemData item)
    {
        return (_leftItem.HasValue && _leftItem.Value.uniqueId == item.uniqueId) ||
               (_rightItem.HasValue && _rightItem.Value.uniqueId == item.uniqueId);
    }

    public void TryAddItem(ItemData item)
    {
        if (!ItemForgeHelper.IsEquip(item.type))
        {
            Debug.Log("장비만 합성 가능");
            return;
        }

        if (!_leftItem.HasValue)
        {
            _leftItem = item;
            RefreshUI();
            OnSelectionChanged?.Invoke();
            return;
        }

        if (!_rightItem.HasValue)
        {
            ItemData left = _leftItem.Value;

            if (!ItemForgeHelper.CanFuse(left, item))
            {
                Debug.Log("같은 타입, 같은 등급만 합성 가능");
                return;
            }

            _rightItem = item;
            RefreshUI();
            OnSelectionChanged?.Invoke();
            return;
        }

        Debug.Log("합성 슬롯이 가득 참");
    }

    public void ClearAll()
    {
        _leftItem = null;
        _rightItem = null;
        RefreshUI();
        OnSelectionChanged?.Invoke();
    }

    private void RefreshUI()
    {
        ClearIcons();

        if (_leftItem.HasValue)
        {
            ItemData item = _leftItem.Value;
            _leftIcon = _factory.CreateIcon(item.type, _leftSlotParent);

            if (_leftIcon != null)
            {
                ItemPrefab icon = _leftIcon.GetComponent<ItemPrefab>();
                if (icon != null)
                {
                    // 왼쪽 슬롯에 들어간 아이템은 다시 누르면 빠지도록 처리
                    icon.Init(item, _factory.GetGradeColor(item.grade), OnLeftSlotClicked);
                    icon.SetButtonInteractable(true);
                }
            }
        }

        if (_rightItem.HasValue)
        {
            ItemData item = _rightItem.Value;
            _rightIcon = _factory.CreateIcon(item.type, _rightSlotParent);

            if (_rightIcon != null)
            {
                ItemPrefab icon = _rightIcon.GetComponent<ItemPrefab>();
                if (icon != null)
                {
                    // 오른쪽 슬롯에 들어간 아이템은 다시 누르면 빠지도록 처리
                    icon.Init(item, _factory.GetGradeColor(item.grade), OnRightSlotClicked);
                    icon.SetButtonInteractable(true);
                }
            }
        }

        if (_leftItem.HasValue && _rightItem.HasValue)
        {
            ItemData left = _leftItem.Value;
            ItemData right = _rightItem.Value;

            if (ItemForgeHelper.CanFuse(left, right))
            {
                int percent = ItemForgeHelper.GetFusionSuccessPercent(left.grade);
                int gemCost = ItemForgeHelper.GetFusionGemCost(left.grade);

                _percentText.text = $"{percent} %";
                _costText.text = gemCost.ToString("N0");

                ItemData resultItem = GetFusionPreviewItem(left);

                _resultIcon = _factory.CreateIcon(resultItem.type, _resultSlotParent);
                if (_resultIcon != null)
                {
                    ItemPrefab icon = _resultIcon.GetComponent<ItemPrefab>();
                    if (icon != null)
                    {
                        icon.Init(resultItem, _factory.GetGradeColor(resultItem.grade), null);
                        icon.SetButtonInteractable(false);
                    }
                }
            }
            else
            {
                _percentText.text = "- %";
                _costText.text = "0";
            }
        }
        else
        {
            _percentText.text = "- %";
            _costText.text = "0";
        }
    }

    private void OnLeftSlotClicked(ItemData item)
    {
        _leftItem = null;
        RefreshUI();
        OnSelectionChanged?.Invoke();
    }

    private void OnRightSlotClicked(ItemData item)
    {
        _rightItem = null;
        RefreshUI();
        OnSelectionChanged?.Invoke();
    }

    private ItemData GetFusionPreviewItem(ItemData baseItem)
    {
        ItemData result = baseItem;
        result.uniqueId = ItemIdGenerator.GetNextId();   // 새 아이템이므로 새 ID
        result.grade = ItemForgeHelper.GetNextGrade(baseItem.grade);
        result.level = 1;
        //result.value = Mathf.CeilToInt(baseItem.value * 1.8f);
        result.value = Mathf.CeilToInt(baseItem.value + 50);
        result.name = $"{result.grade} {baseItem.type}";
        result.price = Mathf.CeilToInt(baseItem.price * 1.5f);

        return result;
    }

    private void ClearIcons()
    {
        if (_leftIcon != null) Destroy(_leftIcon);
        if (_rightIcon != null) Destroy(_rightIcon);
        if (_resultIcon != null) Destroy(_resultIcon);
    }

    private void FuseItems()
    {
        if (!_leftItem.HasValue || !_rightItem.HasValue)
            return;

        ItemData left = _leftItem.Value;
        ItemData right = _rightItem.Value;

        if (!ItemForgeHelper.CanFuse(left, right))
        {
            Debug.Log("합성 조건 불만족");
            return;
        }

        int successPercent = ItemForgeHelper.GetFusionSuccessPercent(left.grade);
        int gemCost = ItemForgeHelper.GetFusionGemCost(left.grade);

        if (DataSource.Instance.Gem < gemCost)
        {
            Debug.Log("젬 부족");
            return;
        }

        DataSource.Instance.UseGem(gemCost);

        InventoryManager.Instance.RemoveItem(left);
        InventoryManager.Instance.RemoveItem(right);

        bool isSuccess = UnityEngine.Random.Range(0, 100) < successPercent;

        if (isSuccess)
        {
            ItemData result = GetFusionPreviewItem(left);
            InventoryManager.Instance.AddItem(result);
            Debug.Log("합성 성공");
        }
        else
        {
            Debug.Log("합성 실패");
        }

        ClearAll();
    }
}