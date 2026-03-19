using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Stage 지점에 해당하는 Track의 Waypoint index 저장

*/
public class FieldManager : Singleton<FieldManager>
{
    [SerializeField] private CinemachineSmoothPath _path;

    private List<Vector3> _stagePosList = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();

        _stagePosList.Add(_path.m_Waypoints[5].position);   // 1 - 1
        _stagePosList.Add(_path.m_Waypoints[8].position);   // 1 - 2

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Vector3 GetStagePosition(int idx) => _stagePosList[idx];

    public int GetStageLength() => _stagePosList.Count;

}
