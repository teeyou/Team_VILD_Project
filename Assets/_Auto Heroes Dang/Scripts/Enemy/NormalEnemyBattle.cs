using UnityEngine;

public class NormalEnemyBattle : Unit
{
    [Header("애니메이터 연결")]
    [SerializeField] protected Animator _animator;

    [Header("스탯 데이터")]
    [SerializeField] protected BaseStatus_SO _statusData;

    [Header("타겟 레이어")]
    [SerializeField] private LayerMask _targetLayer;

    [Header("자동 전투")]
    [SerializeField] private float _detectRange = 10f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _searchInterval = 0.2f;
    [SerializeField] private float _stopDistanceOffset = 0.5f;

    [Header("원거리 공격")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileSpawnForwardOffset = 0.5f;
    [SerializeField] private float _projectileSpeed = 8f;
    [SerializeField] private Transform _projectileSpawnPoint;

    [Header("디버그")]
    [SerializeField] private bool _battleLog = true;
    [SerializeField] private bool _deathLog = true;
    [SerializeField] private bool _drawGizmos = true;

    [Header("런타임 확인용")]
    //[SerializeField] protected float _moveSpeed = 2f;
    [SerializeField] protected float _attackRange = 1.5f;
    [SerializeField] protected float _skillCool;
    //[SerializeField] protected EAttackType _attackType;

    protected float _searchTimer;
    protected float _lastAttackTime = -999f;

    protected Unit _target;
    protected bool _isAttacking;

    public Unit Target => _target;

    protected virtual void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_targetLayer == 0)
            _targetLayer = LayerMask.GetMask("Player");

        ApplyStatusData();
    }

    protected virtual void ApplyStatusData()
    {
        if (_statusData == null)
        {
            Debug.LogWarning($"{name} : BaseStatus_SO가 연결되지 않았습니다.");
            return;
        }

        _maxHp = _statusData.DefaultMaxHp;
        _curHp = _maxHp;
        _atk = _statusData.DefaultAtk;
        _def = _statusData.DefaultDef;

        _attackRange = _statusData.Range;
        _moveSpeed = _statusData.MoveSpeed;
        _skillCool = _statusData.SkillCool;
        _attackType = _statusData.AttackType;
    }

    protected virtual void Update()
    {
        if (_isDead)
            return;

        UpdateTarget();
        HandleCombat();
    }

    protected virtual void HandleCombat()
    {
        if (_isDead)
            return;

        if (_isAttacking)
            return;

        if (_target == null)
        {
            SetMoveAnimation(false);
            return;
        }

        float distance = GetDistanceTo(_target);

        if (distance > _attackRange + _stopDistanceOffset)
        {
            MoveToTarget();
        }
        else
        {
            LookAtTarget();
            SetMoveAnimation(false);
            if (!_isAttacking)
            {
                Attack();
            }
        }
    }


    protected virtual void MoveToTarget()
    {
        if (_target == null)
            return;

        if (_isAttacking)
            return;

        if (_isDead)
            return;

        float distance = GetDistanceTo(_target);
        if (distance <= _attackRange + _stopDistanceOffset)
        {
            SetMoveAnimation(false);
            return;
        }

        Vector3 targetPos = _target.transform.position;
        Vector3 myPos = transform.position;

        targetPos.y = myPos.y;

        Vector3 direction = (targetPos - myPos).normalized;

        transform.position += direction * _moveSpeed * Time.deltaTime;

        LookAtTarget();

        SetMoveAnimation(true);
    }

    protected virtual void UpdateTarget()
    {
        _searchTimer += Time.deltaTime;

        bool needNewTarget = _target == null || _target.IsDead;

        if (!needNewTarget && _target != null)
        {
            float distance = GetDistanceTo(_target);

            if (distance > _detectRange)
            {
                _target = null;
                needNewTarget = true;
            }
        }

        if (!needNewTarget && _searchTimer < _searchInterval)
            return;

        if (_searchTimer < _searchInterval)
            return;

        _searchTimer = 0f;
        _target = FindEnemyInRange();
    }

    protected virtual float GetDistanceTo(Unit other)
    {
        Vector3 a = transform.position;
        Vector3 b = other.transform.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    protected virtual Unit FindEnemyInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectRange, _targetLayer);

        Unit closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < hits.Length; i++)
        {
            Unit other = hits[i].GetComponent<Unit>();

            if (other == null)
                continue;
            if (other == this)
                continue;
            if (other.IsDead)
                continue;

            float distance = GetDistanceTo(other);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = other;
            }
        }

        return closestEnemy;
    }

    protected virtual void LookAtTarget()
    {
        if (_target == null)
            return;

        Vector3 targetPos = _target.transform.position;
        Vector3 myPos = transform.position;

        targetPos.y = myPos.y;

        Vector3 direction = (targetPos - myPos).normalized;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    public override void Attack()
    {
        if (_target == null)
            return;

        if (_target.IsDead)
            return;

        if (_isAttacking)
            return;

        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        float distance = GetDistanceTo(_target);
        if (distance > _attackRange + _stopDistanceOffset)
            return;

        _lastAttackTime = Time.time;
        _isAttacking = true;

        SetMoveAnimation(false);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetTrigger("Attack");
        }
    }

    // 근접 공격용 이벤트
    private void ApplyDamage()
    {
        if (_isDead)
            return;
        if (_target == null)
            return;
        if (_target.IsDead)
            return;

        float distance = GetDistanceTo(_target);
        if (distance > _attackRange + _stopDistanceOffset)
            return;

        _target.TakeDamage(_atk, transform);
        _totalDamage += _atk;

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_target.name} 근접 공격 / 데미지 : {_atk}");
        }
    }

    // 원거리 공격용 이벤트
    private void FireProjectile()
    {
        if (_isDead)
            return;

        if (_target == null)
            return;

        if (_target.IsDead)
            return;

        if (_projectilePrefab == null)
        {
            Debug.LogWarning($"{name} : 투사체 프리팹이 연결되지 않았습니다.");
            return;
        }

        Collider _targetCollider = _target.GetComponent<Collider>();
        if (_targetCollider == null)
        {
            Debug.LogWarning($"{name} : 타겟에 콜라이더가 없음");
            return;
        }

        // Vector3 spawnPos = transform.position + transform.forward * _projectileSpawnForwardOffset;
        // spawnPos.y = transform.position.y;

        Vector3 spawnPos = GetProjectileSpawnPosition();

        GameObject projectileObj = Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);

        HomingProjectile projectile = projectileObj.GetComponent<HomingProjectile>();
        if (projectile == null)
        {
            Debug.LogWarning($"{name} : 투사체 프리팹에 HomingProjectile 스크립트가 없습니다.");
            return;
        }

        projectile.Init(_target, _atk, _projectileSpeed, transform, _targetCollider);

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_target.name} 투사체 발사 / 데미지 : {_atk}");
        }
    }

    protected virtual Vector3 GetProjectileSpawnPosition()
    {
        if (_projectileSpawnPoint != null)
            return _projectileSpawnPoint.position;

        Vector3 spawnPos = transform.position + transform.forward * _projectileSpawnForwardOffset;
        spawnPos.y = transform.position.y;
        return spawnPos;
    }

    private void EndAttack()
    {
        _isAttacking = false;
    }

    public override void Skill() { }

    public override void Heal(int value)
    {
        if (_isDead)
            return;

        _curHp += value;

        if (_curHp > _maxHp)
            _curHp = _maxHp;
    }

    public override void TakeDamage(int damage, Transform attacker)
    {
        if (_isDead)
            return;

        int finalDamage = Mathf.Max(1, damage - _def);
        _curHp -= finalDamage;
        _totalDamaged += finalDamage;

        if (_battleLog)
        {
            string attackerName = attacker != null ? attacker.name : "Unknown";
            Debug.Log($"{name} 피해 받음 / 공격자 : {attackerName} / 피해량 : {finalDamage} / 남은 HP : {_curHp}");
        }

        if (_curHp <= 0)
        {
            _curHp = 0;
            Die();
        }
    }

    public override void Die()
    {
        if (_isDead)
            return;

        _isDead = true;
        _isAttacking = false;

        SetMoveAnimation(false);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetTrigger("Die");
        }

        if (_deathLog)
        {
            Debug.Log($"{name} 죽음");
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    protected virtual void SetMoveAnimation(bool isMove)
    {
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Move", isMove);
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (!_drawGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}