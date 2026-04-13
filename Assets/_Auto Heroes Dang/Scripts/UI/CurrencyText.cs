using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _moveY = 40f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Color _gemColor = new Color(0.7f, 0.3f, 1f);

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    private Color _defaultColor;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_text == null)
            _text = GetComponentInChildren<TMP_Text>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (_text != null)
            _defaultColor = _text.color;
    }

    public void PlayGold(string message)
    {
        if (_text != null)
        {
            _text.text = message;
            _text.color = _defaultColor; // 원래 골드 텍스트 색 유지
        }

        StopAllCoroutines();
        StartCoroutine(Co_Play());
    }

    public void PlayGem(string message)
    {
        if (_text != null)
        {
            _text.text = message;
            _text.color = _gemColor; // 젬은 보라색
        }

        StopAllCoroutines();
        StartCoroutine(Co_Play());
    }

    private IEnumerator Co_Play()
    {
        Vector2 startPos = _rect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0f, _moveY);

        float time = 0f;
        _canvasGroup.alpha = 1f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _duration);

            _rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}