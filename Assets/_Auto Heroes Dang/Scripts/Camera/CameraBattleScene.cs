using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBattleScene : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _startCam;
    [SerializeField] private CinemachineVirtualCamera _skillCam;

    [Header("스킬 연출 거리")]
    [SerializeField] private float _distance = 3f;
    [SerializeField] private float _height = 1f;

    [Header("카메라 오프셋")]
    [SerializeField] private Vector3 _offset;

    [Header("키면 정면, 끄면 측면")]
    [SerializeField] private bool _isFront;

    [Header("스킬 연출 시 터레인 끔")]
    [SerializeField] private GameObject _terrain;

    private const int Default_Priority = 10;
    private const int High_Priority = 11;

    private Coroutine _coroutine = null;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    public void SetStartCam()
    {
        _skillCam.Priority = Default_Priority;
        _startCam.Priority = High_Priority;
    }

    public void SetSkillCam(Unit unit)
    {
        if (_isFront)
            _skillCam.transform.position = unit.transform.position + (unit.transform.forward * _distance) + (unit.transform.up * _height);

        else
            _skillCam.transform.position = unit.transform.position + (unit.transform.right * _distance) + (unit.transform.up * _height);
        
        _skillCam.transform.position += _offset;

        _skillCam.LookAt = unit.transform;
    }

    public void StartSkillCam()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        _coroutine = StartCoroutine(CoStartSkillCam(2f));
    }

    private IEnumerator CoStartSkillCam(float time)
    {
        _terrain.SetActive(false);
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _startCam.Priority = Default_Priority;
        _skillCam.Priority = High_Priority;

        yield return new WaitForSeconds(time);

        _terrain.SetActive(true);
        _cam.clearFlags = CameraClearFlags.Skybox;
        SetStartCam();
    }
}
