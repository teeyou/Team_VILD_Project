using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldAutoMove : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _cart;
    [SerializeField] private CinemachineSmoothPath _path;
    [SerializeField] private float _moveSpeed = 10f;

    private List<float> _cartPositionList = new List<float>();  //웨이포인트 별 카트의 Position 정보를 가지고 있는 리스트

    private bool _isMoving = false;
    private int _currentIdx = 0;    //플레이어의 현재 웨이포인트 인덱스
    

    private void Start()
    {
        _cart.m_Speed = 0f;
        _cart.m_Position = 0f;

        for (int i = 0; i < _path.m_Waypoints.Length; i++)
        {
            _cartPositionList.Add(GetWaypointDistance(i));
            //Debug.Log($"{i}의 Cart Position : {GetWaypointDistance(i)}");
        }
    }

    private void Update()
    {
        if (!_isMoving && InputManager.Instance.IsPressedSpace)
        {
            MoveNextPoint();
        }
        
        if (_isMoving)
        {
            // 카트가 다음 웨이포인트까지 도착하면 멈춤
            if (_cart.m_Position >= _cartPositionList[_currentIdx])
            {
                Stop();
            }
        }

    }

    private void LateUpdate()
    {
        if (_isMoving)
        {
            Vector3 pos = _cart.transform.position;
            pos.y = 1f;
            transform.position = pos;
            transform.rotation = _cart.transform.rotation * Quaternion.AngleAxis(-180f, Vector3.up);
        }

    }

    // 웨이포인트의 월드좌표 -> Cart Position 변환
    private float GetWaypointDistance(int index)
    {
        // 웨이포인트의 월드 좌표
        Vector3 waypointPos = _path.m_Waypoints[index].position;

        // 트랙 상에서 가장 가까운 지점(내부 좌표) 찾기
        float nativePos = _path.FindClosestPoint(waypointPos, -1, -1, 10);

        // Cart의 Position으로 변환
        return _path.FromPathNativeUnits(nativePos, CinemachinePathBase.PositionUnits.Distance);
    }

    public void MoveNextPoint()
    {
        _isMoving = true;
        _cart.m_Speed = _moveSpeed;
        _currentIdx++;
    }

    public void Stop()
    {
        _isMoving = false;
        _cart.m_Speed = 0f;
    }

}
