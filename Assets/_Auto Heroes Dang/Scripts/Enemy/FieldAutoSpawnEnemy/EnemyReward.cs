using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [Header("보상 값")]
    [SerializeField] private int _goldReward = 10;
    [SerializeField] private int _gemReward = 0;

    [Header("골드 랜덤 배율")]
    [SerializeField] private float _goldMinMultiplier = 0.8f;
    [SerializeField] private float _goldMaxMultiplier = 1.2f;

    private bool _canGiveReward = false;
    private bool _rewardGiven = false;

    public void EnableReward()
    {
        _canGiveReward = true;
    }

    public void DisableReward()
    {
        _canGiveReward = false;
    }

    public void GiveReward()
    {
        if (!_canGiveReward)
            return;

        if (_rewardGiven)
            return;

        _rewardGiven = true;

        if (DataSource.Instance != null)
        {
            if (_goldReward > 0)
            {
                float min = Mathf.Min(_goldMinMultiplier, _goldMaxMultiplier);
                float max = Mathf.Max(_goldMinMultiplier, _goldMaxMultiplier);

                float randomMultiplier = Random.Range(min, max);
                int finalGold = Mathf.RoundToInt(_goldReward * randomMultiplier);
                finalGold = Mathf.Max(0, finalGold);

                DataSource.Instance.AddGold(finalGold);
            }

            if (_gemReward > 0)
                DataSource.Instance.AddGem(_gemReward);
        }

        if (UIManager.Instance != null)
            UIManager.Instance.RefreshCurrencyUI();
    }
}