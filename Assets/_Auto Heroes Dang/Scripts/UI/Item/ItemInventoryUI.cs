using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameObject _itemInfoPanel;
    [SerializeField] private GameObject _iconTransform;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Button _equipButton;
    [SerializeField] private TMP_Text _equipButtonText;

    [SerializeField] private Transform[] _equipSlot; // 인덱스 = 아이템데이터 이넘 순서
    private ItemData _currentSelected;

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
        _verticalSlot = _inventoryPanel.activeSelf ? 5 : 4;
        _factory.RefreshUI(InventoryManager.Instance, _verticalSlot, OnItemClicked, IsSelectedItem);
        if(_inventoryPanel != null && _inventoryPanel.activeInHierarchy) RefreshEquipSlots();
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
            ItemInfoSet(item, false); // 아이콘 클릭 시 정보 표기 (버튼 끔)
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
        else
        {
            ItemInfoSet(item, true);
            return;
        }


    }

    // 상점 / 인벤토리에서 버튼 클릭 시 패널에 정보를 띄움.
    private void ItemInfoSet(ItemData item, bool needButtonSet) 
    {
        _currentSelected = item;

        _itemInfoPanel.SetActive(true);
        _equipButton.gameObject.SetActive(needButtonSet);

        _nameText.text = item.name;
        _descriptionText.text = item.FullDescription;

        foreach (Transform child in _iconTransform.transform)
        {
            Destroy(child.gameObject);
        }

        GameObject icon = _factory.CreateIcon(item.type, _iconTransform.transform);

        if (icon != null)
        {
            ItemPrefab slot = icon.GetComponent<ItemPrefab>();
            if (slot != null)
            {
                Color gradeColor = _factory.GetGradeColor(item.grade);

                slot.Init(item, gradeColor, null);
                slot.SetButtonInteractable(false);
            }
        }

        SetupEquipButton(item);

    }

    private void SetupEquipButton(ItemData item)
    {
        _equipButton.onClick.RemoveAllListeners();

        if (item.type > ItemType.Ring)
        {
            _equipButton.gameObject.SetActive(false);
            return;
        }

        bool isEquipped = InventoryManager.Instance.IsEquipped(item);

        if (_equipButtonText != null)
        {
            _equipButtonText.text = isEquipped ? "해제" : "장착";
        }

        _equipButton.onClick.AddListener(() =>
        {
            if (InventoryManager.Instance.IsEquipped(item))
            {
                InventoryManager.Instance.UnequipItem(item);
            }
            else
            {
                InventoryManager.Instance.EquipItem(item);
            }

            Refresh();

            _itemInfoPanel.SetActive(false);
        });
    }

    // 장착 슬롯 UI 갱신
    private void RefreshEquipSlots()
    {
        ItemData[] equips = InventoryManager.Instance.GetAllEquipData();

        for (int i = 0; i < _equipSlot.Length; i++)
        {
            foreach (Transform child in _equipSlot[i])
            {
                Destroy(child.gameObject);
            }

            if (equips[i].uniqueId == 0)
                continue;

            GameObject icon = _factory.CreateIcon(equips[i].type, _equipSlot[i]);

            if (icon != null)
            {
                ItemPrefab slot = icon.GetComponent<ItemPrefab>();
                if (slot != null)
                {
                    Color gradeColor = _factory.GetGradeColor(equips[i].grade);
                    slot.Init(equips[i], gradeColor, OnItemClicked);
                }
            }
        }
    }


}