using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandAutoAttack : AutoAttack
{

    public override void Attack()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;
        Quaternion rot = Quaternion.AngleAxis(90f, transform.up) * transform.rotation;
        ParticleManager.Instance.Play("SlashRed", pos, rot);
        
        if (_targetTr != null && _targetUnit != null)
        {
            _targetUnit.TakeDamage(_atk, transform);
        }
    }

    public override void Skill()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;

        Quaternion rot = Quaternion.AngleAxis(270f, transform.right) * Quaternion.AngleAxis(0f, transform.up) * Quaternion.AngleAxis(270f, transform.forward) * transform.rotation;
        ParticleManager.Instance.Play("Skill_CartoonRedSlash", pos, rot);

        if (_targetTr != null && _targetUnit != null)
        {
            _targetUnit.TakeDamage((int)(_atk * _skillMultiplier), transform);
        }

    }
}
