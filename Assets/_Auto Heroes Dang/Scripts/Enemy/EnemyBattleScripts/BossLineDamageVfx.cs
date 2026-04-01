using System.Collections.Generic;
using UnityEngine;

public class BossLineDamageVfx : MonoBehaviour
{
    [Header("판정 크기")]
    [SerializeField] private Vector3 _boxHalfExtents = new Vector3(0.8f, 0.5f, 2f);

    [Header("틱당 데미지 비율")]
    [SerializeField] private float _damagePercentPerTick = 0.2f; // 보스 공격력의 20%

    private BossEnemyBattle _owner;
    private Vector3 _direction;
    private float _moveSpeed;
    private float _lifeTime;
    private float _tickInterval;
    private LayerMask _targetLayer;

    private float _lifeTimer;
    private float _tickTimer;

    // 각 유닛별 마지막 피해 시간
    private readonly Dictionary<Unit, float> _lastDamageTime = new Dictionary<Unit, float>();

    public void Init(
        BossEnemyBattle owner,
        Vector3 direction,
        float moveSpeed,
        float lifeTime,
        float tickInterval,
        LayerMask targetLayer)
    {
        _owner = owner;
        _direction = direction.normalized;
        _moveSpeed = moveSpeed;
        _lifeTime = lifeTime;
        _tickInterval = tickInterval;
        _targetLayer = targetLayer;
    }

    private void Update()
    {
        transform.position += _direction * _moveSpeed * Time.deltaTime;

        _lifeTimer += Time.deltaTime;
        _tickTimer += Time.deltaTime;

        if (_tickTimer >= _tickInterval)
        {
            _tickTimer = 0f;
            ApplyTickDamage();
        }

        if (_lifeTimer >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    // 현재 VFX 위치를 기준으로 박스 범위 안의 유닛에게 틱 데미지를 적용
    // 보스 현재 공격력의 20%를 기본값으로 사용하고 크리티컬도 적용
    private void ApplyTickDamage()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            _boxHalfExtents,
            transform.rotation,
            _targetLayer
        );

        for (int i = 0; i < hits.Length; i++)
        {
            Unit unit = hits[i].GetComponent<Unit>();

            if (unit == null)
                continue;

            if (unit.IsDead)
                continue;

            if (_owner != null && unit == _owner)
                continue;

            if (_lastDamageTime.TryGetValue(unit, out float lastTime))
            {
                if (Time.time < lastTime + _tickInterval)
                    continue;
            }

            _lastDamageTime[unit] = Time.time;

            int baseAtk = _owner != null ? _owner.Atk : 1;
            int tickAtk = Mathf.Max(1, Mathf.RoundToInt(baseAtk * _damagePercentPerTick));

            bool isCritical;
            int finalDamage = DamageCalculator.CalculateDamage(tickAtk, unit.Def, out isCritical);

            unit.TakeDamage(finalDamage, _owner != null ? _owner.transform : transform, isCritical);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _boxHalfExtents * 2f);
        Gizmos.matrix = oldMatrix;
    }
}