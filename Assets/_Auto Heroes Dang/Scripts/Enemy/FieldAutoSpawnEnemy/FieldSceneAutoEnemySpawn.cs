using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FieldSceneAutoEnemySpawn : MonoBehaviour
{
    [System.Serializable]
    public class StageSpawnData
    {
        [Header("기본 정보")]
        public string stageName;
        public Transform stagePoint;

        [Header("플레이어 감지")]
        public float triggerRadius = 5f;
        public float stayTimeToActivate = 5f;

        [Header("스폰 설정")]
        public List<GameObject> enemyPrefabs = new List<GameObject>();
        public int maxAliveEnemies = 5;
        public float spawnInterval = 2f;
        public int spawnCount = 1;

        [Header("스폰 위치 (스테이지 원 밖)")]
        public float spawnMinDistance = 7f;
        public float spawnMaxDistance = 12f;

        [Header("옵션")]
        public bool keepSpawning = false;
        public bool activateOnlyOnce = true;

        [HideInInspector] public float stayTimer = 0f;
        [HideInInspector] public float spawnTimer = 0f;
        [HideInInspector] public bool isActivated = false;
        [HideInInspector] public bool hasActivatedOnce = false;
        [HideInInspector] public List<GameObject> aliveEnemies = new List<GameObject>();
    }

    [Header("플레이어 감지")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private string _playerTag = "Player";

    [Header("스테이지 목록")]
    [SerializeField] private List<StageSpawnData> _stages = new List<StageSpawnData>();

    [Header("공통 옵션")]
    [SerializeField] private bool _useUnscaledTime = false;
    [SerializeField] private float _spawnHeightOffset = 0f;

    [Header("디버그")]
    [SerializeField] private bool _debugLog = true;
    [SerializeField] private bool _drawGizmos = true;

    private void Awake()
    {
        if (_playerLayer == 0)
            _playerLayer = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (FieldManager.Instance == null)
            return;

        if (!FieldManager.Instance.IsSpawnPossible)
            return;

        float deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        for (int i = 0; i < _stages.Count; i++)
        {
            StageSpawnData stage = _stages[i];

            if (stage == null)
                continue;

            if (stage.stagePoint == null)
                continue;

            CleanupDeadEnemies(stage);

            bool isPlayerInside = IsPlayerInsideStage(stage);

            if (isPlayerInside)
            {
                HandlePlayerInside(stage, deltaTime);
            }
            else
            {
                HandlePlayerOutside(stage);
            }

            if (stage.isActivated)
            {
                HandleSpawn(stage, deltaTime);
            }
        }
    }

    private bool IsPlayerInsideStage(StageSpawnData stage)
    {
        Collider[] hits = Physics.OverlapSphere(
            stage.stagePoint.position,
            stage.triggerRadius,
            _playerLayer
        );

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];

            if (hit == null)
                continue;

            return true;
        }

        return false;
    }

    private void HandlePlayerInside(StageSpawnData stage, float deltaTime)
    {
        if (stage.activateOnlyOnce && stage.hasActivatedOnce)
            return;

        if (!stage.isActivated)
        {
            stage.stayTimer += deltaTime;

            if (stage.stayTimer >= stage.stayTimeToActivate)
            {
                stage.isActivated = true;
                stage.hasActivatedOnce = true;
                stage.spawnTimer = 0f;

                if (_debugLog)
                {
                    Debug.Log($"[FieldSceneAutoEnemySpawn] {stage.stageName} 활성화");
                }
            }
        }
    }

    private void HandlePlayerOutside(StageSpawnData stage)
    {
        stage.stayTimer = 0f;

        if (!stage.keepSpawning)
        {
            stage.isActivated = false;
            stage.spawnTimer = 0f;
        }
    }

    private void HandleSpawn(StageSpawnData stage, float deltaTime)
    {
        if (stage.enemyPrefabs == null || stage.enemyPrefabs.Count == 0)
            return;

        if (stage.aliveEnemies.Count >= stage.maxAliveEnemies)
            return;

        stage.spawnTimer += deltaTime;

        if (stage.spawnTimer < stage.spawnInterval)
            return;

        stage.spawnTimer = 0f;

        int canSpawnCount = Mathf.Min(
            stage.spawnCount,
            stage.maxAliveEnemies - stage.aliveEnemies.Count
        );

        for (int i = 0; i < canSpawnCount; i++)
        {
            SpawnEnemy(stage);
        }
    }

    private void SpawnEnemy(StageSpawnData stage)
    {
        GameObject prefab = GetRandomEnemyPrefab(stage);

        if (prefab == null)
            return;

        Vector3 spawnPos = GetRandomSpawnPosition(stage);
        GameObject spawnedEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        stage.aliveEnemies.Add(spawnedEnemy);

        if (_debugLog)
        {
            Debug.Log($"[FieldSceneAutoEnemySpawn] {stage.stageName} 에서 {spawnedEnemy.name} 스폰");
        }
    }

    private GameObject GetRandomEnemyPrefab(StageSpawnData stage)
    {
        if (stage.enemyPrefabs == null || stage.enemyPrefabs.Count == 0)
            return null;

        int randomIndex = Random.Range(0, stage.enemyPrefabs.Count);
        return stage.enemyPrefabs[randomIndex];
    }

    private Vector3 GetRandomSpawnPosition(StageSpawnData stage)
    {
        Vector3 center = stage.stagePoint.position;

        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(stage.spawnMinDistance, stage.spawnMaxDistance);

        Vector3 dir = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
        Vector3 spawnPos = center + dir * randomDistance;
        spawnPos.y += _spawnHeightOffset;

        return spawnPos;
    }

    private void CleanupDeadEnemies(StageSpawnData stage)
    {
        for (int i = stage.aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (stage.aliveEnemies[i] == null)
            {
                stage.aliveEnemies.RemoveAt(i);
            }
        }
    }

    public void ActivateStageByName(string stageName)
    {
        for (int i = 0; i < _stages.Count; i++)
        {
            if (_stages[i].stageName == stageName)
            {
                _stages[i].isActivated = true;
                _stages[i].hasActivatedOnce = true;
                _stages[i].stayTimer = 0f;
                _stages[i].spawnTimer = 0f;

                if (_debugLog)
                {
                    Debug.Log($"[FieldSceneAutoEnemySpawn] {stageName} 강제 활성화");
                }

                return;
            }
        }
    }

    public void DeactivateStageByName(string stageName)
    {
        for (int i = 0; i < _stages.Count; i++)
        {
            if (_stages[i].stageName == stageName)
            {
                _stages[i].isActivated = false;
                _stages[i].stayTimer = 0f;
                _stages[i].spawnTimer = 0f;

                if (_debugLog)
                {
                    Debug.Log($"[FieldSceneAutoEnemySpawn] {stageName} 비활성화");
                }

                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos || _stages == null)
            return;

        for (int i = 0; i < _stages.Count; i++)
        {
            StageSpawnData stage = _stages[i];

            if (stage == null || stage.stagePoint == null)
                continue;

            Vector3 center = stage.stagePoint.position;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center, stage.triggerRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, stage.spawnMinDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, stage.spawnMaxDistance);
        }
    }
}