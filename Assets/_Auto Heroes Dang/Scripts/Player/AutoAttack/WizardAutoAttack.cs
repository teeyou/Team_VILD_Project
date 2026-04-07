using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAutoAttack : AutoAttack
{
    [SerializeField] private float _fpOffset = 1f;

    public override void Attack()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        pos += transform.forward * _fpOffset;

        //Quaternion rot = Quaternion.AngleAxis(_x, transform.right) * Quaternion.AngleAxis(_y, transform.up) * Quaternion.AngleAxis(_z, transform.forward) * transform.rotation;
        Quaternion rot = transform.rotation;

        GameObject projGo = ParticleManager.Instance.Play("EnergyBallBlue", pos, rot);
        Projectile proj = projGo.GetComponent<Projectile>();

        AudioManager.Instance.PlaySFX("MagicAttack");

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
        pos.y += 0.5f;
        pos += transform.forward * _fpOffset;

        //Quaternion rot = Quaternion.AngleAxis(_x, transform.right) * Quaternion.AngleAxis(_y, transform.up) * Quaternion.AngleAxis(_z, transform.forward) * transform.rotation;
        Quaternion rot = transform.rotation;

        GameObject projGo = ParticleManager.Instance.Play("Skill_EnergyBallBlue", pos, rot);
        Projectile proj = projGo.GetComponent<Projectile>();
        AudioManager.Instance.PlaySFX("MagicSkill");

        if (_targetTr != null)
        {
            proj.TargetTr = _targetTr;
        }

        if (_targetUnit != null)
        {
            bool isCritical;
            int damage = DamageCalculator.CalculateDamage(_atk, _targetUnit.Def, out isCritical);
            proj.Atk = (int)(damage * _skillMultiplier);
            proj.Owner = this;
        }
    }
}
