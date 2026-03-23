using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTarget : Unit
{
    public override void Attack()
    {

    }

    public override void Skill()
    {

    }

    public override void Heal(int value)
    {

    }

    public override void TakeDamage(int damage, Transform target)
    {
        int finalDamage = Mathf.Max(1, damage - _def);
        _curHp -= finalDamage;

        Debug.Log($"{name}이 피해 받음 / 남은 HP : {_curHp}");

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
        _isDead = true;

        Debug.Log($"{name} 죽음");
        Destroy(gameObject);
    }


    void Start()
    {
        _maxHp = 200;
        _curHp = _maxHp;
        _def = 2;
    }

    void Update()
    {

    }
}
