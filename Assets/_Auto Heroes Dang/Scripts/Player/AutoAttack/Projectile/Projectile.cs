using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



// 유도 미사일 베이스
// 각 직업 별로 스크립트 만들고, 상속받아서 OnTrrigerEnter 구현하기



public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _delay = 0f;

    protected Collider _col;
    protected float _timer = 0f;

    public int Atk { get; set; }
    public Transform TargetTr {  get; set; }

    private Unit _targetUnit;
    private void Awake()
    {
        _col = GetComponent<Collider>();

        _col.enabled = false;
    }

    protected virtual void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < _delay)
        {
            return;
        }

        // 차징 스킬 기 모을 때 충돌 판정 방지
        if (!_col.enabled)
        {
            _col.enabled = true;
        }

        // 타겟 없으면 직선 이동
        if (TargetTr == null)
        {
            //Debug.Log("forward 이동");
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }

        // 타겟 있으면 유도 미사일
        else
        {
            if (_targetUnit == null)
            {
                _targetUnit = TargetTr.GetComponent<Unit>();
            }
            
            if (_targetUnit.IsDead)
            {
                TargetTr = null;
                return;
            }

            //Debug.Log("타겟으로 이동");
            Vector3 dir = (TargetTr.position - transform.position).normalized;
            dir.y = 0f;

            // 투사체 forward 타겟 방향으로.
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 10f);

            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }
    }

    
}
