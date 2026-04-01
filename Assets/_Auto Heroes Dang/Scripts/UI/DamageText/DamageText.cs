using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _moveUpSpeed = 50f;
    [SerializeField] private float _lifeTime = 1f;

    [Header("기본 색상(플레이어 공격용)")]
    [SerializeField] private Color _normalColor = Color.red;
    [SerializeField] private Color _criticalColor = Color.yellow;

    [Header("몬스터 공격 색상")]
    [SerializeField] private Color _enemyNormalColor = new Color(0.6f, 0.2f, 1f);
    [SerializeField] private Color _enemyCriticalColor = Color.magenta;

    [Header("랜덤 위치")]
    [SerializeField] private float _randomOffsetX = 30f;

    private RectTransform _rectTransform;
    private float _timer;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // 기본 데미지 텍스트 세팅
    // 플레이어 공격 텍스트는 기존 색상(빨강 / 노랑)을 그대로 사용
    public void Setup(int damage, Vector2 localPosition, bool isCritical)
    {
        float randomX = Random.Range(-_randomOffsetX, _randomOffsetX);

        _rectTransform.anchoredPosition = localPosition + new Vector2(randomX, 0f);
        _text.text = damage.ToString();
        _text.color = isCritical ? _criticalColor : _normalColor;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;
    }

    // 몬스터 공격 데미지 텍스트 세팅
    // 몬스터 공격일 때만 보라 / 마젠타 색상을 사용
    public void SetupEnemyDamage(int damage, Vector2 localPosition, bool isCritical)
    {
        float randomX = Random.Range(-_randomOffsetX, _randomOffsetX);

        _rectTransform.anchoredPosition = localPosition + new Vector2(randomX, 0f);
        _text.text = damage.ToString();
        _text.color = isCritical ? _enemyCriticalColor : _enemyNormalColor;

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