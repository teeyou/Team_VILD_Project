using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBattleScene : MonoBehaviour
{
    //[SerializeField] private CinemachineVirtualCamera _readyCam;
    [SerializeField] private CinemachineVirtualCamera _startCam;

    //private CinemachineBrain _brain;

    private const int Default_Priority = 10;
    private const int High_Priority = 11;

    //private ICinemachineCamera _currentCam;
    private void Awake()
    {
        //_brain = GetComponent<CinemachineBrain>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (BattleManager.Instance.BattleState == EBattleState.Start)
        {
            _startCam.Priority = High_Priority;
        }
    }
}
