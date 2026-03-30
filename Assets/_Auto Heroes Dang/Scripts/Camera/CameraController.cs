using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCam;

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
        // 플레이어 동적 생성 후 카메라에 할당
        if (_playerCam.m_Follow == null)
        {
            Transform playerTr = FieldManager.Instance.MainCharacterTr;

            if (playerTr != null)
            {
                _playerCam.m_Follow = playerTr;
            }
        }
    }
}
