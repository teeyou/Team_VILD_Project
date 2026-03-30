using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

배틀 씬 시작 트랜스폼

가로 - (x+3, 0, z-3
세로 - (x-2, 0, z-2)

Player 시작 트랜스폼
Rotation (0 45 0) 

1행
(-6 0 9) (-3 0 6) (0 0 3)
(-8 0 7) (-5 0 4) (-2 0 1)
(-10 0 5) (-7 0 2) (-4 0 -1)


Enemy 시작 트랜스폼
Rotation (0 225 0) 

1행
(0.5, 0, 15.5) ...
...


*/

public enum EBattleState
{
    None,
    Start,
    Victory,
    Defeat,
}

public class BattleManager : Singleton<BattleManager>
{


    private const int ROW = 3;
    private const int COL = 3;

    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private GameObject[] _enemies;

    [SerializeField] private GameObject _placementPlanePrefab;
    [SerializeField] private GameObject _placementPlaneEnemyPrefab;

    private Transform _placementRoot;

    private List<Vector3> _playerStartingPosList = new List<Vector3>();
    private List<Vector3> _enemyStartingPosList = new List<Vector3>();

    private Dictionary<int, GameObject> _numberToGameObject = new Dictionary<int, GameObject>();
    private Dictionary<GameObject, Unit> _gameObjectToUnit = new Dictionary<GameObject, Unit>();

    private Dictionary<GameObject, Unit> _enemyGoToUnit = new Dictionary<GameObject, Unit>();

    private EBattleState _battleState = EBattleState.None;

    public EBattleState BattleState { get { return _battleState; } }

    public IReadOnlyList<Vector3> PlayerStartingPosList { get { return _playerStartingPosList; } }
    public IReadOnlyList<Vector3> EnemyStartingPosList { get { return _enemyStartingPosList; } }

    public Quaternion PlayerStartingRotation => Quaternion.Euler(0, 45f, 0);
    public Quaternion EnemyStartingRotation => Quaternion.Euler(0, 225f, 0);

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    // 플레이어, 몬스터 시작 좌표 초기화
    private void Init()
    {
        // COL - (x+3, 0, z-3
        // ROW - (x - 2, 0, z - 2)
        //Vector3 pPos = new Vector3(-6f, 0f, 9f);   // 플레이어 기준점
        Vector3 pPos = new Vector3(-10f, 0f, 5f);   // 플레이어 기준점
        Vector3 ePos = new Vector3(0.5f, 0f, 15.5f);   // 적 기준점

        Vector3 addColVector = new Vector3(3f, 0f, -3f);

        for (int i = 0; i < ROW; i++)
        {
            Vector3 addRowVector = new Vector3(-2f * i, 0f, -2f * i);

            Vector3 p = pPos + addRowVector;
            Vector3 e = ePos - addRowVector;    // 얘는 반대 방향으로
            
            // n행의 COL 추가
            for (int j = 0; j < COL; j++)
            {
                if (j > 0)
                {
                    p += addColVector;
                    e += addColVector;
                }

                _playerStartingPosList.Add(p);
                _enemyStartingPosList.Add(e);
            }
        }
    }

    void Start()
    {
        CreatePlacementPlane();
        SpawnPlayerCharacters();
        SpawnEnemies();
    }

    void Update()
    {
        if (_battleState == EBattleState.Start)
        {
            CheckPlayerStatus();
            CheckEnemiesStatus();
        }

        if (_battleState == EBattleState.Victory)
        {
            Debug.Log("승리");
        }

        if (_battleState == EBattleState.Defeat)
        {
            Debug.Log("패배");
        }
    }

    private void CheckPlayerStatus()
    {
        // 한명이라도 살아있으면 끝냄
        foreach (Unit unit in _gameObjectToUnit.Values)
        {
            if (!unit.IsDead)
            {
                return;
            }
        }

        _battleState = EBattleState.Defeat;
    }

    private void CheckEnemiesStatus()
    {
        // 한명이라도 살아있으면 끝냄
        foreach (Unit unit in _enemyGoToUnit.Values)
        {
            Debug.Log($"CheckEnemiesStatus - IsDead : {unit.IsDead}");
            if (!unit.IsDead)
            {
                Debug.Log($"살아있음");
                return;
            }
        }
        Debug.Log("모두 죽음");
        _battleState = EBattleState.Victory;
    }

    private void CreatePlacementPlane()
    {
        _placementRoot = new GameObject("PlacementRoot").transform;

        for (int i = 0; i < _playerStartingPosList.Count; i++)
        {
            GameObject go = Instantiate(_placementPlanePrefab, _playerStartingPosList[i] + (Vector3.up * 0.1f), PlayerStartingRotation);
            go.transform.SetParent(_placementRoot);
        }

        for (int i = 0; i < _enemyStartingPosList.Count; i++)
        {
            GameObject go = Instantiate(_placementPlaneEnemyPrefab, _enemyStartingPosList[i] + (Vector3.up * 0.1f), EnemyStartingRotation);
            go.transform.SetParent(_placementRoot);
        }

    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            GameObject go = Instantiate(_enemies[i]);
            _enemyGoToUnit[go] = go.GetComponent<Unit>();

            go.transform.position = _enemyStartingPosList[i];
            go.transform.rotation = EnemyStartingRotation;
        }
    }

    private void SpawnPlayerCharacters()
    {
        int mainNumber = DataSource.Instance.MainCharacterIdx;

        // 메인 캐릭터가 없는 경우 Shield_02 세팅
        if (mainNumber == -1)
        {
            mainNumber = (int)ECharacterNumber.Shield_02;
        }

        // 메인 캐릭터 세팅
        GameObject mainGo = _playerSpawner.SpawnPlayer(mainNumber, _playerStartingPosList[0], PlayerStartingRotation);
        _numberToGameObject[mainNumber] = mainGo;

        Unit mainUnit = mainGo.GetComponent<Unit>();
        mainGo.GetComponent<Collider>().enabled = false;    // 에너지가 탐지 못하도록 콜라이더 꺼놓음
        mainUnit.enabled = false;                           // 생성 해놓고 자동공격 꺼놓음 (탐지 안함)
        _gameObjectToUnit[mainGo] = mainUnit;


        // 메인 캐릭터 제외한 나머지 캐릭터 세팅
        List<int> numberList = DataSource.Instance.GetCharacterList();

        int startPositionIdx = 2;    // 0번은 메인 캐릭터
        for (int i = 0; i < numberList.Count; i++)
        {
            GameObject go = _playerSpawner.SpawnPlayer(numberList[i], _playerStartingPosList[i + startPositionIdx], PlayerStartingRotation);
            _numberToGameObject[numberList[i]] = go;

            Unit unit = go.GetComponent<Unit>();
            go.GetComponent<Collider>().enabled = false;    // 적 몬스터가 탐지 못하도록 콜라이더 꺼놓음
            unit.enabled = false;                           // 자동공격 꺼놓음
            _gameObjectToUnit[go] = unit;
        }
    }

    public void StartBattle()
    {
        _battleState = EBattleState.Start;

        foreach (Unit unit in _gameObjectToUnit.Values)
        {
            unit.enabled = true;    // 자동공격 켜기
            unit.GetComponent<Collider>().enabled = true;    // 콜라이더 켜기
        }
    }
}
