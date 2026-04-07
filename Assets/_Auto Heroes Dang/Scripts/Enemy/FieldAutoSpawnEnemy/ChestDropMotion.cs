using System.Collections;
using UnityEngine;

public class ChestDropMotion : MonoBehaviour
{
    [Header("상승/하강 높이")]
    [SerializeField] private float _jumpHeight = 1.5f;

    [Header("연출 시간")]
    [SerializeField] private float _duration = 0.5f;

    private Vector3 _startPos;
    private Vector3 _targetPos;

    private void Start()
    {
        _startPos = transform.position;
        _targetPos = _startPos;

        StartCoroutine(Co_DropMotion());
    }

    private IEnumerator Co_DropMotion()
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

            float rotX = 360f * t;
            transform.rotation = Quaternion.Euler(rotX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            yield return null;
        }

        transform.position = _targetPos;
        transform.rotation = Quaternion.Euler(360f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}