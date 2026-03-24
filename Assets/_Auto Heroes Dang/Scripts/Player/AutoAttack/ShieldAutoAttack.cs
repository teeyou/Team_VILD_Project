using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAutoAttack : AutoAttack
{
    [SerializeField] private int _type = 0;

    //[SerializeField] private float _x = 270;
    //[SerializeField] private float _y = 0f;
    //[SerializeField] private float _z = 270;

    public override void Attack()
    {
        if (_targetTr == null)
        {
            return;
        }

        Vector3 pos = transform.position;
        pos.y += 1f;
        Quaternion rot = Quaternion.AngleAxis(270f, transform.up) * transform.rotation;

        if (_type == 1)
        {
            ParticleManager.Instance.Play("SlashBlue", pos, rot);
        }

        else if (_type == 2)
        {
            ParticleManager.Instance.Play("SlashPink", pos, rot);
        }

        _targetUnit.TakeDamage(_atk, transform);
    }

    public override void Skill()
    {
        if (_targetTr == null)
        {
            return;
        }

        Vector3 pos = _targetTr.position;
        pos.y += 1f;
        pos.x += Random.Range(-0.3f, 0.3f);
        pos.z += Random.Range(-0.3f, 0.3f);

        if (_type == 1)
        {
            ParticleManager.Instance.Play("Skill_HitBlue", pos);
        }

        else if (_type == 2)
        {
            ParticleManager.Instance.Play("Skill_HitPink", pos);
        }

        _targetUnit.TakeDamage((int)(_atk * _skillMultiplier), transform);
    }

    //public override void TakeDamage(int damage, Transform target)
    //{
    //    Debug.Log($"{target.name} - {damage}");
    //}
}
