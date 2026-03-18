using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Unit : MonoBehaviour, IDamageable
{
    protected int _curHp;
    protected int _maxHp;
    protected int _atk;
    protected int _def;
    protected int _cp;
    protected int _totalDamaged;
    protected float _lifetime;
    
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

    public void SetMaxHp(int maxHp)
    {
        _maxHp = maxHp;
    }

    public int GetMaxHp() => _maxHp;


}
