using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAutoAttack : AutoAttack
{
    [SerializeField] private float _skillMultiplier = 1.2f;
    //[SerializeField] private float _x = 0f;
    //[SerializeField] private float _y = 0f;
    //[SerializeField] private float _z = 0f;

    public override void Attack()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;
        //Quaternion rot = Quaternion.AngleAxis(_x, transform.right) * Quaternion.AngleAxis(_y, transform.up) * Quaternion.AngleAxis(_z, transform.forward) * transform.rotation;
        Quaternion rot = Quaternion.AngleAxis(270f, transform.up) * transform.rotation;
        ParticleManager.Instance.Play("SlashBlue", pos, rot);
    }

    public override void Skill()
    {
        Vector3 pos = _targetTr.position;
        pos.y += 1f;
        pos.x += Random.Range(-0.3f, 0.3f);
        pos.z += Random.Range(-0.3f, 0.3f);

        ParticleManager.Instance.Play("Skill_HitFrost", pos);
    }

    public override void TakeDamage(int damage, Transform target)
    {
        Debug.Log($"{target.name} - {damage}");
    }
}
