using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyFollow : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _cart;
    [SerializeField] private CinemachineSmoothPath _path;

    [SerializeField] private float _spacing = 2f;

    private Animator _animator;
    private AutoAttack _autoAttack;

    public int FollowOrder { get; set; } = -1;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _autoAttack = GetComponent<AutoAttack>();
    }

    private void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬이 필드 씬이 아니면 비활성화
        //ESceneId.FieldScene.ToString()
        if (currentSceneName != ESceneId.ZFieldTest.ToString())
        {
            enabled = false;
            Debug.Log("FieldAutoMove disabled.");
            return;
        }
    }

    void Update()
    {
        if (_cart == null || _path == null)
        {
            SetCartAndPath();
            return;
        }

        if (_cart.m_Speed <= 0)
        {
            _autoAttack.enabled = true;
            _animator.SetBool("Move", false);
            return;
        }

        float memberPos = _cart.m_Position - _spacing * FollowOrder;
        if (memberPos < 0)
        {
            memberPos = 0;
        }

        // path 상의 특정 지점 값 -> 월드 좌표 변환
        Vector3 pos = _path.EvaluatePositionAtUnit(memberPos, CinemachinePathBase.PositionUnits.Distance);
        
        // path 상의 회전 
        Quaternion rot = _path.EvaluateOrientationAtUnit(memberPos, CinemachinePathBase.PositionUnits.Distance);

        transform.position = pos;
        transform.rotation = rot;

        Debug.Log($"cart speed : {_cart.m_Speed}");

        if (_cart.m_Speed > 0)
        {
            Debug.Log("Move");
            _autoAttack.enabled = false;
            _animator.SetBool("Move", true);
        }

        //else
        //{
        //    Debug.Log("Idle");
        //    _animator.SetBool("Move", false);
        //}
    }

    private void SetCartAndPath()
    {
        _cart = FindObjectOfType<CinemachineDollyCart>();
        _path = FindObjectOfType<CinemachineSmoothPath>();
    }
}
