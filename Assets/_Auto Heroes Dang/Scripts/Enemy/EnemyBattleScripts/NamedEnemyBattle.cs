using System.Collections.Generic;
using UnityEngine;

public enum SkillAreaType
{
    Circle,
    Fan
}

public class NamedEnemyBattle : NormalEnemyBattle
{
    [Header("스킬 설정")]
    [SerializeField] protected float _skillRange = 3f;
    [SerializeField] protected int _skillLoopCount = 1;

    [Header("첫 스킬 대기")]
    [SerializeField] protected float _firstSkillDelay = 2f;

    [Header("범위 스킬 설정")]
    [SerializeField] protected SkillAreaType _skillAreaType = SkillAreaType.Circle;
    [SerializeField] protected float _skillRadius = 3f;
    [SerializeField] protected float _skillAngle = 120f;

    [Header("스킬 치명타")]
    [SerializeField] protected bool _useSkillCritical = true;
    [SerializeField] protected int _skillCriticalPercent = 20;
    [SerializeField] protected float _skillCriticalMultiplier = 1.5f;

    [Header("스킬 VFX")]
    [SerializeField] private GameObject _skillVfxPrefab;
    [SerializeField] private Transform _skillVfxSpawnPoint;
    [SerializeField] private Vector3 _skillVfxPositionOffset;
    [SerializeField] private Vector3 _skillVfxRotationOffset;
    [SerializeField] private float _skillVfxDestroyTime = 3f;
    [SerializeField] private float _skillVfxPlaySpeed = 1f;

    protected float _lastSkillTime = -999f;
    protected bool _isUsingSkill;
    protected int _currentSkillLoop;

    protected float _spawnTime;
    protected bool _hasUsedSkillOnce;

    protected override void Start()
    {
        base.Start();

        _spawnTime = Time.time;
        _hasUsedSkillOnce = false;
    }

    protected override void HandleCombat()
    {
        if (_isDead)
            return;

        if (_target == null)
        {
            SetMoveAnimation(false);
            return;
        }

        if (_isUsingSkill)
            return;

        if (CanUseSkill())
        {
            StartSkill();
            return;
        }

        base.HandleCombat();
    }

    public override void Attack()
    {
        if (_isUsingSkill)
            return;

        base.Attack();
    }

    protected virtual bool CanUseSkill()
    {
        if (_target == null)
            return false;

        if (_target.IsDead)
            return false;

        if (_isUsingSkill)
            return false;

        if (_isAttacking)
            return false;

        if (!_hasUsedSkillOnce)
        {
            if (Time.time < _spawnTime + _firstSkillDelay)
                return false;
        }
        else
        {
            if (Time.time < _lastSkillTime + _skillCool)
                return false;
        }

        float distance = GetDistanceTo(_target);
        if (distance > _skillRange)
            return false;

        return true;
    }

    protected virtual void StartSkill()
    {
        _isUsingSkill = true;
        _currentSkillLoop = 0;
        _hasUsedSkillOnce = true;

        SetMoveAnimation(false);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.ResetTrigger("Skill");
            _animator.SetTrigger("Skill");
        }

        if (_battleLog)
        {
            Debug.Log($"{name} 스킬 시작");
        }
    }

    protected virtual List<Unit> GetSkillTargets()
    {
        switch (_skillAreaType)
        {
            case SkillAreaType.Circle:
                return GetTargetsInCircle(transform.position, _skillRadius);

            case SkillAreaType.Fan:
                return GetTargetsInFan(transform.position, transform.forward, _skillRadius, _skillAngle);
        }

        return new List<Unit>();
    }

    protected virtual List<Unit> GetTargetsInCircle(Vector3 center, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, _targetLayer);
        List<Unit> targets = new List<Unit>();

        for (int i = 0; i < hits.Length; i++)
        {
            Unit unit = hits[i].GetComponent<Unit>();

            if (unit == null)
                continue;

            if (unit == this)
                continue;

            if (unit.IsDead)
                continue;

            if (targets.Contains(unit))
                continue;

            targets.Add(unit);
        }

        return targets;
    }

    protected virtual List<Unit> GetTargetsInFan(Vector3 center, Vector3 forward, float radius, float angle)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, _targetLayer);
        List<Unit> targets = new List<Unit>();

        Vector3 flatForward = forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        for (int i = 0; i < hits.Length; i++)
        {
            Unit unit = hits[i].GetComponent<Unit>();

            if (unit == null)
                continue;

            if (unit == this)
                continue;

            if (unit.IsDead)
                continue;

            if (targets.Contains(unit))
                continue;

            Vector3 dirToTarget = unit.transform.position - center;
            dirToTarget.y = 0f;

            if (dirToTarget == Vector3.zero)
                continue;

            float targetAngle = Vector3.Angle(flatForward, dirToTarget.normalized);

            if (targetAngle <= angle * 0.5f)
            {
                targets.Add(unit);
            }
        }

        return targets;
    }

    protected virtual int CalculateSkillDamage(Unit target)
    {
        if (target == null)
            return 1;

        float atk = _atk * _skillMultiplier;
        float def = target.Def;

        float totalDamage = atk * (atk / (atk + def));

        if (CalculateSkillCriticalProb())
        {
            totalDamage *= _skillCriticalMultiplier;
        }

        return Mathf.Max(1, Mathf.RoundToInt(totalDamage));
    }

    public int GetSkillDamage(Unit target)
    {
        return CalculateSkillDamage(target);
    }

    protected virtual bool CalculateSkillCriticalProb()
    {
        if (!_useSkillCritical)
            return false;

        int roll = Random.Range(0, 100);
        return roll < _skillCriticalPercent;
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// 범위 스킬 데미지 적용
    /// </summary>
    public virtual void ApplyAreaSkill()
    {
        if (_isDead)
            return;

        List<Unit> targets = GetSkillTargets();

        for (int i = 0; i < targets.Count; i++)
        {
            Unit unit = targets[i];

            if (unit == null)
                continue;

            if (unit.IsDead)
                continue;

            AudioManager.Instance.PlaySFX("OrcSkill");

            int finalDamage = CalculateSkillDamage(unit);

            unit.TakeDamage(finalDamage, transform);
            _totalDamage += finalDamage;

            if (_battleLog)
            {
                Debug.Log($"{name} >> {unit.name} 스킬 공격 / 최종 데미지 : {finalDamage}");
            }
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// 단일 타겟 스킬 데미지 적용
    /// </summary>
    public virtual void ApplySingleSkill()
    {
        if (_isDead)
            return;

        if (_target == null)
            return;

        if (_target.IsDead)
            return;

        float distance = GetDistanceTo(_target);
        if (distance > _skillRange)
            return;

        int finalDamage = CalculateSkillDamage(_target);

        _target.TakeDamage(finalDamage, transform);
        _totalDamage += finalDamage;

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_target.name} 단일 스킬 공격 / 최종 데미지 : {finalDamage}");
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// 스킬 루프 종료 처리
    /// </summary>
    public virtual void EndSkillLoop()
    {
        _currentSkillLoop++;

        if (_currentSkillLoop >= _skillLoopCount)
        {
            _isUsingSkill = false;
            _lastSkillTime = Time.time;
            _nextActionTime = Time.time + _postAttackDelay;

            if (_battleLog)
            {
                Debug.Log($"{name} 스킬 종료");
            }

            return;
        }

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.ResetTrigger("Skill");
            _animator.SetTrigger("Skill");
        }
    }

    protected virtual Vector3 GetSkillVfxSpawnPosition()
    {
        if (_skillVfxSpawnPoint != null)
        {
            return _skillVfxSpawnPoint.position + _skillVfxSpawnPoint.TransformDirection(_skillVfxPositionOffset);
        }

        return transform.position + transform.TransformDirection(_skillVfxPositionOffset);
    }

    protected virtual Quaternion GetSkillVfxSpawnRotation()
    {
        Quaternion baseRotation = _skillVfxSpawnPoint != null
            ? _skillVfxSpawnPoint.rotation
            : transform.rotation;

        return baseRotation * Quaternion.Euler(_skillVfxRotationOffset);
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출
    /// 스킬 VFX 생성
    /// </summary>
    public virtual void SpawnSkillVfx()
    {
        if (_skillVfxPrefab == null)
            return;

        Vector3 spawnPos = GetSkillVfxSpawnPosition();
        Quaternion spawnRot = GetSkillVfxSpawnRotation();

        GameObject vfxObj = Instantiate(_skillVfxPrefab, spawnPos, spawnRot);

        SetParticleSpeed(vfxObj, _skillVfxPlaySpeed);

        Destroy(vfxObj, _skillVfxDestroyTime);
    }

    private void SetParticleSpeed(GameObject vfxObj, float speedMultiplier)
    {
        ParticleSystem[] particleSystems = vfxObj.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < particleSystems.Length; i++)
        {
            var main = particleSystems[i].main;
            main.simulationSpeed = speedMultiplier;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (!_drawGizmos)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _skillRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _skillRadius);

        if (_skillAreaType == SkillAreaType.Fan)
        {
            DrawFanGizmo(transform.position, transform.forward, _skillRadius, _skillAngle, Color.green);
        }
    }

    protected virtual void DrawFanGizmo(Vector3 center, Vector3 forward, float radius, float angle, Color color)
    {
        Gizmos.color = color;

        Vector3 flatForward = forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        Quaternion leftRot = Quaternion.Euler(0f, -angle * 0.5f, 0f);
        Quaternion rightRot = Quaternion.Euler(0f, angle * 0.5f, 0f);

        Vector3 leftDir = leftRot * flatForward;
        Vector3 rightDir = rightRot * flatForward;

        Gizmos.DrawLine(center, center + leftDir * radius);
        Gizmos.DrawLine(center, center + rightDir * radius);

        int segmentCount = 20;
        Vector3 prevPoint = center + leftDir * radius;

        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float currentAngle = Mathf.Lerp(-angle * 0.5f, angle * 0.5f, t);
            Vector3 dir = Quaternion.Euler(0f, currentAngle, 0f) * flatForward;
            Vector3 point = center + dir * radius;

            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}