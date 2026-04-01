using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private Canvas _targetCanvas;
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

    // 피격당한 유닛 위치에 데미지 텍스트를 생성하는 함수
    // 플레이어 공격이면 기존 색상 사용
    // 몬스터 공격이면 몬스터 전용 색상 사용
    public void ShowDamage(Unit targetUnit, int damage, Transform attacker, bool isCritical)
    {
        if (targetUnit == null)
            return;

        if (_targetCanvas == null || _damageTextPrefab == null)
            return;

        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (_canvasRectTransform == null)
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();

        Vector3 worldPos = GetDamageTextWorldPosition(targetUnit);
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

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (attacker != null && attacker.gameObject.layer == enemyLayer)
        {
            damageText.SetupEnemyDamage(damage, localPoint, isCritical);
        }
        else
        {
            damageText.Setup(damage, localPoint, isCritical);
        }
    }

    private Vector3 GetDamageTextWorldPosition(Unit targetUnit)
    {
        if (targetUnit.DamageTextPoint != null)
            return targetUnit.DamageTextPoint.position;

        return targetUnit.transform.position + Vector3.up * targetUnit.DamageTextHeightOffset;
    }
}