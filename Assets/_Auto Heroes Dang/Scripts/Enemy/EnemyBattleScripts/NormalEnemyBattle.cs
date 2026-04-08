using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float _hitVfxForwardOffset = 0.5f;
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
    [SerializeField] protected float _attackRange = 1.5f;
    [SerializeField] protected float _skillCool;
    [SerializeField] protected float _skillMultiplier = 1f;

    [Header("전투 슬롯 이동")]
    [SerializeField] protected bool _useCombatSlotMove = true;
    [SerializeField] protected int _slotCount = 6;
    [SerializeField] protected float _slotRadiusOffset = 0.2f;
    [SerializeField] protected float _slotArriveDistance = 0.3f;

    [SerializeField] private string[] _attackSfxNames = { "MonsterAttack1", "MonsterAttack2", "MonsterAttack3" };

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

    protected Coroutine _defBuffCoroutine;
    protected int _currentDefBuffAmount;

    protected Coroutine _atkBuffCoroutine;
    protected int _currentAtkBuffAmount;

    protected int _baseAtk;
    protected int _baseDef;

    public Unit Target => _target;
    public int Def => _def;
    public int Atk => _atk;
    public float SkillMultiplier => _skillMultiplier;

    private string _currentSceneName;
    protected override void Awake()
    {
        base.Awake();

        if (_animator == null)
            _animator = GetComponent<Animator>();

        if (_targetLayer == 0)
            _targetLayer = LayerMask.GetMask("Player");

        ApplyStatusData();
    }
    protected virtual void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
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

        _baseAtk = _statusData.DefaultAtk;
        _baseDef = _statusData.DefaultDef;

        _atk = _baseAtk;
        _def = _baseDef;

        _attackRange = _statusData.Range;
        _moveSpeed = _statusData.MoveSpeed;
        _skillCool = _statusData.SkillCool;
        _skillMultiplier = _statusData.SkillMultiplier;
        _attackType = _statusData.AttackType;
    }

    protected virtual void Update()
    {
        if (_currentSceneName != ESceneId.FieldScene.ToString())
        {

            if (!GameManager.Instance.IsStageStart)
            {
                return;
            }
        }

        if (_isDead)
            return;

        UpdateRotation();
        UpdateTarget();
        HandleCombat();
    }

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

        float targetDistance = GetDistanceTo(_target);
        if (targetDistance <= _attackRange + _stopDistanceOffset)
        {
            SetMoveAnimation(false);
            return;
        }

        Vector3 moveTargetPos = GetMoveTargetPosition();

        // 슬롯을 못 받았으면 잠깐 대기
        if (moveTargetPos == Vector3.zero)
        {
            SetMoveAnimation(false);
            return;
        }

        Vector3 myPos = transform.position;
        moveTargetPos.y = myPos.y;

        Vector3 direction = moveTargetPos - myPos;
        float slotDistance = direction.magnitude;

        if (slotDistance <= _slotArriveDistance)
        {
            // 슬롯에는 도착했지만 아직 공격 사거리 아니면 조금 더 접근할 수도 있음
            if (targetDistance <= _attackRange + _stopDistanceOffset)
            {
                SetMoveAnimation(false);
                return;
            }

            direction = _target.transform.position - myPos;
            direction.y = 0f;
        }

        if (direction == Vector3.zero)
        {
            SetMoveAnimation(false);
            return;
        }

        direction.Normalize();

        transform.position += direction * _moveSpeed * Time.deltaTime;
        transform.forward = direction;
        SetMoveAnimation(true);
    }

    protected virtual Vector3 GetMoveTargetPosition()
    {
        if (_target == null)
            return transform.position;

        if (!_useCombatSlotMove)
            return _target.transform.position;

        float slotRadius = _attackRange + _slotRadiusOffset;
        return CombatSlotManager.GetSlotPosition(_target.transform, transform, _slotCount, slotRadius);
    }

    protected virtual void ReleaseCombatSlot()
    {
        if (_target != null)
        {
            CombatSlotManager.ReleaseSlot(_target.transform, transform);
        }
        else
        {
            CombatSlotManager.ReleaseAllSlotsForRequester(transform);
        }
    }

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
                ReleaseCombatSlot();
                _target = null;
                needNewTarget = true;
            }
        }

        if (!needNewTarget && _searchTimer < _searchInterval)
            return;

        if (_searchTimer < _searchInterval)
            return;

        _searchTimer = 0f;

        Unit newTarget = FindEnemyInRange();

        if (_target != newTarget)
        {
            ReleaseCombatSlot();
            _target = newTarget;
        }
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

        FaceTargetImmediately(_target);

        // 공격 직전에 실제 타겟을 바라보게
        Vector3 lookDir = _target.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir != Vector3.zero)
        {
            transform.forward = lookDir.normalized;
        }

        _lockedAttackTarget = _target;
        _isAttacking = true;

        SetMoveAnimation(false);

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("Attack");
        }
    }

    // 공격 사운드 3개 중 하나를 랜덤으로 재생하는 함수
    private void PlayRandomAttackSfx()
    {
        if (_attackSfxNames == null || _attackSfxNames.Length == 0)
            return;

        int randomIndex = Random.Range(0, _attackSfxNames.Length);
        AudioManager.Instance.PlaySFX(_attackSfxNames[randomIndex]);
    }

    protected virtual void FaceTargetImmediately(Unit target)
    {
        if (target == null)
            return;

        Vector3 lookDir = target.transform.position - transform.position;
        lookDir.y = 0f;

        if (lookDir == Vector3.zero)
            return;

        transform.forward = lookDir.normalized;
    }

    protected virtual void ApplyDamage()
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

        _totalDamage += finalDamage;

        PlayRandomAttackSfx();
        SpawnMeleeHitVfx();

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_lockedAttackTarget.name} 공격 / 최종 데미지 : {finalDamage}");
        }
    }

    private void EndAttack()
    {
        _isAttacking = false;
        _lockedAttackTarget = null;
        _lastAttackTime = Time.time;
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

    public override void TakeDamage(int damage, Transform attacker, bool isCritical = false)
    {
        if (_isDead)
            return;

        int finalDamage = Mathf.Max(1, damage);
        _curHp -= finalDamage;
        _totalDamaged += finalDamage;

        ShowDamageText(finalDamage, attacker, isCritical);
        AudioManager.Instance.PlaySFX("HitSound1");

        if (_battleLog)
        {
            string attackerName = attacker != null ? attacker.name : "Unknown";
            Debug.Log($"{name} 피해 받음 / 공격자 : {attackerName} / 피해량 : {finalDamage} / 크리티컬 : {isCritical} / 남은 HP : {_curHp}");
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

        ReleaseCombatSlot();

        _isDead = true;
        _isAttacking = false;
        _isRotating = false;
        _lockedAttackTarget = null;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        SetMoveAnimation(false);

        DisableBuffVfx("DefBuff");
        DisableBuffVfx("AttackBuff");

        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            _animator.SetTrigger("Die");
        }

        if (_deathLog)
        {
            Debug.Log($"{name} 죽음");
        }

        EnemyReward reward = GetComponent<EnemyReward>();
        if (reward != null)
            reward.GiveReward();

        EnemyChestReward chestReward = GetComponent<EnemyChestReward>();
        if (chestReward != null)
            chestReward.TryDropChest();

        Destroy(gameObject, 3f);
    }

    private void DisableBuffVfx(string childName)
    {
        Transform[] children = GetComponentsInChildren<Transform>(true);

        bool found = false;

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains(childName))
            {
                children[i].gameObject.SetActive(false);
                found = true;
            }
        }

        if (!found)
        {
            // Debug.LogWarning($"{name} : {childName} 못 찾음");
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

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

    public void ApplyDefenseBuff(int amount, float duration)
    {
        if (_isDead)
            return;

        if (_defBuffCoroutine != null)
        {
            StopCoroutine(_defBuffCoroutine);
            _def -= _currentDefBuffAmount;
            _currentDefBuffAmount = 0;
        }

        _def += amount;
        _currentDefBuffAmount = amount;

        _defBuffCoroutine = StartCoroutine(Co_DefenseBuff(duration));
    }

    private IEnumerator Co_DefenseBuff(float duration)
    {
        yield return new WaitForSeconds(duration);

        _def -= _currentDefBuffAmount;
        _currentDefBuffAmount = 0;
        _defBuffCoroutine = null;
    }

    public int GetCurrentDef()
    {
        return _def;
    }

    public void ApplyAttackBuff(int amount, float duration)
    {
        if (_isDead)
            return;

        if (_atkBuffCoroutine != null)
        {
            StopCoroutine(_atkBuffCoroutine);
            _atk -= _currentAtkBuffAmount;
            _currentAtkBuffAmount = 0;
        }

        _atk += amount;
        _currentAtkBuffAmount = amount;

        _atkBuffCoroutine = StartCoroutine(Co_AttackBuff(duration));
    }

    private IEnumerator Co_AttackBuff(float duration)
    {
        yield return new WaitForSeconds(duration);

        _atk -= _currentAtkBuffAmount;
        _currentAtkBuffAmount = 0;
        _atkBuffCoroutine = null;
    }

    public void ApplyAttackBuffPercent(float percent, float duration)
    {
        if (_isDead)
            return;

        if (_atkBuffCoroutine != null)
        {
            StopCoroutine(_atkBuffCoroutine);
            _atk -= _currentAtkBuffAmount;
            _currentAtkBuffAmount = 0;
        }

        int buffAmount = Mathf.Max(1, Mathf.RoundToInt(_baseAtk * percent));

        _atk += buffAmount;
        _currentAtkBuffAmount = buffAmount;

        _atkBuffCoroutine = StartCoroutine(Co_AttackBuff(duration));
    }

    public void ApplyDefenseBuffPercent(float percent, float duration)
    {
        if (_isDead)
            return;

        if (_defBuffCoroutine != null)
        {
            StopCoroutine(_defBuffCoroutine);
            _def -= _currentDefBuffAmount;
            _currentDefBuffAmount = 0;
        }

        int buffAmount = Mathf.Max(1, Mathf.RoundToInt(_baseDef * percent));

        _def += buffAmount;
        _currentDefBuffAmount = buffAmount;

        _defBuffCoroutine = StartCoroutine(Co_DefenseBuff(duration));
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