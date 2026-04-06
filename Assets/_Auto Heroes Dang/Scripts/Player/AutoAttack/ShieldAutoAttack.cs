using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAutoAttack : AutoAttack
{
    [SerializeField] private int _type = 0;

    private int _skillHitCount = 2;

    public override void Attack()
    {
        base.Attack();

        if (_targetTr == null)
        {
            return;
        }

        Vector3 pos = transform.position;
        pos.y += 1f;
        Quaternion rot = Quaternion.AngleAxis(270f, transform.up) * transform.rotation;

        AudioManager.Instance.PlaySFX("SwordAttack1");

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
            bool isCritical;
            int damage = DamageCalculator.CalculateDamage(_atk, _targetUnit.Def, out isCritical);
            _totalDamage += damage;
            _targetUnit.TakeDamage(damage, transform);
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
            AudioManager.Instance.PlaySFX("SwordSkill");
        }

        else if (_type == 2)
        {
            ParticleManager.Instance.Play("Skill_HitPink", pos);
            AudioManager.Instance.PlaySFX("SwordSkill");
        }

        if (_targetTr != null && _targetUnit != null)
        {
            bool isCritical;
            int damage = DamageCalculator.CalculateDamage(_atk, _targetUnit.Def, out isCritical);

            damage = (int)(damage * _skillMultiplier / _skillHitCount);

            _totalDamage += damage;
            _targetUnit.TakeDamage(damage, transform);
        }
    }
}
