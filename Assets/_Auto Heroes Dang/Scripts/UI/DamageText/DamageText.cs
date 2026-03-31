using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("움직임")]
    [SerializeField] private float _moveUpSpeed = 50f;
    [SerializeField] private float _lifeTime = 1f;

    [Header("페이드")]
    [SerializeField] private bool _useFade = true;

    private RectTransform _rectTransform;
    private float _timer;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(int damage, Vector2 localPosition, Color color)
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();

        if (_text != null)
        {
            _text.text = damage.ToString();
            _text.color = color;
        }

        _rectTransform.anchoredPosition = localPosition;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition += Vector2.up * _moveUpSpeed * Time.deltaTime;
        }

        if (_useFade && _canvasGroup != null)
        {
            float t = Mathf.Clamp01(_timer / _lifeTime);
            _canvasGroup.alpha = 1f - t;
        }

        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }
}