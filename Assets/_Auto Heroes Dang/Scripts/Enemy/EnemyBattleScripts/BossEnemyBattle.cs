using System.Collections.Generic;
using UnityEngine;

public class BossEnemyBattle : NamedEnemyBattle
{
    [Header("보스 기본 공격 트리거")]
    [SerializeField] private string _attackTrigger1 = "Attack1";
    [SerializeField] private string _attackTrigger2 = "Attack2";

    [Header("Attack1 - 직선 VFX")]
    [SerializeField] private GameObject _attack1LineVfxPrefab;
    [SerializeField] private float _attack1LineDamage = 5f;
    [SerializeField] private float _attack1LineSpeed = 8f;
    [SerializeField] private float _attack1LineLifeTime = 2f;
    [SerializeField] private float _attack1LineTickInterval = 0.3f;
    [SerializeField] private float _attack1SpawnHeight = 0.5f;
    [SerializeField] private Vector3 _attack1SpawnOffset = Vector3.zero;
    [SerializeField] private LayerMask _attack1TargetLayer;

    [Header("Attack2 - 전방 부채꼴")]
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

        // 공격 직전에 타겟 방향을 바라보게 한다
        FaceTargetImmediately(_target);

        Vector3 lookDir = _target.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            transform.forward = lookDir.normalized;
        }

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
    /// 보스의 현재 공격력과 대상 방어력을 기준으로 데미지를 계산한다
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

        bool isCritical;
        int finalDamage = DamageCalculator.CalculateDamage(_atk, _lockedAttackTarget.Def, out isCritical);

        _lockedAttackTarget.TakeDamage(finalDamage, transform, isCritical);

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_lockedAttackTarget.name} Attack1 근접타 / 최종 데미지 : {finalDamage} / 크리티컬 : {isCritical}");
        }
    }

    // Attack1 직후 보스 정면 방향으로 직선 VFX를 발사
    // y값은 0.5f로 고정해서 수평으로 날아가게 한다
    public void SpawnAttack1LineVfx()
    {
        if (_attack1LineVfxPrefab == null)
            return;

        Vector3 spawnPos =
            transform.position +
            transform.right * _attack1SpawnOffset.x +
            transform.up * _attack1SpawnOffset.y +
            transform.forward * _attack1SpawnOffset.z;

        spawnPos.y = _attack1SpawnHeight;

        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        if (dir == Vector3.zero)
            dir = Vector3.forward;

        Quaternion rot = Quaternion.LookRotation(dir);

        GameObject obj = Instantiate(_attack1LineVfxPrefab, spawnPos, rot);

        BossLineDamageVfx lineVfx = obj.GetComponent<BossLineDamageVfx>();
        if (lineVfx != null)
        {
            lineVfx.Init(
                owner: this,
                direction: dir,
                moveSpeed: _attack1LineSpeed,
                lifeTime: _attack1LineLifeTime,
                tickInterval: _attack1LineTickInterval,
                targetLayer: _attack1TargetLayer
            );
        }
    }

    /// <summary>
    /// Attack2 애니메이션 이벤트
    /// 정면 부채꼴 범위의 플레이어 전부에게
    /// 보스의 현재 공격력을 기준으로 데미지를 계산해서 적용한다
    /// </summary>
    public void ApplyBossAttack2FanDamage()
    {
        if (_isDead)
            return;

        float fanRange = _attackRange + 1.5f;

        List<Unit> targets = GetTargetsInFan(
            transform.position,
            transform.forward,
            fanRange,
            _attack2Angle
        );

        for (int i = 0; i < targets.Count; i++)
        {
            Unit unit = targets[i];

            if (unit == null)
                continue;

            if (unit.IsDead)
                continue;

            bool isCritical;
            int finalDamage = DamageCalculator.CalculateDamage(_atk, unit.Def, out isCritical);

            unit.TakeDamage(finalDamage, transform, isCritical);

            if (_battleLog)
            {
                Debug.Log($"{name} >> {unit.name} Attack2 범위타 / 최종 데미지 : {finalDamage} / 크리티컬 : {isCritical} / 범위 : {fanRange}");
            }
        }
    }

    // Attack2 애니메이션 이벤트
    // 보스 자신의 위치 기준으로 VFX 생성
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

    // Attack1 / Attack2 애니메이션 마지막 이벤트
    public void EndBossAttack()
    {
        _isAttacking = false;
        _lockedAttackTarget = null;
        _nextActionTime = Time.time + _postAttackDelay;
    }
}