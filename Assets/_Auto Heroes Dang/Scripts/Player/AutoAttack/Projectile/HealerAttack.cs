using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAttack : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            hitPoint.y += 0.5f;

            ParticleManager.Instance.Play("Hit_HealerAttack", hitPoint);

            if (Owner != null)
            {
                other.GetComponent<Unit>().TakeDamage(Atk, transform);
                Owner.TotalDamage += Atk;
            }

            Destroy(gameObject);
        }
    }
}
