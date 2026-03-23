using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSkill : Projectile
{
    protected new void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _delay)
            return;

        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    protected new void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($" ArrowSkill - {other.name} - 데미지 {Atk} 입음");
            Destroy(gameObject);
        }
    }
}
