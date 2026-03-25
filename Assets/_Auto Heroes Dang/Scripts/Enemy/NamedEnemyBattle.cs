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
    [SerializeField] protected int _skillDamage = 50;
    [SerializeField] protected int _skillLoopCount = 1;

    [Header("첫 스킬 대기")]
    [SerializeField] protected float _firstSkillDelay = 2f;

    [Header("범위 스킬 설정")]
    [SerializeField] protected SkillAreaType _skillAreaType = SkillAreaType.Circle;
    [SerializeField] protected float _skillRadius = 3f;
    [SerializeField] protected float _skillAngle = 120f;

    [Header("스킬 VFX")]
    [SerializeField] private GameObject _skillVfxPrefab;
    [SerializeField] private Transform _skillVfxSpawnPoint;
    [SerializeField] private Vector3 _skillVfxPositionOffset;
    [SerializeField] private Vector3 _skillVfxRotationOffset;
    [SerializeField] private float _skillVfxDestroyTime = 3f;
    [SerializeField] private float _skillVfxPlaySpeed = 0.5f;

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

    protected virtual bool CanUseSkill()
    {
        if (_target == null)
            return false;

        if (_target.IsDead)
            return false;

        // 첫 스킬은 따로 대기시간 체크
        if (!_hasUsedSkillOnce)
        {
            if (Time.time < _spawnTime + _firstSkillDelay)
                return false;
        }
        else
        {
            // 두 번째부터는 기존 스킬 쿨타임 사용
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
        _lastSkillTime = Time.time;
        _currentSkillLoop = 0;
        _hasUsedSkillOnce = true;

        SetMoveAnimation(false);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.ResetTrigger("Skill");
            _animator.SetTrigger("Skill");
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

            Vector3 dirToTarget = (unit.transform.position - center).normalized;
            dirToTarget.y = 0f;

            Vector3 flatForward = forward;
            flatForward.y = 0f;

            float targetAngle = Vector3.Angle(flatForward, dirToTarget);

            if (targetAngle <= angle * 0.5f)
            {
                targets.Add(unit);
            }
        }

        return targets;
    }

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

            unit.TakeDamage(_skillDamage, transform);

            if (_battleLog)
            {
                Debug.Log($"{name} >> {unit.name} 스킬 공격 / 데미지 : {_skillDamage}");
            }
        }
    }

    public virtual void EndSkillLoop()
    {
        _currentSkillLoop++;

        if (_currentSkillLoop >= _skillLoopCount)
        {
            _isUsingSkill = false;
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
}