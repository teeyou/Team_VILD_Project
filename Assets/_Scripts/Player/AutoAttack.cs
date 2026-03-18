using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAttack : Unit
{
    [SerializeField] private BaseStatus_SO _baseStatsData;
    [SerializeField] private float _searchRadius;
    [SerializeField] private LayerMask _layerMask;

    public BaseStatus_SO BaseStatsData { get; set; }

    
    private void Start()
    {
        _maxHp = BaseStatsData.DefaultMaxHp;
        _curHp = BaseStatsData.DefaultMaxHp;

        _atk = BaseStatsData.DefaultAtk;
        _def = BaseStatsData.DefaultDef;
        _cp = _curHp + _atk + _def;
        
        _range = BaseStatsData.Range;
        
        _line = EPositionLine.None;
        _map = ECombatMap.Field;

        _layerMask = LayerMask.GetMask("Enemy");

        _targetTr = null;
        _searchRadius = 10f;

        PrintData();
    }

    void Update()
    {
        // 타겟이 NULL 이거나 오브젝트 비활성화면 타겟 탐색
        if (_targetTr == null || !_targetTr.gameObject.activeSelf)
        {
            CheckTarget();
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
            
    }

    public override void Skill()
    {
            
    }

    public void PrintData()
    {
        Debug.Log($"{BaseStatsData.Id} {BaseStatsData.ChName} {BaseStatsData.DefaultMaxHp} {BaseStatsData.DefaultAtk} {BaseStatsData.DefaultDef}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }


}
