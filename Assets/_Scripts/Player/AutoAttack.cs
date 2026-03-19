using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : Unit
{
    [SerializeField] private float _searchRadius;
    [SerializeField] private LayerMask _layerMask;
    
    private Animator _animator;

    private float _skillCool;
    private float _currentSkillCool;

    private bool _skillEnd;
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
    }

    void Update()
    {
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
                    Debug.Log("이미 공격중");
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
            return;

        float minDist = float.MaxValue;
        int idx = 0;

        // 제일 가까운 타겟 찾기
        for (int i = 0; i < cols.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, cols[i].transform.position);

            if (Vector3.Distance(transform.position, cols[i].transform.position) < dist)
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

        if (_attackType == EAttackType.Melee)
        {
            Debug.Log("근거리 공격");
        }

        else
        {
            Debug.Log("원거리 공격");
        }
    }

    public void AttackEnd()
    {
        Debug.Log("AttackEnd");
        StartCoroutine(CoDelay(1f));
        _isAttack = false;
    }

    private IEnumerator CoDelay(float sec)
    {
        Debug.Log("CoDelay");
        yield return new WaitForSeconds(sec);
    }
    public override void Skill()
    {
        Debug.Log("Skill");

        if (_attackType == EAttackType.Melee)
        {
            Debug.Log("!!! 근거리 스킬");
        }

        else
        {
            Debug.Log("!!! 원거리 스킬");
        }
    }

    public void SkillEnd()
    {
        Debug.Log("SkillEnd");
        StartCoroutine(CoDelay(2f));

        _isAttack = false;

        _currentSkillCool = _skillCool;
        _skillEnd = true;   //스킬 쿨다운 시작
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.forward * _range);
    }


}
