using System.Collections;
using UnityEngine;

public class ChestDropMotion : MonoBehaviour
{
    [Header("점프 높이")]
    [SerializeField] private float _jumpHeight = 1.5f;

    [Header("연출 시간")]
    [SerializeField] private float _duration = 0.5f;

    [Header("회전 대상")]
    [SerializeField] private Transform _visualRoot;

    [Header("회전 각도")]
    [SerializeField] private float _rotationX = 360f;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private Vector3 _visualStartEuler;

    private void Start()
    {
        _startPos = transform.position;
        _targetPos = _startPos;

        if (_visualRoot != null)
            _visualStartEuler = _visualRoot.localEulerAngles;

        StartCoroutine(Co_PlayDropMotion());
    }

    private IEnumerator Co_PlayDropMotion()
    {
        float time = 0f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _duration);

            float height = 4f * _jumpHeight * t * (1f - t);

            Vector3 pos = _targetPos;
            pos.y += height;
            transform.position = pos;

            if (_visualRoot != null)
            {
                float rotX = Mathf.Lerp(0f, _rotationX, t);
                _visualRoot.localRotation = Quaternion.Euler(
                    _visualStartEuler.x + rotX,
                    _visualStartEuler.y,
                    _visualStartEuler.z
                );
            }

            yield return null;
        }

        transform.position = _targetPos;

        if (_visualRoot != null)
        {
            _visualRoot.localRotation = Quaternion.Euler(
                _visualStartEuler.x,
                _visualStartEuler.y,
                _visualStartEuler.z
            );
        }
    }
}