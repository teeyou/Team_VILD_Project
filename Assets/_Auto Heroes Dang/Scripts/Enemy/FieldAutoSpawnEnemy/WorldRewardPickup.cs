using System.Collections;
using UnityEngine;

public class WorldRewardPickup : MonoBehaviour
{
    [Header("월드 이동")]
    [SerializeField] private float _scatterDuration = 0.25f;
    [SerializeField] private float _scatterHeight = 0.8f;
    [SerializeField] private float _waitAfterScatter = 0.2f;

    [Header("UI 아이콘 프리팹")]
    [SerializeField] private GameObject _goldFlyIconPrefab;
    [SerializeField] private GameObject _gemFlyIconPrefab;

    private RewardType _rewardType;
    private int _amount;
    private Vector3 _startPos;
    private Vector3 _targetWorldPos;

    public void Init(RewardType rewardType, int amount, Vector3 targetWorldPos)
    {
        _rewardType = rewardType;
        _amount = amount;
        _startPos = transform.position;
        _targetWorldPos = targetWorldPos;

        StartCoroutine(Co_PlayRewardFlow());
    }

    private IEnumerator Co_PlayRewardFlow()
    {
        yield return StartCoroutine(Co_Scatter());

        yield return new WaitForSeconds(_waitAfterScatter);

        SpawnFlyIconToUI();

        Destroy(gameObject);
    }

    private IEnumerator Co_Scatter()
    {
        float time = 0f;

        while (time < _scatterDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _scatterDuration);

            Vector3 pos = Vector3.Lerp(_startPos, _targetWorldPos, t);
            pos.y += 4f * _scatterHeight * t * (1f - t);

            transform.position = pos;

            yield return null;
        }

        transform.position = _targetWorldPos;
    }

    private void SpawnFlyIconToUI()
    {
        if (UIManager.Instance == null)
            return;

        RectTransform canvasRoot = UIManager.Instance.GetRewardCanvasRoot();
        if (canvasRoot == null)
            return;

        GameObject iconPrefab = null;
        Vector3 targetUIPos;

        if (_rewardType == RewardType.Gold)
        {
            iconPrefab = _goldFlyIconPrefab;
            targetUIPos = UIManager.Instance.GoldTargetUIPosition;
        }
        else
        {
            iconPrefab = _gemFlyIconPrefab;
            targetUIPos = UIManager.Instance.GemTargetUIPosition;
        }

        if (iconPrefab == null || Camera.main == null)
            return;

        Vector3 startScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        GameObject iconObj = Instantiate(iconPrefab, canvasRoot);
        RewardFlyIcon flyIcon = iconObj.GetComponent<RewardFlyIcon>();

        if (flyIcon != null)
        {
            flyIcon.Init(startScreenPos, targetUIPos, _rewardType, _amount);
        }
    }
}