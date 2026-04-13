using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilMageSkill : MonoBehaviour
{
    [Header("탐색")]
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _skillRange = 6f;
    [SerializeField] private int _targetCount = 2;

    [Header("다단히트 설정")]
    [SerializeField] private int _hitCount = 5;
    [SerializeField] private float _hitInterval = 0.3f;

    [Header("VFX")]
    [SerializeField] private GameObject _hitVfxPrefab;
    [SerializeField] private float _vfxYOffset = 1f;
    [SerializeField] private float _vfxLifeTime = 1f;

    private NamedEnemyBattle _owner;
    private Coroutine _skillCoroutine;

    private void Awake()
    {
        _owner = GetComponent<NamedEnemyBattle>();

        if (_targetLayer == 0)
            _targetLayer = LayerMask.GetMask("Player");
    }

    public void CastMultiHitSkill()
    {
        if (_owner == null)
            return;

        if (_owner.IsDead)
            return;

        List<Transform> targets = FindClosestTargetsInRange(_targetCount);

        if (targets.Count == 0)
            return;

        if (_skillCoroutine != null)
            StopCoroutine(_skillCoroutine);

        _skillCoroutine = StartCoroutine(Co_MultiHit(targets));
    }

    private List<Transform> FindClosestTargetsInRange(int count)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _skillRange, _targetLayer);
        List<Transform> candidates = new List<Transform>();

        for (int i = 0; i < hits.Length; i++)
        {
            Transform candidate = hits[i].transform;

            if (candidate == null)
                continue;

            if (candidates.Contains(candidate))
                continue;

            candidates.Add(candidate);
        }

        candidates.Sort((a, b) =>
        {
            float distA = (a.position - transform.position).sqrMagnitude;
            float distB = (b.position - transform.position).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        List<Transform> result = new List<Transform>();
        int finalCount = Mathf.Min(count, candidates.Count);

        for (int i = 0; i < finalCount; i++)
        {
            result.Add(candidates[i]);
        }

        return result;
    }

    private IEnumerator Co_MultiHit(List<Transform> targets)
    {
        for (int hitIndex = 0; hitIndex < _hitCount; hitIndex++)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Transform target = targets[i];

                if (target == null)
                    continue;

                Unit targetUnit = target.GetComponent<Unit>();

                if (targetUnit == null)
                    continue;

                if (targetUnit.IsDead)
                    continue;

                int finalDamage = _owner.GetSkillDamage(targetUnit);

                targetUnit.TakeDamage(finalDamage, transform);
                SpawnHitVfx(target);
                AudioManager.Instance.PlaySFX("EvilMageSkill");

                if (_owner != null)
                {
                    //Debug.Log($"{name} >> {targetUnit.name} 다단히트 스킬 / 최종 데미지 : {finalDamage} ({hitIndex + 1}/{_hitCount})");
                }
            }

            if (hitIndex < _hitCount - 1)
                yield return new WaitForSeconds(_hitInterval);
        }

        _skillCoroutine = null;
    }

    private void SpawnHitVfx(Transform target)
    {
        if (_hitVfxPrefab == null || target == null)
            return;

        Vector3 spawnPos = target.position;
        spawnPos.y += _vfxYOffset;

        GameObject vfx = Instantiate(_hitVfxPrefab, spawnPos, Quaternion.identity);
        Destroy(vfx, _vfxLifeTime);
    }
}