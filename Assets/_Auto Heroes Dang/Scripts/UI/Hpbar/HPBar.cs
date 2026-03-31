using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _fillImage;

    [Header("추적 대상")]
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 2f, 0f);

    private Camera _mainCamera;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _mainCamera = Camera.main;
    }

    public void Initialize(Transform target, Vector3 worldOffset)
    {
        _target = target;
        _worldOffset = worldOffset;

        if (_mainCamera == null)
            _mainCamera = Camera.main;
    }

    public void SetHp(int currentHp, int maxHp)
    {
        if (_fillImage == null)
            return;

        if (maxHp <= 0)
        {
            _fillImage.fillAmount = 0f;
            return;
        }

        _fillImage.fillAmount = (float)currentHp / maxHp;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_mainCamera == null)
            return;

        Vector3 worldPos = _target.position + _worldOffset;
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        _rectTransform.position = screenPos;
    }
}