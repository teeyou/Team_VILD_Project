using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSkill : Projectile
{
    protected new void Update()
    {
        // 차징 스킬 딜레이
        _timer += Time.deltaTime;
        
        if (_timer < _delay)
        {
            return;
        }

        // 차징 스킬 기 모을 때 충돌 판정 방지
        if (!_col.enabled)
        {
            _col.enabled = true;
        }


        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            hitPoint.y += 0.5f;
            
            ParticleManager.Instance.Play("Hit_GreenLarge", hitPoint);

            if (Owner != null)
            {
                other.GetComponent<Unit>().TakeDamage(Atk, transform);
                Owner.TotalDamage += Atk;
            }

            Destroy(gameObject);
        }
    }
}
