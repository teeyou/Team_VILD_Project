using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _delay = 0f;

    protected float _timer = 0f;

    public int Atk { get; set; }
    public Transform TargetTr {  get; set; }

    protected virtual void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < _delay)
        {
            return;
        }

        if (TargetTr == null)
        {
            Debug.Log("forward 이동");
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }

        else
        {
            Debug.Log("타겟으로 이동");
            Vector3 dir = (TargetTr.position - transform.position).normalized;
            dir.y = 0f;

            // 투사체 forward 타겟 방향으로.
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * 10f);

            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }
    }

    
}
