using System.Collections.Generic;
using UnityEngine;

public class BishopKnightSkill : MonoBehaviour
{
    [Header("버프 대상 탐색")]
    [SerializeField] private LayerMask _allyLayer;
    [SerializeField] private float _shieldRange = 6f;
    [SerializeField] private bool _includeSelf = false;
    [SerializeField] private int _targetCount = 2;

    [Header("버프 수치")]
    [SerializeField] private int _defBuffAmount = 3;
    [SerializeField] private float _buffDuration = 10f;

    [Header("쉴드 VFX")]
    [SerializeField] private GameObject _shieldVfxPrefab;
    [SerializeField] private Vector3 _vfxOffset = new Vector3(0f, 1f, 0f);

    [Header("디버그")]
    [SerializeField] private bool _debugLog = true;
    [SerializeField] private bool _drawGizmos = true;

    private NormalEnemyBattle _owner;

    private void Awake()
    {
        _owner = GetComponent<NormalEnemyBattle>();

        if (_allyLayer == 0)
            _allyLayer = LayerMask.GetMask("Enemy");
    }

    /// <summary>
    /// 방어 애니메이션 이벤트에서 호출
    /// 범위 안의 Enemy 레이어 아군 중 랜덤하게 여러 명에게 쉴드 부여
    /// </summary>
    public void ApplyRandomShieldToAlly()
    {
        if (_owner == null)
            return;

        if (_owner.IsDead)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, _shieldRange, _allyLayer);
        List<NormalEnemyBattle> candidates = new List<NormalEnemyBattle>();

        for (int i = 0; i < hits.Length; i++)
        {
            NormalEnemyBattle ally = hits[i].GetComponent<NormalEnemyBattle>();

            if (ally == null)
                continue;

            if (ally.IsDead)
                continue;

            if (!_includeSelf && ally.gameObject == gameObject)
                continue;

            if (candidates.Contains(ally))
                continue;

            candidates.Add(ally);
        }

        if (candidates.Count == 0)
        {
            if (_debugLog)
                Debug.Log($"{name} : 쉴드를 줄 아군이 범위 안에 없습니다.");

            return;
        }

        int finalTargetCount = Mathf.Min(_targetCount, candidates.Count);

        for (int i = 0; i < finalTargetCount; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            NormalEnemyBattle selectedAlly = candidates[randomIndex];

            selectedAlly.ApplyDefenseBuff(_defBuffAmount, _buffDuration);

            if (_shieldVfxPrefab != null)
            {
                Vector3 spawnPos = selectedAlly.transform.position + _vfxOffset;
                GameObject vfx = Instantiate(
                    _shieldVfxPrefab,
                    spawnPos,
                    Quaternion.identity,
                    selectedAlly.transform
                );

                Destroy(vfx, _buffDuration);
            }

            if (_debugLog)
            {
                Debug.Log($"{name} >> {selectedAlly.name} 에게 방어력 +{_defBuffAmount} 쉴드 부여 ({_buffDuration}초)");
            }

            // 중복 선택 방지
            candidates.RemoveAt(randomIndex);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _shieldRange);
    }
}