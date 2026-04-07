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

    [Header("골드 범위")]
    [SerializeField] private int _goldMin;
    [SerializeField] private int _goldMax;

    [Header("젬 범위")]
    [SerializeField] private int _gemMin;
    [SerializeField] private int _gemMax;

    [Header("UI 날아가는 아이콘 프리팹")]
    [SerializeField] private GameObject _goldFlyIconPrefab;
    [SerializeField] private GameObject _gemFlyIconPrefab;

    [Header("UI 캔버스 부모")]
    [SerializeField] private RectTransform _uiCanvasTransform;

    [Header("자동 오픈 시간")]
    [SerializeField] private float _autoOpenDelay = 2f;

    [Header("아이콘 생성 반경")]
    [SerializeField] private float _spawnRadius = 1f;

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

        int rewardGold = GetRandomValue(_goldMin, _goldMax);
        int rewardGem = GetRandomValue(_gemMin, _gemMax);

        if (rewardGold > 0)
        {
            SpawnFlyRewardIcons(RewardType.Gold, rewardGold, _goldFlyIconPrefab, UIManager.Instance.GoldTargetUIPosition);
        }

        if (rewardGem > 0)
        {
            SpawnFlyRewardIcons(RewardType.Gem, rewardGem, _gemFlyIconPrefab, UIManager.Instance.GemTargetUIPosition);
        }

        Destroy(gameObject);
    }

    private void SpawnFlyRewardIcons(RewardType rewardType, int totalAmount, GameObject iconPrefab, Vector3 targetUIPos)
    {
        if (iconPrefab == null || _uiCanvasTransform == null)
            return;

        Camera mainCam = Camera.main;
        if (mainCam == null)
            return;

        int iconCount = GetIconCount(totalAmount);

        int remainAmount = totalAmount;

        for (int i = 0; i < iconCount; i++)
        {
            int amount = (i == iconCount - 1) ? remainAmount : Mathf.Max(1, totalAmount / iconCount);
            remainAmount -= amount;

            Vector3 randomWorldPos = transform.position + GetRandomOffsetInCircle(_spawnRadius);
            Vector3 screenPos = mainCam.WorldToScreenPoint(randomWorldPos);

            GameObject iconObj = Instantiate(iconPrefab, _uiCanvasTransform);
            RewardFlyIcon flyIcon = iconObj.GetComponent<RewardFlyIcon>();

            if (flyIcon != null)
            {
                flyIcon.Init(screenPos, targetUIPos, rewardType, amount);
            }
        }
    }

    private int GetIconCount(int totalAmount)
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
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return new Vector3(randomCircle.x, 0f, randomCircle.y);
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
}