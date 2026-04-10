using UnityEngine;

public class BlacksmithTabUI : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject _enhancementPanel;
    [SerializeField] private GameObject _combinationPanel;

    [Header("UI 스크립트")]
    [SerializeField] private ItemEnhancementUI _enhancementUI;
    [SerializeField] private ItemCombinationUI _combinationUI;
    [SerializeField] private ItemInventoryUI _inventoryUI;

    private void Start()
    {
        OpenEnhancement();
    }

    public void OpenEnhancement()
    {
        // 합성창에 남아있던 아이템 제거
        if (_combinationUI != null)
            _combinationUI.ClearAll();

        if (_combinationPanel != null)
            _combinationPanel.SetActive(false);

        if (_enhancementPanel != null)
            _enhancementPanel.SetActive(true);

        if (_inventoryUI != null)
            _inventoryUI.Refresh();
    }

    public void OpenCombination()
    {
        // 강화창에 남아있던 아이템 제거
        if (_enhancementUI != null)
            _enhancementUI.ClearSlot();

        if (_enhancementPanel != null)
            _enhancementPanel.SetActive(false);

        if (_combinationPanel != null)
            _combinationPanel.SetActive(true);

        if (_inventoryUI != null)
            _inventoryUI.Refresh();
    }
}