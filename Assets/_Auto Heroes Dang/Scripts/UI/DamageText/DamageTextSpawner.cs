using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance { get; private set; }

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

    public void SpawnDamageText(int damage, Vector3 worldPosition, Color color)
    {
        if (_targetCanvas == null)
        {
            Debug.LogWarning("DamageTextSpawner : Canvas가 연결되지 않았습니다.");
            return;
        }

        if (_damageTextPrefab == null)
        {
            Debug.LogWarning("DamageTextSpawner : DamageText 프리팹이 연결되지 않았습니다.");
            return;
        }

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_canvasRectTransform == null)
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();

        if (_mainCamera == null || _canvasRectTransform == null)
            return;

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPos.z <= 0f)
            return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            screenPos,
            _targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera,
            out localPoint
        );

        DamageText damageText = Instantiate(_damageTextPrefab, _canvasRectTransform);
        damageText.Setup(damage, localPoint, color);
    }
}