using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보류
// BattleScene 에서 위치
// Front, Middle -> Front or Middle 중 가까운 것 먼저 공격
// Rear -> Rear 먼저 공격. Rear 없으면, 위치 상관없이 가장 가까운 것 먼저




// Front, Middle, Rear -> 제일 가까운 것 먼저 공격
public enum EPositionLine
{
    None,
    Front,
    Middle,
    Rear,
}

public enum ECombatMap
{
    Field,
    Battle,
}

public abstract class Unit : MonoBehaviour, IDamageable
{
    protected int _curHp;
    protected int _maxHp;
    protected int _atk;
    protected int _def;
    protected int _cp;

    protected ECombatMap _map;      // 필드씬 , 배틀씬

    protected EPositionLine _line;  // 배치 위치
    protected float _range;         // 공격 가능 거리

    protected int _totalDamage;     // 내가 준 피해량
    protected int _totalDamaged;    // 내가 받은 피해량
    protected float _lifetime;      // 전투 중 살아남은 시간
    
    protected Transform _targetTr;  // 공격 대상 (몬스터 -> 플레이어, 플레이어 -> 몬스터)
    
    public virtual void Attack()
    {

    }

    public virtual void Skill()
    {

    }

    public virtual void Heal(int value)
    {

    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
    public virtual void TakeDamage(int damage, Transform target)
    {
        
    }

    //public void SetMaxHp(int maxHp)
    //{
    //    _maxHp = maxHp;
    //}

    //public int GetMaxHp() => _maxHp;


}
