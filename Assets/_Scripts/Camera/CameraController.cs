using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _townCam;
    [SerializeField] private CinemachineVirtualCamera _stageOneCam;
    [SerializeField] private CinemachineVirtualCamera _stageTwoCam;
    [SerializeField] private CinemachineVirtualCamera _stageThreeCam;

    private CinemachineBrain _brain;

    private const int Default_Priority = 10;
    private const int High_Priority = 11;

    private ICinemachineCamera _currentCam;

    private void Awake()
    {
        _brain = GetComponent<CinemachineBrain>();
    }
    
    private void Update()
    {
        if (InputManager.Instance.IsPressedNum1)
        {
            _currentCam = _brain.ActiveVirtualCamera;

            _currentCam.Priority = Default_Priority;
            _townCam.Priority = High_Priority;
        }

        if (InputManager.Instance.IsPressedNum2)
        {
            _currentCam = _brain.ActiveVirtualCamera;

            _currentCam.Priority = Default_Priority;
            _stageOneCam.Priority = High_Priority;
        }

        if (InputManager.Instance.IsPressedNum3)
        {
            _currentCam = _brain.ActiveVirtualCamera;

            _currentCam.Priority = Default_Priority;
            _stageTwoCam.Priority = High_Priority;
        }

        if (InputManager.Instance.IsPressedNum4)
        {
            _currentCam = _brain.ActiveVirtualCamera;

            _currentCam.Priority = Default_Priority;
            _stageThreeCam.Priority = High_Priority;
        }

    }
}
