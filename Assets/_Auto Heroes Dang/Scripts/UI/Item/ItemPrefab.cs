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

    [SerializeField] private GameObject _soldObject;


    public void Init(ItemData data, Color color, Action<ItemData> onClick)
    {
        _itemData = data;
        _onClick = onClick;

        _colorImage.color = color;
        _levelText.text = $"Lv. {data.level}";

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _onClick?.Invoke(_itemData));
    }

    public void SoldOut(bool soldOut)
    {
        _button.interactable = !soldOut;
        if (_soldObject != null) _soldObject.SetActive(soldOut);
    }

    public void SetButtonInteractable(bool v)
    {
        if (_button != null)
        {
            _button.interactable = v;
            _button.enabled = v;
        }
    }

}