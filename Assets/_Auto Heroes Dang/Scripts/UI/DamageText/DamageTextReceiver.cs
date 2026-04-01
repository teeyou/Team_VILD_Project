using UnityEngine;

public class DamageTextReceiver : MonoBehaviour
{
    [Header("기준 위치")]
    [SerializeField] private Transform _centerPoint;

    [Header("표시 높이")]
    [SerializeField] private float _heightOffset = 0f;

    [Header("랜덤 퍼짐")]
    [SerializeField] private Vector2 _randomOffsetX = new Vector2(-0.15f, 0.15f);
    [SerializeField] private Vector2 _randomOffsetY = new Vector2(0f, 0.2f);
    [SerializeField] private Vector2 _randomOffsetZ = new Vector2(-0.15f, 0.15f);

    [Header("색상")]
    [SerializeField] private Color _normalDamageColor = Color.red;
    [SerializeField] private Color _criticalDamageColor = Color.yellow;

    public void ShowDamage(int damage, Transform attacker = null, bool isCritical = false)
    {
        //if (DamageTextSpawner.Instance == null)
        //{
        //    Debug.LogWarning("DamageTextReceiver : DamageTextSpawner.Instance 가 없습니다.");
        //    return;
        //}

        Vector3 basePos = _centerPoint != null ? _centerPoint.position : transform.position;
        basePos += Vector3.up * _heightOffset;

        basePos += new Vector3(
            Random.Range(_randomOffsetX.x, _randomOffsetX.y),
            Random.Range(_randomOffsetY.x, _randomOffsetY.y),
            Random.Range(_randomOffsetZ.x, _randomOffsetZ.y)
        );

        Color color = isCritical ? _criticalDamageColor : _normalDamageColor;
       // DamageTextSpawner.Instance.SpawnDamageText(damage, basePos, color);
    }
}