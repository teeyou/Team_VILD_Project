using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardSkill : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            hitPoint.y -= 0.8f;

            ParticleManager.Instance.Play("Hit_WizardSkill", hitPoint);

            if (Owner != null)
            {
                other.GetComponent<Unit>().TakeDamage(Atk, transform);
                Owner.TotalDamage += Atk;
            }

            Destroy(gameObject);
        }
    }
}
