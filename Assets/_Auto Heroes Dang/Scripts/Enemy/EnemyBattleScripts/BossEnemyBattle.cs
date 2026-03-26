using System.Collections.Generic;
using UnityEngine;

public class BossEnemyBattle : NamedEnemyBattle
{
    [Header("보스 기본 공격 트리거")]
    [SerializeField] private string _attackTrigger1 = "Attack1";
    [SerializeField] private string _attackTrigger2 = "Attack2";

    [Header("Attack1 - 내려찍기")]
    [SerializeField] private int _attack1MeleeDamage = 20;

    [Header("Attack2 - 전방 부채꼴")]
    [SerializeField] private int _attack2Damage = 25;
    [SerializeField, Range(0f, 360f)] private float _attack2Angle = 160f;

    [Header("Attack2 - VFX")]
    [SerializeField] private GameObject _attack2VfxPrefab;
    [SerializeField] private Vector3 _attack2VfxOffset = new Vector3(0f, 0f, 1.5f);
    [SerializeField] private Vector3 _attack2VfxRotationOffset = Vector3.zero;
    [SerializeField] private float _attack2VfxLifeTime = 2f;

    [Header("디버그")]
    [SerializeField] private bool _drawBossAttackGizmos = true;

    private int _currentAttackType; // 0 = Attack1, 1 = Attack2

    public override void Attack()
    {
        if (_target == null)
            return;

        if (_target.IsDead)
            return;

        if (_isAttacking)
            return;

        if (_isRotating)
            return;

        if (_isUsingSkill)
            return;

        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        float distance = GetDistanceTo(_target);
        if (distance > _attackRange + _stopDistanceOffset)
            return;

        _lockedAttackTarget = _target;
        _lastAttackTime = Time.time;
        _isAttacking = true;

        SetMoveAnimation(false);

        _currentAttackType = Random.Range(0, 2);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            if (_currentAttackType == 0)
            {
                _animator.ResetTrigger(_attackTrigger2);
                _animator.SetTrigger(_attackTrigger1);
            }
            else
            {
                _animator.ResetTrigger(_attackTrigger1);
                _animator.SetTrigger(_attackTrigger2);
            }
        }
    }

    /// <summary>
    /// Attack1 애니메이션 이벤트
    /// 큰칼이 실제로 내려찍히는 프레임에 연결
    /// </summary>
    public void ApplyBossAttack1MeleeDamage()
    {
        if (_isDead)
            return;

        if (_lockedAttackTarget == null)
            return;

        if (_lockedAttackTarget.IsDead)
            return;

        float distance = GetDistanceTo(_lockedAttackTarget);
        if (distance > _attackRange + _stopDistanceOffset)
            return;

        _lockedAttackTarget.TakeDamage(_attack1MeleeDamage, transform);

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_lockedAttackTarget.name} Attack1 근접타 / 데미지 : {_attack1MeleeDamage}");
        }
    }

    /// <summary>
    /// Attack2 애니메이션 이벤트
    /// 정면 160도 범위의 플레이어 전부 데미지
    /// 반경은 _attackRange를 그대로 사용
    /// </summary>
    public void ApplyBossAttack2FanDamage()
    {
        if (_isDead)
            return;

        List<Unit> targets = GetTargetsInFan(
            transform.position,
            transform.forward,
            _attackRange,
            _attack2Angle
        );

        for (int i = 0; i < targets.Count; i++)
        {
            Unit unit = targets[i];

            if (unit == null)
                continue;

            if (unit.IsDead)
                continue;

            unit.TakeDamage(_attack2Damage, transform);

            if (_battleLog)
            {
                Debug.Log($"{name} >> {unit.name} Attack2 범위타 / 데미지 : {_attack2Damage}");
            }
        }
    }

    /// <summary>
    /// Attack2 애니메이션 이벤트
    /// 보스 자신의 위치 기준으로 VFX 생성
    /// </summary>
    public void SpawnAttack2Vfx()
    {
        if (_attack2VfxPrefab == null)
            return;

        Vector3 spawnPos =
            transform.position +
            transform.right * _attack2VfxOffset.x +
            transform.up * _attack2VfxOffset.y +
            transform.forward * _attack2VfxOffset.z;

        Quaternion spawnRot = transform.rotation * Quaternion.Euler(_attack2VfxRotationOffset);

        GameObject vfx = Instantiate(_attack2VfxPrefab, spawnPos, spawnRot);

        if (_attack2VfxLifeTime > 0f)
        {
            Destroy(vfx, _attack2VfxLifeTime);
        }
    }

    /// <summary>
    /// Attack1 / Attack2 애니메이션 마지막 이벤트
    /// </summary>
    public void EndBossAttack()
    {
        _isAttacking = false;
        _lockedAttackTarget = null;
        _nextActionTime = Time.time + _postAttackDelay;
    }
}