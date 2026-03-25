using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

- 플레이어 생성함

필드 씬 -> 배틀 씬 -> 필드 씬

필드씬의 데이터 저장 필요
- cart, path 관련 데이터 (Cart의 Position, )



- 흐름 정리

진행 버튼 클릭
-> UI Manager : MovePlayer()
-> GameManager : MoveNextPoint()


*/

public enum EGameStage
{
    Stage1_1 = 0,
    Stage1_2 = 1,
    Stage1_3 = 2,
    Stage2_1 = 3,
    Stage2_2 = 4,
    Stage2_3 = 5,
    Stage3_1 = 6,
    Stage3_2 = 7,
    Stage3_3 = 8,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] PlayerSpawner _playerSpawner;
    [SerializeField] Vector3 _startPosition = new Vector3(52f, 0f, 32f);
    
    private EGameStage _currentStage = EGameStage.Stage1_1;

    private FieldAutoMove _autoMove;
    public Transform MainCharacterTr { get; private set; } = null;
    
    private List<Transform> _subCharacterTrList = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (DataSource.Instance.MainCharacterIdx == -1)
        {
            MainCharacterTr = _playerSpawner.SpawnPlayer((int)ECharacterNumber.Shield_01, _startPosition, Quaternion.identity).transform;
        }

        else
        {
            MainCharacterTr = _playerSpawner.SpawnPlayer(DataSource.Instance.MainCharacterIdx, _startPosition, Quaternion.identity).transform;
        }
        
        // 메인 제외 다른 캐릭터들 생성
        List<int> subNumberList = DataSource.Instance.GetCharacterList();
        
        for (int i = 0; i < subNumberList.Count; i++)
        {
            GameObject go = _playerSpawner.SpawnPlayer(subNumberList[i], _startPosition + new Vector3(-2f * i - 2f, 0f, 0f), Quaternion.identity);
            PartyFollow pf = go.GetComponent<PartyFollow>();
            pf.FollowOrder = i + 1;
            _subCharacterTrList.Add(go.transform);
            
        }

    }

    private void Update()
    {
        
    }

    public void MoveNextPoint()
    {
        if (_autoMove == null)
        {
            _autoMove = MainCharacterTr.GetComponent<FieldAutoMove>();
        }

        // 이미 이동중이면 종료
        if (_autoMove.IsMoving)
        {
            return;
        }

        _autoMove.MoveNextPoint();
    }
}
