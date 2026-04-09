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
        Debug.Log($"enhancement active: {_enhancementPanel != null && _enhancementPanel.activeInHierarchy}");
        Debug.Log($"fusion active: {_CombinationPanel != null && _CombinationPanel.activeInHierarchy}");

        if (_shopPanel != null && _shopPanel.activeInHierarchy)
        {
        }
        else if (_inventoryPanel != null && _inventoryPanel.activeInHierarchy)
        {
        }
        else if (_enhancementPanel != null && _enhancementPanel.activeInHierarchy)
        {
            Debug.Log("강화 패널로 아이템 전달");
            _enhancementUI.SetItem(item);
        }
        else if (_CombinationUI != null)
        {
            Debug.Log("합성 패널로 아이템 전달");
            _CombinationUI.TryAddItem(item);
        }
    }
}