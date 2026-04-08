using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoAttack : Unit
{
    public event Action<Unit> OnSkillUsed;

    [SerializeField] protected float _searchRadius;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField] protected float _skillMultiplier;

    [SerializeField] private float _attackDelay = 0.5f;

    private Animator _animator;
    private Collider _col;

    //private float _skillCool;
    //private float _currentSkillCool;

    private bool _skillEnd;

    protected Unit _targetUnit = null;

    protected string _currentSceneName;   // FieldScene이면 데미지 안 받게 처리

    // 반경 내에 다른 캐릭터가 있으면 (겹쳐있으면) 공격마다 안겹치도록 조금씩 이동
    private LayerMask _playerMask;
    [SerializeField] private float _playerSearchRadius = 1f;

    private float _checkTargetInterval = 1f;
    private float _checkTargetTimer = 0f;

    public void Init(PlayerRuntimeData data, int number)
    {
        _chName = data.ChName;
        _maxHp = data.DefaultMaxHp;
        _curHp = data.DefaultMaxHp;

        _atk = data.DefaultAtk;
        _def = data.DefaultDef;
        //_cp = _curHp + _atk + _def;
        _cp = CPCalculator.CalculateCP(_maxHp, _atk, _def);

        _range = data.Range;

        _moveSpeed = data.MoveSpeed;

        _maxSkillCool = data.SkillCool;
        _skillMultiplier = data.SkillMultiplier;

        _currentSkillCool = _maxSkillCool;

        _attackType = data.AttackType;

        _level = data.Level;
        _grade = data.Grade;

        _line = EPositionLine.None;
        _map = ECombatMap.Field;

        _targetTr = null;
        _searchRadius = 20f;

        _isAttack = false;
        _skillEnd = true;

        CharacterNumber = number;
        TotalDamage = 0;
        TotalDamaged = 0;
        IsSkillUsed = false;
    }

    public void Init(BaseStatus_SO data, int number)
    {
        _chName = data.ChName;
        _maxHp = data.DefaultMaxHp;
        _curHp = data.DefaultMaxHp;

        _atk = data.DefaultAtk;
        _def = data.DefaultDef;
        //_cp = _curHp + _atk + _def;
        _cp = CPCalculator.CalculateCP(_maxHp, _atk, _def);

        _range = data.Range;

        _moveSpeed = data.MoveSpeed;

        _maxSkillCool = data.SkillCool;
        _skillMultiplier = data.SkillMultiplier;

        _currentSkillCool = _maxSkillCool;

        _attackType = data.AttackType;

        _level = data.Level;
        _grade = data.Grade;

        _line = EPositionLine.None;
        _map = ECombatMap.Field;

        _targetTr = null;
        _searchRadius = 20f;

        _isAttack = false;
        _skillEnd = true;

        CharacterNumber = number;
        TotalDamage = 0;
        TotalDamaged = 0;
        IsSkillUsed = false;
    }

    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponent<Animator>();
        _col = GetComponent<Collider>();

        _playerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
    }

    //private void OnEnable()
    //{
    //    _currentSceneName = SceneManager.GetActiveScene().name;
    //}

    void Update()
    {
        if (_currentSceneName != ESceneId.FieldScene.ToString())
        {
            //Debug.Log($"{_currentSceneName} , {ESceneId.FieldScene.ToString()}");
            if (!GameManager.Instance.IsStageStart)
            {
                return;
            }
        }

        if (_isDead)
        {
            return;
        }

        _lifetime += Time.deltaTime;    // 전투 중 살아남은 시간

        _checkTargetTimer += Time.deltaTime;    // 일정주기로 타겟 탐색

        if (_skillEnd)
        {
            _currentSkillCool -= Time.deltaTime;
        }

        // 타겟이 NULL 이거나 오브젝트 비활성화면 타겟 탐색
        if (_targetTr == null || _targetTr.gameObject.activeSelf == false)
        {
            TryCheckTarget();
        }

        if (_targetTr != null)
        {
            // _targetUnit 캐싱
            if (_targetUnit == null)
            {
                _targetUnit = _targetTr.GetComponent<Unit>();
            }

            if (_targetUnit.IsDead)
            {
                _targetTr = null;
                _targetUnit = null;
                return;
            }

            // 타겟 방향으로 회전
            Vector3 dir = _targetTr.transform.position - transform.position;
            dir.y = 0f;

            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(dir);

            // 공격 사거리보다 멀리 있으면 이동
            if (_range < Vector3.Distance(transform.position, _targetTr.position))
            {
                transform.position += dir * _moveSpeed * Time.deltaTime;
                _animator.SetBool("Move", true);
            }

            // 사거리 안에 들어오면 공격
            else
            {
                _animator.SetBool("Move", false);

                // 이미 공격중이면 여기서 종료
                if (_isAttack)
                {
                    return;
                }

                // 필드 씬에서 스킬 자동 사용
                if (_currentSceneName == ESceneId.FieldScene.ToString())
                {
                    if (_currentSkillCool < 0f)
                    {
                        // 필드 씬에서는 자동으로 스킬 사용
                        _isAttack = true;
                        _skillEnd = false;
                        _animator.SetTrigger("Skill");
                        return;
                    }

                }

                // 배틀 씬에서 스킬 버튼 누르면 사용
                else
                {
                    if (_currentSkillCool < 0f && IsSkillUsed)
                    {
                        _currentSkillCool = 1234; // 스킬 사용 중에 스킬 버튼 막기. 스킬 끝나면 MaxCool로 세팅됨

                        IsSkillUsed = false;

                        _isAttack = true;
                        _skillEnd = false;
                        _animator.SetTrigger("Skill");
                        OnSkillUsed?.Invoke(this);
                        return;
                    }
                }

                _isAttack = true;
                _animator.SetTrigger("Attack");

            }
        }
    }

    private void TryCheckTarget()
    {
        if (_currentSceneName == ESceneId.FieldScene.ToString())
        {
            if (GameManager.Instance.IsFirstPoint)
            {
                // 필드 씬 시작 지점에서는 타겟 탐색 안함
                return;
            }
        }

        // 매 프레임 탐색하면 과부하
        // 일정 간격마다 탐색
        if (_checkTargetTimer < _checkTargetInterval)
        {
            return;
        }

        _checkTargetTimer = 0f;

        CheckTarget(_searchRadius, _layerMask);
        _animator.SetBool("Move", false);
    }

    private Vector3 CheckTarget(float radius, LayerMask mask)
    {
        //Debug.Log("CheckTarget");

        Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask);

        if (cols.Length <= 0)
        {
            return Vector3.zero;
        }

        float minDist = float.MaxValue;
        int idx = 0;

        // 제일 가까운 타겟 찾기
        for (int i = 0; i < cols.Length; i++)
        {
            if (transform.gameObject == cols[i].gameObject)
            {
                continue;
            }

            float dist = Vector3.Distance(transform.position, cols[i].transform.position);

            if (Vector3.Distance(transform.position, cols[i].transform.position) < minDist)
            {
                minDist = dist;
                idx = i;
            }
        }

        if (mask == _layerMask)
        {
            _targetTr = cols[idx].transform;
            //Debug.Log($"제일 가까운 타겟 : {_targetTr.name}");
        }

        else if (mask == _playerMask)
        {
            Vector3 v = transform.position - cols[idx].transform.position;
            v.y = 0f;
            return v.normalized;
        }

        return Vector3.zero;

    }

    public override void Attack()
    {
        Vector3 dir = CheckTarget(_playerSearchRadius, _playerMask);
        // 겹쳐진 캐릭터 반대방향 세팅
        if (dir != Vector3.zero)
        {
            //Debug.Log("살짝 이동");
            transform.position += dir * 0.2f;
        }
    }

    public void AttackEnd()
    {
        StartCoroutine(CoAttackDelay(_attackDelay));
    }

    private IEnumerator CoAttackDelay(float sec)
    {
        yield return new WaitForSeconds(sec);

        _isAttack = false;
    }

    private IEnumerator CoSkillDelay(float sec)
    {
        yield return new WaitForSeconds(sec);

        _isAttack = false;
        _currentSkillCool = _maxSkillCool;
        _skillEnd = true;   //스킬 쿨다운 시작
    }

    public override void Skill()
    {

    }

    public void SkillEnd()
    {
        StartCoroutine(CoSkillDelay(_attackDelay));
    }

    public override void TakeDamage(int damage, Transform attacker, bool isCritical = false)
    {
        if (_isDead)
        {
            return;
        }

        if (_currentSceneName == null)
        {
            _currentSceneName = SceneManager.GetActiveScene().name;
        }

        // 필드 씬이면 데미지 안 받음
        if (_currentSceneName == ESceneId.FieldScene.ToString())
        {
            return;
        }

        _curHp -= damage;
        _totalDamaged += damage;

        ShowDamageText(damage, attacker, isCritical);

        if (_curHp <= 0)
        {
            if (_isDead)
            {
                return;
            }

            _curHp = 0;
            Die();
        }
    }

    public override void Heal(int value)
    {
        _curHp += value;

        _curHp = Mathf.Clamp(_curHp, 0, _maxHp);
    }

    public override void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");
        _col.enabled = false;

        // 5초 뒤에 비활성화
        StartCoroutine(CoDie());
        //Destroy(gameObject, 5f);
    }

    private IEnumerator CoDie()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.forward * _range);
    }


}
