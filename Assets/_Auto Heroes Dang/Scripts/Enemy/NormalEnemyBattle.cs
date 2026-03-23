using UnityEngine;

public enum EnemyState
{
    Idle,
    Rotate,
    Move,
    Attack,
    Die
}

public class NormalEnemyBattle : Unit
{
    [Header("애니메이터 연결")]
    [SerializeField] protected Animator _animator;

    [Header("스탯 데이터")]
    [SerializeField] protected BaseStatus_SO _statusData;

    [Header("타겟 레이어")]
    [SerializeField] protected LayerMask _targetLayer;

    [Header("자동 전투")]
    [SerializeField] protected float _detectRange = 10f;
    [SerializeField] protected float _attackCooldown = 1f;
    [SerializeField] protected float _searchInterval = 0.2f;
    [SerializeField] protected float _stopDistanceOffset = 0.5f;
    [SerializeField] protected float _postAttackDelay = 0.2f;

    [Header("히트 VFX")]
    [SerializeField] private GameObject _hitVfxPrefab;
    [SerializeField] private float _hitVfxForwardOffset = 1.0f;
    [SerializeField] private float _hitVfxHeightOffset = 1.0f;
    [SerializeField] private float _hitVfxLifeTime = 0.2f;
    [SerializeField] private Vector3 _meleeHitVfxScale = Vector3.one;

    [Header("원거리 공격")]
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected Transform _projectileSpawnPoint;
    [SerializeField] protected float _projectileSpawnForwardOffset = 0.5f;
    [SerializeField] protected float _projectileSpeed = 8f;

    [Header("회전 설정")]
    [SerializeField] protected float _rotateDuration = 0.2f;

    [Header("디버그")]
    [SerializeField] protected bool _battleLog = true;
    [SerializeField] protected bool _deathLog = true;
    [SerializeField] protected bool _drawGizmos = true;

    [Header("런타임 확인용")]
    //[SerializeField] protected float _moveSpeed = 2f;
    [SerializeField] protected float _attackRange = 1.5f;
    [SerializeField] protected float _skillCool;
    //[SerializeField] protected EAttackType _attackType;

    protected float _searchTimer;
    protected float _lastAttackTime = -999f;
    protected float _nextActionTime;

    protected Unit _target;
    protected Unit _lockedAttackTarget;

    protected bool _isAttacking;

    protected bool _isRotating;
    protected Quaternion _startRotation;
    protected Quaternion _targetRotation;
    protected float _rotateTimer;
    protected Unit _lastLookTarget;

    public Unit Target => _target;

    // 시작 시 Animator 자동 연결 및 SO 데이터 적용
    protected virtual void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_targetLayer == 0)
            _targetLayer = LayerMask.GetMask("Player");

        ApplyStatusData();
    }

    // SO 값을 실제 전투 변수에 적용
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

    // 매 프레임 회전, 타겟 갱신, 전투 행동 처리
    protected virtual void Update()
    {
        if (_isDead)
            return;

        UpdateRotation();
        UpdateTarget();
        HandleCombat();
    }

    // 현재 타겟과의 거리를 기준으로 이동 또는 공격을 결정
    protected virtual void HandleCombat()
    {
        if (_isDead)
            return;

        if (Time.time < _nextActionTime)
        {
            SetMoveAnimation(false);
            return;
        }

        if (_target == null)
        {
            SetMoveAnimation(false);
            return;
        }

        if (_isAttacking)
        {
            SetMoveAnimation(false);
            return;
        }

        if (_isRotating)
        {
            SetMoveAnimation(false);
            return;
        }

        // 새 타겟이면 먼저 방향부터 맞춤
        if (_lastLookTarget != _target)
        {
            _lastLookTarget = _target;
            StartRotateToTarget();
            return;
        }

        float distance = GetDistanceTo(_target);

        if (distance > _attackRange + _stopDistanceOffset)
        {
            MoveToTarget();
        }
        else
        {
            SetMoveAnimation(false);
            Attack();
        }
    }

    // 현재 타겟 방향으로 이동
    protected virtual void MoveToTarget()
    {
        if (_target == null)
            return;

        if (_isAttacking)
            return;

        if (_isDead)
            return;

        if (_isRotating)
            return;

        if (Time.time < _nextActionTime)
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

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }

        SetMoveAnimation(true);
    }

    // 공격 중/행동 대기 중에는 타겟 갱신을 잠깐 막음
    protected virtual void UpdateTarget()
    {
        if (_isAttacking)
            return;

        if (_isRotating)
            return;

        if (Time.time < _nextActionTime)
            return;

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

    // 다른 유닛과의 평면 거리 계산
    protected virtual float GetDistanceTo(Unit other)
    {
        Vector3 a = transform.position;
        Vector3 b = other.transform.position;

        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }

    // 감지 범위 안에서 타겟 레이어에 해당하는 가장 가까운 유닛 탐색
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

    // 공격 가능한 조건을 통과하면 Attack 애니메이션 실행
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

        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        float distance = GetDistanceTo(_target);
        if (distance > _attackRange + _stopDistanceOffset)
            return;

        // 공격 시작 시점의 타겟을 잠금
        _lockedAttackTarget = _target;

        _lastAttackTime = Time.time;
        _isAttacking = true;

        SetMoveAnimation(false);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetTrigger("Attack");
        }
    }

    // Attack 애니메이션 이벤트에서 호출되어 실제 데미지 적용
    private void ApplyDamage()
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

        _lockedAttackTarget.TakeDamage(_atk, transform);
        _totalDamage += _atk;

        SpawnMeleeHitVfx();

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_lockedAttackTarget.name} 공격 / 데미지 : {_atk}");
        }
    }

    // Attack 애니메이션 마지막 이벤트에서 호출
    private void EndAttack()
    {
        _isAttacking = false;
        _lockedAttackTarget = null;
        _nextActionTime = Time.time + _postAttackDelay;
    }

    public override void Skill()
    {
    }

    public override void Heal(int value)
    {
        if (_isDead)
            return;

        _curHp += value;

        if (_curHp > _maxHp)
            _curHp = _maxHp;
    }

    // 데미지를 받아 체력을 감소시키고 사망 여부 확인
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

    // 사망 처리 후 Die 애니메이션 재생
    public override void Die()
    {
        if (_isDead)
            return;

        _isDead = true;
        _isAttacking = false;
        _isRotating = false;
        _lockedAttackTarget = null;

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

    // Die 애니메이션 마지막 이벤트에서 호출
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    // 새 타겟을 향해 잠깐 멈춘 뒤 회전 시작
    protected virtual void StartRotateToTarget()
    {
        if (_target == null)
            return;

        Vector3 targetPos = _target.transform.position;
        Vector3 myPos = transform.position;

        targetPos.y = myPos.y;
        Vector3 direction = (targetPos - myPos).normalized;

        if (direction == Vector3.zero)
            return;

        _isRotating = true;
        _rotateTimer = 0f;
        _startRotation = transform.rotation;
        _targetRotation = Quaternion.LookRotation(direction);

        SetMoveAnimation(false);
    }

    // 회전 처리
    protected virtual void UpdateRotation()
    {
        if (!_isRotating)
            return;

        _rotateTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_rotateTimer / _rotateDuration);

        transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);

        if (t >= 1f)
        {
            _isRotating = false;
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

        Collider targetCollider = _target.GetComponent<Collider>();
        if (targetCollider == null)
        {
            Debug.LogWarning($"{name} : 타겟에 Collider가 없습니다.");
            return;
        }

        Vector3 spawnPos = GetProjectileSpawnPosition();

        GameObject projectileObj = Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);

        HomingProjectile projectile = projectileObj.GetComponent<HomingProjectile>();
        if (projectile == null)
        {
            Debug.LogWarning($"{name} : 투사체 프리팹에 HomingProjectile 스크립트가 없습니다.");
            return;
        }

        projectile.Init(_target, _atk, _projectileSpeed, transform, targetCollider);

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_target.name} 투사체 발사 / 데미지 : {_atk}");
        }
    }

    // Move bool 제어용 공통 함수
    protected virtual void SetMoveAnimation(bool isMove)
    {
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetBool("Move", isMove);
        }
    }

    private void SpawnMeleeHitVfx()
    {
        if (_hitVfxPrefab == null)
            return;

        Vector3 hitPos = transform.position + transform.forward * (_hitVfxForwardOffset + _attackRange);



        if (_target != null)
        {
            hitPos.y = _target.transform.position.y + _hitVfxHeightOffset;
        }
        else
        {
            hitPos.y = transform.position.y + _hitVfxHeightOffset;
        }

        GameObject hitVfx = Instantiate(_hitVfxPrefab, hitPos, Quaternion.identity);
        hitVfx.transform.localScale = _meleeHitVfxScale;
        Destroy(hitVfx, _hitVfxLifeTime);
    }

    // 감지 범위와 공격 범위를 Scene 뷰에 표시
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