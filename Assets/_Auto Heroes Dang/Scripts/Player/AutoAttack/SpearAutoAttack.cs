using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAutoAttack : AutoAttack
{
    private int _skillHitCount = 4;

    private void Start()
    {
        _skillMultiplier = 2f;
    }

    public override void Attack()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;
        //Quaternion rot = Quaternion.AngleAxis(_x, transform.right) * Quaternion.AngleAxis(_y, transform.up) * Quaternion.AngleAxis(_z, transform.forward) * transform.rotation;
        Quaternion rot = Quaternion.AngleAxis(270f, transform.up) * transform.rotation;
        ParticleManager.Instance.Play("SlashWaterBlue", pos, rot);

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

        ParticleManager.Instance.Play("Skill_HitFrost", pos);

        if (_targetTr != null && _targetUnit != null)
        {
            int damage = DamageCalculator.CalculateDamage(_atk, _targetUnit.Def);
            damage = (int)(damage * _skillMultiplier / _skillHitCount);

            _targetUnit.TakeDamage(damage, transform);
        }
    }
}
