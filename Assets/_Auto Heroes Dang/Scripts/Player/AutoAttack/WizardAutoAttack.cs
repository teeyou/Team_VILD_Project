using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAutoAttack : AutoAttack
{
    [SerializeField] private float _x = 0f;
    [SerializeField] private float _y = 0f;
    [SerializeField] private float _z = 0f;

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
        proj.TargetTr = _targetTr;
        proj.Atk = _atk;
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
        proj.TargetTr = _targetTr;
        proj.Atk = (int)(_atk * _skillMultiplier);
    }

    //public override void TakeDamage(int damage, Transform target)
    //{
    //    Debug.Log($"{target.name} - {damage}");
    //}
}
