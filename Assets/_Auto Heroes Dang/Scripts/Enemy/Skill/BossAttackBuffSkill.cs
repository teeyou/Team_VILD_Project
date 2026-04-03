using System.Collections.Generic;
using UnityEngine;

public class BossAllyAttackBuffSkill : MonoBehaviour
{
    [Header("버프 대상 탐색")]
    [SerializeField] private LayerMask _allyLayer;
    [SerializeField] private float _buffRange = 6f;
    [SerializeField] private bool _includeSelf = true;

    [Header("버프 수치(기본 공격력 기준 %)")]
    [SerializeField, Range(0f, 5f)] private float _atkBuffPercent = 0.2f;
    [SerializeField] private float _buffDuration = 10f;

    [Header("버프 VFX")]
    [SerializeField] private GameObject _buffVfxPrefab;
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

    public void ApplyAttackBuffToAllAllies()
    {
        if (_owner == null)
            return;

        if (_owner.IsDead)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, _buffRange, _allyLayer);
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

            AudioManager.Instance.PlaySFX("BossSkill");
        }

        if (candidates.Count == 0)
        {
            if (_debugLog)
                Debug.Log($"{name} : 공격력 버프를 줄 아군이 범위 안에 없습니다.");

            return;
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            NormalEnemyBattle ally = candidates[i];

            ally.ApplyAttackBuffPercent(_atkBuffPercent, _buffDuration);

            if (_buffVfxPrefab != null)
            {
                Vector3 spawnPos = ally.transform.position + _vfxOffset;
                GameObject vfx = Instantiate(
                    _buffVfxPrefab,
                    spawnPos,
                    Quaternion.identity,
                    ally.transform
                );

                Destroy(vfx, _buffDuration);
            }

            if (_debugLog)
            {
                Debug.Log($"{name} >> {ally.name} 에게 공격력 {(int)(_atkBuffPercent * 100f)}% 버프 부여 ({_buffDuration}초)");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _buffRange);
    }
}