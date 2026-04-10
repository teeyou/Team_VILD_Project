using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPrefab : MonoBehaviour
{
    [SerializeField] private Image _colorImage;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _soldObject;
    [SerializeField] private GameObject _checkObject; // 강화 합성에서 사용할 체크 표시

    private ItemData _itemData;
    private Action<ItemData> _onClick;

    public void Init(ItemData data, Color color, Action<ItemData> onClick)
    {
        _itemData = data;
        _onClick = onClick;

        if (_colorImage != null)
            _colorImage.color = color;

        if (_levelText != null)
            _levelText.text = $"Lv. {data.level}";

        if (_checkObject != null)
            _checkObject.SetActive(false);

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => _onClick?.Invoke(_itemData));
            _button.interactable = true;
            _button.enabled = true;
        }

        if (_soldObject != null)
            _soldObject.SetActive(false);

        if (_checkObject != null)
            _checkObject.SetActive(false);
    }

    public void SoldOut(bool soldOut)
    {
        if (_button != null)
            _button.interactable = !soldOut;

        if (_soldObject != null)
            _soldObject.SetActive(soldOut);
    }

    public void SetButtonInteractable(bool value)
    {
        if (_button != null)
        {
            _button.interactable = value;
            _button.enabled = value;
        }
    }

    public void SetSelected(bool selected)
    {
        if (_checkObject != null)
            _checkObject.SetActive(selected);

        if (_button != null)
            _button.interactable = !selected;
    }
}