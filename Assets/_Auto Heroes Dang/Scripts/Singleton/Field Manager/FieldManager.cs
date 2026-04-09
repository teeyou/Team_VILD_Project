using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
Stage 지점에 해당하는 Track의 Waypoint index 저장

*/
public class FieldManager : Singleton<FieldManager>
{
    [SerializeField] PlayerSpawner _playerSpawner;
    [SerializeField] Vector3 _startPosition = new Vector3(52f, 0f, 32f);

    [SerializeField] private CinemachineSmoothPath _path;

    private List<Vector3> _stagePosList = new List<Vector3>();

    private FieldAutoMove _autoMove;

    private List<Transform> _subCharacterTrList = new List<Transform>();
    public Transform MainCharacterTr { get; private set; } = null;
    public bool IsSpawnPossible { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();

        _stagePosList.Add(_path.m_Waypoints[6].position);   // 1 - 1
        _stagePosList.Add(_path.m_Waypoints[9].position);   // 1 - 2
        _stagePosList.Add(_path.m_Waypoints[12].position);  // 1 - 3

        _stagePosList.Add(_path.m_Waypoints[14].position);  // 2 - 1
        _stagePosList.Add(_path.m_Waypoints[16].position);  // 2 - 2
        _stagePosList.Add(_path.m_Waypoints[19].position);  // 2 - 3

        _stagePosList.Add(_path.m_Waypoints[21].position);  // 3 - 1
        _stagePosList.Add(_path.m_Waypoints[24].position);  // 3 - 2
        _stagePosList.Add(_path.m_Waypoints[26].position);  // 3 - 3

    }
    void Start()
    {
        SpawnPlayerCharacter();
        Save();

        if (DataSource.Instance.GetSaveData != null)
        {
            IsSpawnPossible = DataSource.Instance.GetSaveData.isSpawnPossible;
        }
    }

    private void SpawnPlayerCharacter()
    {
        // 필드씬에서 바로 게임시작 시 여기로 진입
        if (DataSource.Instance.MainCharacterIdx == -1)
        {
            DataSource.Instance.MainCharacterIdx = (int)ECharacterNumber.Shield_01;

            MainCharacterTr = _playerSpawner.SpawnPlayer(DataSource.Instance.MainCharacterIdx, _startPosition, Quaternion.identity).transform;

            // 메인 제외 다른 캐릭터들 생성
            List<int> subNumberList = DataSource.Instance.GetCharacterList();

            int len = 3; //필드에 보여질 서브 캐릭터 수

            // 서브 캐릭터 생성
            for (int i = 0; i < len; i++)
            {
                GameObject go = _playerSpawner.SpawnPlayer(subNumberList[i], _startPosition + new Vector3(-2f * i - 2f, 0f, 0f), Quaternion.identity);
                PartyFollow pf = go.GetComponent<PartyFollow>();
                pf.FollowOrder = i + 1;
                _subCharacterTrList.Add(go.transform);
            }
        }

        else
        {
            // 캐릭터 선택 씬 또는 스테이지에서 복귀 할 때 여기로 진입

            PosRotData mainData = DataSource.Instance.MainCharacterPosRot;
            if (mainData == null)
            {
                MainCharacterTr = _playerSpawner.SpawnPlayer(DataSource.Instance.MainCharacterIdx, _startPosition, Quaternion.identity).transform;
            }

            else
            {
                MainCharacterTr = _playerSpawner.SpawnPlayer(DataSource.Instance.MainCharacterIdx, mainData.Pos, mainData.Rot).transform;
            }


            // 서브 캐릭터 생성
            List<int> subNumberList = DataSource.Instance.GetCharacterList();   //생성 넘버

            int len = 3; //필드에 보여질 서브 캐릭터 수

            for (int i = 0; i < len; i++)
            {
                PosRotData data = DataSource.Instance.GetSubPosRot(i);

                GameObject go;
                if (data == null)
                {
                    go = _playerSpawner.SpawnPlayer(subNumberList[i], _startPosition + new Vector3(-2f * i - 2f, 0f, 0f), Quaternion.identity);
                }

                else
                {
                    go = _playerSpawner.SpawnPlayer(subNumberList[i], data.Pos, data.Rot);
                }

                PartyFollow pf = go.GetComponent<PartyFollow>();
                pf.FollowOrder = i + 1;
                _subCharacterTrList.Add(go.transform);
            }

        }
    }

    private void Save()
    {
        // 메인 캐릭터 저장
        DataSource.Instance.MainCharacterPosRot = new PosRotData(MainCharacterTr.position, MainCharacterTr.rotation);

        IReadOnlyList<Transform> trList = GetSubCharacterTrList;  //필드 씬 서브 캐릭터들 트랜스폼

        for (int i = 0; i < trList.Count; i++)
        {
            DataSource.Instance.UpdatePosRotList(i, trList[i].position, trList[i].rotation);
        }

        DataSource.Instance.Save(); //  JSON에 저장
    }

    void Update()
    {
        
    }

    public Vector3 GetStagePosition(int idx) => _stagePosList[idx];

    public int GetStageLength() => _stagePosList.Count;

    public void MoveNextPoint()
    {
        if (_autoMove == null)
        {
            _autoMove = MainCharacterTr.GetComponent<FieldAutoMove>();
        }

        // 이미 이동중이면 종료
        if (_autoMove.IsMoving)
        {
            Debug.Log("이미 이동중");
            return;
        }

        _autoMove.MoveNextPoint();
    }

    public IReadOnlyList<Transform> GetSubCharacterTrList => _subCharacterTrList;
}
