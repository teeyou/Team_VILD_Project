using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSfx : MonoBehaviour
{
    [SerializeField] private string _clickSfxName = "UIClick";

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlayClickSfx);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(PlayClickSfx);
    }

    private void PlayClickSfx()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlaySFX(_clickSfxName);
    }
}