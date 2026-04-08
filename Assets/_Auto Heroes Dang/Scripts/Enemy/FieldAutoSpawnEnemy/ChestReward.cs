using UnityEngine;
using System.Collections;

public enum ChestGrade
{
    A,
    B,
    C,
    D
}

public class ChestReward : MonoBehaviour
{
    [Header("상자 등급")]
    [SerializeField] private ChestGrade _chestGrade;

    [Header("상자 뚜껑")]
    [SerializeField] private Transform _cover;
    [SerializeField] private float _openDuration = 0.25f;
    [SerializeField] private float _openTargetX = -90f;

    [Header("스테이지 보상 비율 사용")]
    [SerializeField] private bool _useStageRewardRate = true;

    [Header("스테이지 클리어 보상의 평균 비율")]
    [SerializeField] private float _goldAverageRate = 0.1f;
    [SerializeField] private float _gemAverageRate = 0.1f;

    [Header("평균 대비 랜덤 배율")]
    [SerializeField] private float _goldMinMultiplier = 0.8f;
    [SerializeField] private float _goldMaxMultiplier = 1.2f;
    [SerializeField] private float _gemMinMultiplier = 0.8f;
    [SerializeField] private float _gemMaxMultiplier = 1.2f;

    [Header("직접 입력 범위(비율 미사용 시)")]
    [SerializeField] private int _goldMin;
    [SerializeField] private int _goldMax;
    [SerializeField] private int _gemMin;
    [SerializeField] private int _gemMax;

    [Header("아이템 보상 예정")]
    [SerializeField] private bool _useItemReward = false;

    [Header("자동 오픈 시간")]
    [SerializeField] private float _autoOpenDelay = 2f;

    [Header("월드 보상 프리팹")]
    [SerializeField] private GameObject _worldGoldPrefab;
    [SerializeField] private GameObject _worldGemPrefab;

    [Header("보상 드랍 반경")]
    [SerializeField] private float _dropRadius = 1f;

    [Header("생성 높이 보정")]
    [SerializeField] private float _spawnYOffset = 0.5f;

    [Header("상자 제거 시간")]
    [SerializeField] private float _destroyDelayAfterOpen = 1.5f;

    private bool _isOpened = false;

    private void Start()
    {
        StartCoroutine(Co_AutoOpen());
    }

    private IEnumerator Co_AutoOpen()
    {
        yield return new WaitForSeconds(_autoOpenDelay);
        OpenChest();
    }

    public void OpenChest()
    {
        if (_isOpened)
            return;

        _isOpened = true;
        StartCoroutine(Co_OpenChest());
    }

    private IEnumerator Co_OpenChest()
    {
        yield return StartCoroutine(Co_OpenCover());

        int rewardGold;
        int rewardGem;

        if (_useStageRewardRate && GameManager.Instance != null)
        {
            rewardGold = GetStageBasedReward(
                GameManager.Instance.GetCurrentStageGoldReward(),
                _goldAverageRate,
                _goldMinMultiplier,
                _goldMaxMultiplier
            );

            rewardGem = GetStageBasedReward(
                GameManager.Instance.GetCurrentStageGemReward(),
                _gemAverageRate,
                _gemMinMultiplier,
                _gemMaxMultiplier
            );
        }
        else
        {
            rewardGold = GetRandomValue(_goldMin, _goldMax);
            rewardGem = GetRandomValue(_gemMin, _gemMax);
        }

        // A, B 상자만 젬 가능
        if (_chestGrade == ChestGrade.C || _chestGrade == ChestGrade.D)
        {
            rewardGem = 0;
        }

        if (rewardGold > 0)
        {
            DataSource.Instance.AddGold(rewardGold);
            SpawnWorldRewards(rewardGold, _worldGoldPrefab);
        }

        if (rewardGem > 0)
        {
            DataSource.Instance.AddGem(rewardGem);
            SpawnWorldRewards(rewardGem, _worldGemPrefab);
        }

        if (_useItemReward)
        {
            Debug.Log($"{_chestGrade} 상자 : 아이템 보상은 이후 구현 예정");
        }

        if (UIManager.Instance != null)
            UIManager.Instance.RefreshCurrencyUI();

        Destroy(gameObject, _destroyDelayAfterOpen);
    }

    private int GetStageBasedReward(int stageReward, float averageRate, float minMultiplier, float maxMultiplier)
    {
        int averageReward = Mathf.RoundToInt(stageReward * averageRate);

        if (averageReward <= 0)
            return 0;

        float min = Mathf.Min(minMultiplier, maxMultiplier);
        float max = Mathf.Max(minMultiplier, maxMultiplier);

        float randomMultiplier = Random.Range(min, max);
        int finalReward = Mathf.RoundToInt(averageReward * randomMultiplier);

        return Mathf.Max(0, finalReward);
    }

    private IEnumerator Co_OpenCover()
    {
        if (_cover == null)
            yield break;

        float time = 0f;
        Vector3 startEuler = _cover.localEulerAngles;

        float startX = NormalizeAngle(startEuler.x);
        float targetX = _openTargetX;

        while (time < _openDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _openDuration);

            float x = Mathf.Lerp(startX, targetX, t);
            _cover.localRotation = Quaternion.Euler(x, startEuler.y, startEuler.z);

            yield return null;
        }

        _cover.localRotation = Quaternion.Euler(targetX, startEuler.y, startEuler.z);
    }

    private void SpawnWorldRewards(int totalAmount, GameObject prefab)
    {
        if (prefab == null)
            return;

        int spawnCount = GetRewardObjectCount(totalAmount);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * _spawnYOffset;
            Vector3 targetWorldPos = spawnPos + GetRandomOffsetInCircle(_dropRadius);

            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
            WorldRewardPickup pickup = obj.GetComponent<WorldRewardPickup>();

            if (pickup != null)
            {
                pickup.Init(targetWorldPos);
            }
        }
    }

    private int GetRewardObjectCount(int totalAmount)
    {
        if (totalAmount >= 1000)
            return 8;
        if (totalAmount >= 300)
            return 5;
        if (totalAmount >= 100)
            return 3;

        return 1;
    }

    private Vector3 GetRandomOffsetInCircle(float radius)
    {
        Vector2 circle = Random.insideUnitCircle * radius;
        return new Vector3(circle.x, 0f, circle.y);
    }

    private int GetRandomValue(int min, int max)
    {
        if (max < min)
        {
            int temp = min;
            min = max;
            max = temp;
        }

        if (max <= 0)
            return 0;

        return Random.Range(min, max + 1);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180f)
            angle -= 360f;

        return angle;
    }
}