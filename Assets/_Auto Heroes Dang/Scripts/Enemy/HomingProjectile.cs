using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _hitRadius = 0.8f;

    [Header("투사체 소환되는 위치")]
    [SerializeField] private Transform _projectileSpawnPoint;

    [Header("히트 이펙트")]
    [SerializeField] private GameObject _hitVfxPrefab;
    [SerializeField] private float _hitVfxLifeTime = 1f;

    private Unit _target;
    private int _damage;
    private Transform _owner;
    private Collider _targetCollider;

    private bool _hasHit;

    public void Init(Unit target, int damage, float speed, Transform owner, Collider targetCollider)
    {
        _target = target;
        _damage = damage;
        _speed = speed;
        _owner = owner;
        _targetCollider = targetCollider;

        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        if (_hasHit)
            return;

        if (_target == null || _target.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        MoveToTarget();
        CheckHit();
    }


    private void MoveToTarget()
    {
        Vector3 targetPos = _target.transform.position;
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * _speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    private void CheckHit()
    {
        if (_targetCollider == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 closetPoint = _targetCollider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(transform.position, closetPoint);

        if (distance <= _hitRadius)
        {
            _hasHit = true;

            _target.TakeDamage(_damage, _owner);

            SpawnHitVfx(closetPoint);

            Destroy(gameObject);
        }
    }

    private void SpawnHitVfx(Vector3 hitposition)
    {
        if (_hitVfxPrefab == null)
            return;

        GameObject hitVfx = Instantiate(_hitVfxPrefab, hitposition, Quaternion.identity);
        Destroy(hitVfx, _hitVfxLifeTime);
    }

}