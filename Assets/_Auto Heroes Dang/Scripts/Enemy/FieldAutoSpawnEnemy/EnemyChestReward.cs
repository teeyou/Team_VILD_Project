using UnityEngine;

public class EnemyChestReward : MonoBehaviour
{
    [Header("상자 드랍 확률")]
    [SerializeField] private float _chestDropChance = 15f;

    [Header("상자 프리팹")]
    [SerializeField] private GameObject _chestAPrefab;
    [SerializeField] private GameObject _chestBPrefab;
    [SerializeField] private GameObject _chestCPrefab;
    [SerializeField] private GameObject _chestDPrefab;

    [Header("생성 위치 보정")]
    [SerializeField] private Vector3 _spawnOffset = Vector3.zero;

    private bool _canDropChest = false;
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

        Vector3 spawnPos = transform.position + _spawnOffset;
        Instantiate(chestPrefab, spawnPos, Quaternion.identity);
    }

    private GameObject GetRandomChestPrefab()
    {
        float roll = Random.Range(0f, 100f);

        if (roll < 60f)
            return _chestAPrefab;

        if (roll < 85f)
            return _chestBPrefab;

        if (roll < 95f)
            return _chestCPrefab;

        return _chestDPrefab;
    }
}