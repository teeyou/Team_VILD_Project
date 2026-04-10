using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 
플레이어가 체크포인트까지 가서 정지하면 

JSON 데이터 저장 
 
 
*/


public class FieldAutoMove : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart _cart;
    [SerializeField] private CinemachineSmoothPath _path;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private FieldUI _fieldUI;          // 스테이지 정보 팝업용으로 추가 0325 진주

    private Animator _animator;
    private AutoAttack _autoAttack;

    private List<float> _cartPositionList = new List<float>();  //웨이포인트 별 카트의 Position 정보를 가지고 있는 리스트

    private bool _isMoving = false;
    private int _currentIdx = 0;    //플레이어의 현재 웨이포인트 인덱스

    public bool IsMoving { get { return _isMoving; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _autoAttack = GetComponent<AutoAttack>();
    }

    private void Start()
    {
        // 씬에 따라서 스크립트 활성화 / 비활성화
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬이 필드 씬이 아니면 비활성화
        if (currentSceneName != ESceneId.FieldScene.ToString())
        {
            enabled = false;
            Debug.Log("FieldAutoMove disabled.");
            return;
        }

        SetCartAndPath();

        transform.rotation = _cart.transform.rotation;

        // 트랙의 웨이포인트 위치를 카트의 Position으로 변환
        int stageLength =  FieldManager.Instance.GetStageLength();
        for (int i = 0; i < stageLength; i++)
        {
            _cartPositionList.Add(GetCartPositionFromWaypoint(i));
        }

        // track 웨이포인트 인덱스
        _currentIdx = DataSource.Instance.CurrentIdx;
    }

    private void SetCartAndPath()
    {
        _cart = FindObjectOfType<CinemachineDollyCart>();
        _path = FindObjectOfType<CinemachineSmoothPath>();
        _fieldUI = FindObjectOfType<FieldUI>();             // 스테이지 정보 팝업용으로 추가 0325 진주

        _cart.m_Speed = 0f;
        _cart.m_Position = DataSource.Instance.CartPosition;
    }

    private void Update()
    {
        // 씬 전환하고 돌아올 때 참조가 끊어지면 세팅
        if (_cart == null || _path == null)
        {
            SetCartAndPath();
        }

        if (!_isMoving && InputManager.Instance.IsPressedSpace)
        {
            UIManager.Instance.ToggleProgressButton(false);
            MoveNextPoint();
        }
        
        if (_isMoving)
        {
            // 카트가 다음 웨이포인트까지 도착하면 멈춤
            if (_cart.m_Position >= _cartPositionList[_currentIdx])
            {
                Stop();
                return;
            }

            Vector3 pos = _cart.transform.position;
            transform.position = pos;
            transform.rotation = _cart.transform.rotation;
        }

    }

    // 웨이포인트의 월드좌표 -> Cart Position 변환
    private float GetCartPositionFromWaypoint(int idx)
    {
        // 웨이포인트의 월드 좌표
        Vector3 waypointPos = _path.transform.TransformPoint(FieldManager.Instance.GetStagePosition(idx));

        // 트랙 상에서 가장 가까운 지점(내부 좌표) 찾기
        float nativePos = _path.FindClosestPoint(waypointPos, -1, -1, 10);

        // Cart의 Position으로 변환
        return _path.FromPathNativeUnits(nativePos, CinemachinePathBase.PositionUnits.Distance);
    }

    public void MoveNextPoint()
    {
        if (_currentIdx >= FieldManager.Instance.GetStageLength())
        {
            return;
        }

        _autoAttack.enabled = false;    // 이동 중에는 전투 발생 안함. 꺼놓음 

        _isMoving = true;
        _cart.m_Speed = _moveSpeed;
        _animator.SetBool("Move", true);

        AudioManager.Instance.PlayLoopSFX("FootSFX");

        FieldManager.Instance.IsSpawnPossible = false;  // 이동 중에는 몬스터 스폰 불가능
    }

    public void Stop()
    {
        GameManager.Instance.IsFirstPoint = false;  // Stop이 호출된 지점부터는 스테이지 포인트
        GameManager.Instance.IsStageClear = false;
        _autoAttack.enabled = true;     // 필드에서 일정 시간 지나면 몬스터 스폰되기 때문에 활성화

        _isMoving = false;
        _cart.m_Speed = 0f;
        DataSource.Instance.CurrentIdx++;

        _animator.SetBool("Move", false);

        AudioManager.Instance.StopLoopSFX();

        UIManager.Instance.ToggleStageButton(true);
        //_fieldUI.PopUpFieldInfo(); // 스테이지 정보 UI 팝업.

        FieldManager.Instance.IsSpawnPossible = true;

        Save();
        
        UIManager.Instance.PopUpToastMessage("잠시 후 스폰되는 몬스터를 잡아서 재화를 모을 수 있어요.", 5f);
    }

    private void Save()
    {
        // 메인 캐릭터 저장
        DataSource.Instance.MainCharacterPosRot = new PosRotData(
            FieldManager.Instance.MainCharacterTr.position,
            FieldManager.Instance.MainCharacterTr.rotation);

        // 서브 캐릭터 저장
        //int count = DataSource.Instance.GetSubPosRotList().Count;    // 필드 씬 위치정보 리스트 길이

        IReadOnlyList<Transform> trList = FieldManager.Instance.GetSubCharacterTrList;  //필드 씬 서브 캐릭터들 트랜스폼
        int len = trList.Count;

        for (int i = 0; i < len; i++)
        {
            DataSource.Instance.UpdatePosRotList(i, trList[i].position, trList[i].rotation);
        }

        DataSource.Instance.CartPosition = _cart.m_Position;

        DataSource.Instance.Save(); //  JSON에 저장
    }
}
