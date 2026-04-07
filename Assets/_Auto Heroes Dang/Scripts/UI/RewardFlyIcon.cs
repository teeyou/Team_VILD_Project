using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum RewardType
{
    Gold,
    Gem
}

public class RewardFlyIcon : MonoBehaviour
{
    [SerializeField] private float _moveDuration = 0.6f;

    private RectTransform _rectTransform;
    private Vector3 _startPos;
    private Vector3 _targetPos;

    private RewardType _rewardType;
    private int _amount;

    public void Init(Vector3 startPos, Vector3 targetPos, RewardType rewardType, int amount)
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPos = startPos;
        _targetPos = targetPos;
        _rewardType = rewardType;
        _amount = amount;

        _rectTransform.position = _startPos;

        StartCoroutine(Co_MoveToTarget());
    }

    private IEnumerator Co_MoveToTarget()
    {
        float time = 0f;

        while (time < _moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _moveDuration);

            _rectTransform.position = Vector3.Lerp(_startPos, _targetPos, t);

            yield return null;
        }

        _rectTransform.position = _targetPos;

        ApplyReward();

        Destroy(gameObject);
    }

    private void ApplyReward()
    {
        if (DataSource.Instance == null)
            return;

        if (_rewardType == RewardType.Gold)
        {
            DataSource.Instance.AddGold(_amount);
        }
        else if (_rewardType == RewardType.Gem)
        {
            DataSource.Instance.AddGem(_amount);
        }

        if (UIManager.Instance != null)
            UIManager.Instance.RefreshCurrencyUI();
    }
}