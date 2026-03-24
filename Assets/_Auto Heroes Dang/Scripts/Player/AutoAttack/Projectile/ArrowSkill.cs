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

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            hitPoint.y += 0.5f;
            
            ParticleManager.Instance.Play("Hit_GreenLarge", hitPoint);

            other.GetComponent<Unit>().TakeDamage(Atk, transform);

            Debug.Log($" ArrowSkill - {other.name} - 데미지 {Atk} 입음");
            
            Destroy(gameObject);
        }
    }
}
