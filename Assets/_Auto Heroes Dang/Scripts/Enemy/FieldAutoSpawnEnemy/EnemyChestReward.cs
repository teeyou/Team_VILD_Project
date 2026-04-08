using UnityEngine;

public class EnemyChestReward : MonoBehaviour
{
    [Header("상자 드랍 가능 여부")]
    [SerializeField] private bool _canDropChest = false;

    [Header("상자 드랍 확률")]
    [SerializeField] private float _chestDropChance = 15f;

    [Header("상자 프리팹")]
    [SerializeField] private GameObject _chestAPrefab;
    [SerializeField] private GameObject _chestBPrefab;
    [SerializeField] private GameObject _chestCPrefab;
    [SerializeField] private GameObject _chestDPrefab;

    [Header("생성 위치 보정")]
    [SerializeField] private Vector3 _spawnOffset = Vector3.zero;

    [Header("상자 랜덤 드랍 반경")]
    [SerializeField] private float _dropRadius = 2f;

    private bool _isDropped = false;

    public void EnableChestDrop()
    {
        _canDropChest = true;
    }

    public void DisableChestDrop()
    {
        _canDropChest = false;
    }

    public void TryDropChest()
    {
        if (!_canDropChest)
            return;

        if (_isDropped)
            return;

        _isDropped = true;

        float dropRoll = Random.Range(0f, 100f);

        if (dropRoll > _chestDropChance)
            return;

        GameObject chestPrefab = GetRandomChestPrefab();

        if (chestPrefab == null)
            return;

        Vector3 randomOffset = GetRandomOffsetInCircle(_dropRadius);
        Vector3 spawnPos = transform.position + _spawnOffset + randomOffset;

        float randomY = Random.Range(0f, 360f);
        Quaternion spawnRot = Quaternion.Euler(0f, randomY, 0f);

        Instantiate(chestPrefab, spawnPos, spawnRot);
    }

    private GameObject GetRandomChestPrefab()
    {
        float roll = Random.Range(0f, 100f);

        // A = 최고 등급, D = 최하 등급
        if (roll < 5f)
            return _chestAPrefab;   // 5%

        if (roll < 15f)
            return _chestBPrefab;   // 10%

        if (roll < 40f)
            return _chestCPrefab;   // 25%

        return _chestDPrefab;       // 60%
    }

    private Vector3 GetRandomOffsetInCircle(float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}