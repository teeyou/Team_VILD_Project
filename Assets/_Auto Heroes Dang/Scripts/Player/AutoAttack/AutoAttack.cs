using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : Unit
{
    [SerializeField] protected float _searchRadius;
    [SerializeField] protected LayerMask _layerMask;
    [SerializeField] protected float _skillMultiplier = 1.5f;

    [SerializeField] private float _attackDelay = 0.5f;

    private Animator _animator;
    private Collider _col;

    private float _skillCool;
    private float _currentSkillCool;

    private bool _skillEnd;

    protected Unit _targetUnit = null;

    public void Init(BaseStatus_SO data)
    {
        _maxHp = data.DefaultMaxHp;
        _curHp = data.DefaultMaxHp;

        _atk = data.DefaultAtk;
        _def = data.DefaultDef;
        _cp = _curHp + _atk + _def;

        _range = data.Range;

        _moveSpeed = data.MoveSpeed;

        _skillCool = data.SkillCool;
        _currentSkillCool = _skillCool;

        _attackType = data.AttackType;

        _level = data.Level;
        _grade = data.Grade;

        _line = EPositionLine.None;
        _map = ECombatMap.Field;

        _targetTr = null;
        _searchRadius = 20f;

        _isAttack = false;
        _skillEnd = true;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _col = GetComponent<Collider>();
    }

    private void Start()
    {

    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        if (_skillEnd)
            _currentSkillCool -= Time.deltaTime;

        // 타겟이 NULL 이거나 오브젝트 비활성화면 타겟 탐색
        if (_targetTr == null || !_targetTr.gameObject.activeSelf)
        {
            CheckTarget();
            _animator.SetBool("Move", false);
        }

        if (_targetTr != null)
        {
            // _targetUnit 캐싱
            if (_targetUnit == null)
            {
                Debug.Log("_targetUnit 최초 캐싱");
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

                if (_isAttack)
                {
                    return;
                }

                
                if (_currentSkillCool < 0f)
                {
                    _isAttack = true;
                    _skillEnd = false;
                    _animator.SetTrigger("Skill");
                }

                else
                {
                    _isAttack = true;
                    _animator.SetTrigger("Attack");
                }
            }
        }
    }

    private void CheckTarget()
    {
        Debug.Log("CheckTarget");

        Collider[] cols = Physics.OverlapSphere(transform.position, _searchRadius, _layerMask);

        if (cols.Length <= 0)
        {
            return;
        }

        float minDist = float.MaxValue;
        int idx = 0;

        // 제일 가까운 타겟 찾기
        for (int i = 0; i < cols.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, cols[i].transform.position);

            if (Vector3.Distance(transform.position, cols[i].transform.position) < minDist)
            {
                minDist = dist;
                idx = i;
            }
        }

        _targetTr = cols[idx].transform;
        Debug.Log($"제일 가까운 타겟 : {_targetTr.name}");
    }

    public override void Attack()
    {
        Debug.Log("Attack");
    }

    public void AttackEnd()
    {
        Debug.Log("AttackEnd");
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
        _currentSkillCool = _skillCool;
        _skillEnd = true;   //스킬 쿨다운 시작
    }

    public override void Skill()
    {
        Debug.Log("Skill");
    }

    public void SkillEnd()
    {
        Debug.Log("SkillEnd");
        StartCoroutine(CoSkillDelay(_attackDelay));
    }

    public override void TakeDamage(int damage, Transform target)
    {
        if (_isDead)
        {
            return;
        }

        Debug.Log($"{target.name} 로부터 데미지 {damage} 받음");

        _curHp -= damage;
        Debug.Log($"현재 HP : {_curHp}");
        
        //_animator.SetTrigger("Hit");

        if (_curHp <= 0)
        {
            if (_isDead)
            {
                return;
            }

            Die();
        }
    }
    public override void Heal(int value)
    {
        Debug.Log($"{transform.name} Heal - {value}");
        _curHp += value;

        _curHp = Mathf.Clamp(_curHp, 0, _maxHp);
    }

    public override void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");
        _col.enabled = false;

        // 5초 뒤에 파괴
        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.forward * _range);
    }


}
