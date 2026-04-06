using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAutoAttack : AutoAttack
{
    [SerializeField] private int _healAmount = 100;

    [SerializeField] private float _fpOffset = 1f;
    public override void Attack()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        pos += transform.forward * _fpOffset;

        //Quaternion rot = Quaternion.AngleAxis(_x, transform.right) * Quaternion.AngleAxis(_y, transform.up) * Quaternion.AngleAxis(_z, transform.forward) * transform.rotation;
        Quaternion rot = transform.rotation;

        GameObject projGo = ParticleManager.Instance.Play("ProjectilePurple", pos, rot);
        Projectile proj = projGo.GetComponent<Projectile>();

        AudioManager.Instance.PlaySFX("HealerAttack");

        if (_targetTr != null)
        {
            proj.TargetTr = _targetTr;
        }

        if (_targetUnit != null)
        {
            bool isCritical;
            int damage = DamageCalculator.CalculateDamage(_atk, _targetUnit.Def, out isCritical);
            proj.Atk = damage;
            proj.Owner = this;
        }
    }

    public override void Skill()
    {
        Vector3 pos = transform.position;
        //pos.y += 0.5f;
        //pos += transform.forward * _fpOffset;

        //Quaternion rot = Quaternion.AngleAxis(_x, transform.right) * Quaternion.AngleAxis(_y, transform.up) * Quaternion.AngleAxis(_z, transform.forward) * transform.rotation;
        //Quaternion rot = transform.rotation;

        GameObject go = ParticleManager.Instance.Play("Skill_HealArea", pos);
        go.transform.SetParent(transform);

        // 아군 전부 회복
        Collider[] cols = Physics.OverlapSphere(transform.position, _searchRadius, LayerMask.GetMask("Player"));

        for (int i = 0; i < cols.Length; i++)
        {
            Unit unit = cols[i].GetComponent<Unit>();

            if (unit == null)
            {
                return;
            }

            GameObject heal = ParticleManager.Instance.Play("Heal", unit.transform.position);
            heal.transform.SetParent(unit.transform);

            unit.Heal((int)(_maxHp * _skillMultiplier));
        }
    }
}
