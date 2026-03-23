using UnityEngine;

public class NamedEnemyBattle : NormalEnemyBattle
{
    [Header("스킬 설정")]
    [SerializeField] protected float _skillCooldown = 5f;
    [SerializeField] protected float _skillRange = 3f;
    [SerializeField] protected int _skillDamage = 50;
    [SerializeField] protected int _skillLoopCount = 1;

    protected float _lastSkillTime = -999f;
    protected bool _isUsingSkill;
    protected int _currentSkillLoop;

    protected override void HandleCombat()
    {
        if (_isDead)
            return;

        if (_target == null)
            return;

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

        if (Time.time < _lastSkillTime + _skillCooldown)
            return false;

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

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Move", false);
            _animator.SetTrigger("Skill");
        }
    }

    // 스킬 타격 시점 이벤트
    public virtual void ApplySkillDamage()
    {
        if (_target == null)
            return;

        if (_target.IsDead)
            return;

        float distance = GetDistanceTo(_target);
        if (distance > _skillRange)
            return;

        _target.TakeDamage(_skillDamage, transform);

        Debug.Log($"{name} >> {_target.name} 스킬 공격 / 데미지 : {_skillDamage}");
    }

    // 스킬 애니메이션 마지막 프레임 이벤트
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
            _animator.SetTrigger("Skill");
        }
    }
}