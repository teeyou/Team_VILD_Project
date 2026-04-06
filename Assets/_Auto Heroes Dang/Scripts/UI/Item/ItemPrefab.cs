using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPrefab : MonoBehaviour
{
    [SerializeField] private Image _colorImage;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Button _button;

    private ItemData _itemData;
    private Action<ItemData> _onClick;

    public void Init(ItemData data, Color color, Action<ItemData> onClick)
    {
        _itemData = data;
        _onClick = onClick;

        _colorImage.color = color;
        _levelText.text = $"Lv. {data.level}";

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _onClick?.Invoke(_itemData));
    }
}