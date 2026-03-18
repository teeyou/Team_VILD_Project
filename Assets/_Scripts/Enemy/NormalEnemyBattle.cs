using UnityEngine;

public class NormalEnemyBattle : Unit
{
    [Header("타겟 레이어")]
    [SerializeField] private LayerMask _targetLayer;

    [Header("기본 스탯")]
    [SerializeField] private int _defaultMaxHp = 100;
    [SerializeField] private int _defaultAtk = 10;
    [SerializeField] private int _defaultDef = 1;

    [Header("자동 전투")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _detectRange = 10f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _searchInterval = 0.2f;
    [SerializeField] private float _stopDistanceOffset = 0.5f;

    [Header("디버그")]
    [SerializeField] private bool _battleLog = true;
    [SerializeField] private bool _deathLog = true;
    [SerializeField] private bool _drawGizmos = true;

    //protected bool _isDead;
    protected float _searchTimer;
    protected float _lastAttackTime = -999f;

    protected Unit _target;

    //public bool IsDead => _isDead;
    public Unit Target => _target;

    protected virtual void Awake()
    {
        _targetLayer = LayerMask.GetMask("Player");
    }

    // 시작 시 스탯 초기화
    protected virtual void Start()
    {
        _maxHp = _defaultMaxHp;
        _curHp = _maxHp;
        _atk = _defaultAtk;
        _def = _defaultDef;
    }

    // 매 프레임 타겟 갱신과 전투 행동 처리
    protected virtual void Update()
    {
        if (_isDead)
            return;

        UpdateTarget();
        HandleCombat();
    }

    // 현재 타겟과의 거리를 기준으로 이동 또는 공격을 결정
    protected virtual void HandleCombat()
    {
        if (_target == null)
            return;

        float distance = GetDistanceTo(_target);

        if (distance > _attackRange + _stopDistanceOffset)
        {
            MoveToTarget();
        }
        else
        {
            Attack();
        }
    }

    // 현재 타겟 방향으로 이동
    protected virtual void MoveToTarget()
    {
        if (_target == null)
            return;

        float distance = GetDistanceTo(_target);
        if (distance <= _attackRange + _stopDistanceOffset)
            return;

        Vector3 targetPos = _target.transform.position;
        Vector3 myPos = transform.position;

        targetPos.y = myPos.y;

        Vector3 direction = (targetPos - myPos).normalized;

        transform.position += direction * _moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    // 현재 타겟이 유효한지 확인하고 필요하면 새 타겟 탐색
    protected virtual void UpdateTarget()
    {
        _searchTimer += Time.deltaTime;

        bool needNewTarget = _target == null;

        if (!needNewTarget && _target != null)
        {
            NormalEnemyBattle enemy = _target.GetComponent<NormalEnemyBattle>();
            bool targetDead = false;

            if (enemy != null)
                targetDead = enemy.IsDead;

            if (targetDead)
            {
                _target = null;
                needNewTarget = true;
            }
            else
            {
                float distance = GetDistanceTo(_target);

                if (distance > _detectRange)
                {
                    _target = null;
                    needNewTarget = true;
                }
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

            NormalEnemyBattle enemy = other.GetComponent<NormalEnemyBattle>();
            bool otherDead = false;

            if (enemy != null)
                otherDead = enemy.IsDead;

            if (otherDead)
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

    // 공격 가능 조건을 확인한 뒤 타겟에게 데미지 전달
    public override void Attack()
    {
        if (_target == null)
            return;

        NormalEnemyBattle enemy = _target.GetComponent<NormalEnemyBattle>();
        bool targetDead = false;

        if (enemy != null)
            targetDead = enemy.IsDead;

        if (targetDead)
            return;

        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        float distance = GetDistanceTo(_target);

        if (distance > _attackRange + _stopDistanceOffset)
            return;

        _lastAttackTime = Time.time;

        _target.TakeDamage(_atk, transform);
        _totalDamage += _atk;

        if (_battleLog)
        {
            Debug.Log($"{name} >> {_target.name} 공격 / 데미지 : {_atk}");
        }
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

    // 사망 처리 후 오브젝트 제거
    public override void Die()
    {
        if (_isDead)
            return;

        _isDead = true;

        if (_deathLog)
        {
            Debug.Log($"{name} 죽음");
        }

        Destroy(gameObject);
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