using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandAutoAttack : AutoAttack
{
    [SerializeField] private float _skillMultiplier = 1.2f;
    //[SerializeField] private float _x = 270;
    //[SerializeField] private float _y = 0f;
    //[SerializeField] private float _z = 270;

    public override void Attack()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;
        Quaternion rot = Quaternion.AngleAxis(90f, transform.up) * transform.rotation;
        ParticleManager.Instance.Play("SlashRed", pos, rot);
    }

    public override void Skill()
    {
        Vector3 pos = transform.position;
        pos.y += 1f;

        Quaternion rot = Quaternion.AngleAxis(270f, transform.right) * Quaternion.AngleAxis(0f, transform.up) * Quaternion.AngleAxis(270f, transform.forward) * transform.rotation;
        ParticleManager.Instance.Play("Skill_CartoonRedSlash", pos, rot);
    }

    public override void TakeDamage(int damage, Transform target)
    {
        Debug.Log($"{target.name} - {damage}");
    }
}
