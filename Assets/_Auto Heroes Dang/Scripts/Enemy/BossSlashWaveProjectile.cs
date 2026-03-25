using System.Collections.Generic;
using UnityEngine;

public class BossSlashWaveProjectile : MonoBehaviour
{
    private BossEnemyBattle _owner;
    private float _speed;
    private float _maxDistance;
    private int _startDamage;
    private float _minDamageRatio;
    private float _minScaleRatio;
    private LayerMask _targetLayer;

    private Vector3 _startPosition;
    private Vector3 _initialScale;
    private readonly HashSet<Unit> _hitTargets = new HashSet<Unit>();

    public void Initialize(
        BossEnemyBattle owner,
        float speed,
        float maxDistance,
        int startDamage,
        float minDamageRatio,
        float minScaleRatio,
        LayerMask targetLayer)
    {
        _owner = owner;
        _speed = speed;
        _maxDistance = maxDistance;
        _startDamage = startDamage;
        _minDamageRatio = minDamageRatio;
        _minScaleRatio = minScaleRatio;
        _targetLayer = targetLayer;

        _startPosition = transform.position;
        _initialScale = transform.localScale;
    }

    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;

        float travelDistance = Vector3.Distance(_startPosition, transform.position);
        float t = Mathf.Clamp01(travelDistance / _maxDistance);

        float scaleRatio = Mathf.Lerp(1f, _minScaleRatio, t);
        transform.localScale = _initialScale * scaleRatio;

        if (travelDistance >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _targetLayer) == 0)
            return;

        Unit unit = other.GetComponent<Unit>();
        if (unit == null) 
            return;

        if (unit.IsDead) 
            return;

        if (_hitTargets.Contains(unit)) 
            return;

        _hitTargets.Add(unit);

        float travelDistance = Vector3.Distance(_startPosition, transform.position);
        float t = Mathf.Clamp01(travelDistance / _maxDistance);

        float damageRatio = Mathf.Lerp(1f, _minDamageRatio, t);
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(_startDamage * damageRatio));

        unit.TakeDamage(finalDamage, _owner != null ? _owner.transform : transform);

        if (_owner != null)
        {
            Debug.Log($"{_owner.name} >> {unit.name} 검기 타격 / 거리={travelDistance:F1} / 데미지={finalDamage}");
        }
    }
}