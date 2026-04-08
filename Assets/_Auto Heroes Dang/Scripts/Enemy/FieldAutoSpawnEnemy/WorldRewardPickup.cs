using System.Collections;
using UnityEngine;

public class WorldRewardPickup : MonoBehaviour
{
    [Header("월드 이동")]
    [SerializeField] private float _scatterDuration = 0.5f;
    [SerializeField] private float _scatterHeight = 1.2f;

    [Header("사라지는 시간")]
    [SerializeField] private float _lifeTimeAfterScatter = 2f;

    private Vector3 _startPos;
    private Vector3 _targetWorldPos;

    public void Init(Vector3 targetWorldPos)
    {
        _startPos = transform.position;
        _targetWorldPos = targetWorldPos;

        StartCoroutine(Co_PlayRewardFlow());
    }

    private IEnumerator Co_PlayRewardFlow()
    {
        yield return StartCoroutine(Co_Scatter());

        yield return new WaitForSeconds(_lifeTimeAfterScatter);

        Destroy(gameObject);
    }

    private IEnumerator Co_Scatter()
    {
        float time = 0f;

        while (time < _scatterDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / _scatterDuration);

            Vector3 pos = Vector3.Lerp(_startPos, _targetWorldPos, t);
            pos.y += 4f * _scatterHeight * t * (1f - t);

            transform.position = pos;

            yield return null;
        }

        transform.position = _targetWorldPos;
    }
}