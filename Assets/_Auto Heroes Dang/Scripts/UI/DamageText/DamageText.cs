using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _moveUpSpeed = 50f;
    [SerializeField] private float _lifeTime = 1f;
    [SerializeField] private Color _normalColor = Color.red;
    [SerializeField] private Color _criticalColor = Color.yellow;

    private RectTransform _rectTransform;
    private float _timer;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(int damage, Vector2 localPosition, bool isCritical)
    {
        _rectTransform.anchoredPosition = localPosition;
        _text.text = damage.ToString();
        _text.color = isCritical ? _criticalColor : _normalColor;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        _rectTransform.anchoredPosition += Vector2.up * _moveUpSpeed * Time.deltaTime;

        if (_canvasGroup != null)
        {
            float t = Mathf.Clamp01(_timer / _lifeTime);
            _canvasGroup.alpha = 1f - t;
        }

        if (_timer >= _lifeTime)
            Destroy(gameObject);
    }
}