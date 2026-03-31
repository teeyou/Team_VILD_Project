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

// 현재 씬이 필드씬 or 배틀씬
public enum ECombatMap
{
    Field,
    Battle,
}

// 근거리 공격 or 원거리 공격
public enum EAttackType
{
    Melee,
    Range,
    Tank,
}

public enum EGrade
{
    B,
    A,
    S
}

public abstract class Unit : MonoBehaviour, IDamageable
{
    protected float _moveSpeed;
    protected int _curHp;
    protected int _maxHp;
    protected int _atk;
    protected int _def;
    protected int _cp;
    protected int _level;
    protected EGrade _grade;
    
    protected bool _isAttack;           // 공격중인지 확인

    protected EAttackType _attackType; // 근거리 또는 원거리

    protected ECombatMap _map;      // 필드씬 , 배틀씬

    protected EPositionLine _line;  // 배치 위치
    protected float _range;         // 공격 가능 거리

    protected int _totalDamage;     // 내가 준 피해량
    protected int _totalDamaged;    // 내가 받은 피해량
    protected float _lifetime;      // 전투 중 살아남은 시간

    protected DamageTextReceiver _damageTextReceiver; // 데미지 텍스트
    protected HpBar _HpBar; // 캐릭터 HP바

    protected Transform _targetTr;  // 공격 대상 (몬스터 -> 플레이어, 플레이어 -> 몬스터)

    protected bool _isDead;
    public bool IsDead {get { return _isDead; } set { _isDead = value; } }

    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public int CurHp { get { return _curHp; } set { _curHp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Atk { get { return _atk; } set { _atk = value; } }
    public int Def { get { return _def; } set { _def = value; } }
    public int Cp { get { return _cp; } set { _cp = value; } }
    public int Level { get { return _level; } set { _level = value; } }

    public bool IsAttack { get { return _isAttack; } set { _isAttack = value; } }

    public EGrade Grade { get { return _grade; } set { _grade = value; } }
    public EAttackType AttackType { get { return _attackType; } set { _attackType = value; } }
    public ECombatMap Map { get { return _map; } set { _map = value; } }
    public EPositionLine Line { get { return _line; } set { _line = value; } }
    public float Range { get { return _range; } set { _range = value; } }
    public int TotalDamage { get { return _totalDamage; } set { _totalDamage = value; } }
    public int TotalDamaged { get { return _totalDamaged; } set { _totalDamaged = value; } }
    public float LifeTime { get { return _lifetime; } set { _lifetime = value; } }
    public Transform TargetTr { get { return _targetTr; } set { _targetTr = value; } }
    
    public int CharacterNumber { get; set; }

    // 리시버 연결
    protected virtual void Awake()
    {
        _damageTextReceiver = GetComponent<DamageTextReceiver>();
    }

    // 데미지 텍스트를 공통으로 띄우는 함수
    protected void ShowDamageText(int damage, Transform attacker, bool isCritical = false)
    {
        if (_damageTextReceiver != null)
        {
            _damageTextReceiver.ShowDamage(damage, attacker, isCritical);
        }
    }

    // hp바
    public void SetHpBar(HpBar hpBar)
    {
        _HpBar = hpBar;
        RefreshHpBar();
    }

    protected void RefreshHpBar()
    {
        if (_HpBar != null)
        {
            _HpBar.SetHp(_curHp, _maxHp);
        }
    }

    public abstract void Attack();

    public abstract void Skill();

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
}
