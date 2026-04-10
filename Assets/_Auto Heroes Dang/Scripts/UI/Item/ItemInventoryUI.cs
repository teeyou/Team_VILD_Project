using UnityEngine;

public class ItemInventoryUI : MonoBehaviour
{
    [SerializeField] private ItemSlotFactory _factory;
    [SerializeField] private int _verticalSlot = 5;

    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _enhancementPanel;
    [SerializeField] private GameObject _CombinationPanel;

    [SerializeField] private ItemEnhancementUI _enhancementUI;
    [SerializeField] private ItemCombinationUI _CombinationUI;

    private void OnEnable()
    {
        InventoryManager.Instance.OnChanged += Refresh;

        if (_enhancementUI != null)
            _enhancementUI.OnSelectionChanged += Refresh;

        if (_CombinationUI != null)
            _CombinationUI.OnSelectionChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnChanged -= Refresh;

        if (_enhancementUI != null)
            _enhancementUI.OnSelectionChanged -= Refresh;

        if (_CombinationUI != null)
            _CombinationUI.OnSelectionChanged -= Refresh;
    }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        _factory.RefreshUI(InventoryManager.Instance, _verticalSlot, OnItemClicked, IsSelectedItem);
    }

    // 현재 강화 / 합성 슬롯에 들어간 아이템인지 확인
    private bool IsSelectedItem(ItemData item)
    {
        if (_enhancementPanel != null && _enhancementPanel.activeSelf)
        {
            return _enhancementUI != null && _enhancementUI.HasSelectedItem(item);
        }

        if (_CombinationPanel != null && _CombinationPanel.activeSelf)
        {
            return _CombinationUI != null && _CombinationUI.HasSelectedItem(item);
        }

        return false;
    }

    private void OnItemClicked(ItemData item)
    {
        Debug.Log($"enhancement active: {_enhancementPanel != null && _enhancementPanel.activeInHierarchy}");
        Debug.Log($"fusion active: {_CombinationPanel != null && _CombinationPanel.activeInHierarchy}");

        if (_shopPanel != null && _shopPanel.activeInHierarchy)
        {
            return;
        }
        else if (_inventoryPanel != null && _inventoryPanel.activeInHierarchy)
        {
            return;
        }
        else if (_enhancementPanel != null && _enhancementPanel.activeSelf)
        {
            Debug.Log("강화 패널로 아이템 전달");
            _enhancementUI.SetItem(item);
            Refresh();
        }
        else if (_CombinationPanel != null && _CombinationPanel.activeSelf)
        {
            Debug.Log("합성 패널로 아이템 전달");
            _CombinationUI.TryAddItem(item);
            Refresh();
        }
    }
}