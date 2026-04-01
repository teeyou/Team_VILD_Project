using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [Header("Canvas")]
    [SerializeField] private Canvas _targetCanvas;

    [Header("데미지 텍스트 프리팹")]
    [SerializeField] private DamageText _damageTextPrefab;

    private Camera _mainCamera;
    private RectTransform _canvasRectTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _mainCamera = Camera.main;

        if (_targetCanvas != null)
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();
    }

    public void ShowDamage(Unit targetUnit, int damage, Transform attacker = null, bool isCritical = false)
    {
        if (targetUnit == null)
            return;

        if (_targetCanvas == null || _damageTextPrefab == null)
            return;

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_canvasRectTransform == null)
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();

        Vector3 worldPos = GetDamageTextWorldPosition(targetUnit, attacker);

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
        if (screenPos.z <= 0f)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            screenPos,
            _targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera,
            out Vector2 localPoint
        );

        DamageText damageText = Instantiate(_damageTextPrefab, _canvasRectTransform);
        damageText.Setup(damage, localPoint, isCritical);
    }

    private Vector3 GetDamageTextWorldPosition(Unit targetUnit, Transform attacker)
    {
        Vector3 basePos;

        if (targetUnit.DamageTextPoint != null)
            basePos = targetUnit.DamageTextPoint.position;
        else
            basePos = targetUnit.transform.position + Vector3.up * targetUnit.DamageTextHeightOffset;

        return basePos;
    }
}