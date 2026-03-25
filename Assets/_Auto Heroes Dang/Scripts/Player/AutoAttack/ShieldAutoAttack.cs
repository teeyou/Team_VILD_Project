using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAutoAttack : AutoAttack
{
    [SerializeField] private int _type = 0;

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

        if (_targetTr != null && _targetUnit != null)
        {
            _targetUnit.TakeDamage(_atk, transform);
        }
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

        if (_targetTr != null && _targetUnit != null)
        {
            _targetUnit.TakeDamage((int)(_atk * _skillMultiplier), transform);
        }
    }
}
