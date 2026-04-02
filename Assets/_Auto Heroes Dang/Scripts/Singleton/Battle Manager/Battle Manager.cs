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
    Finish,
}

public class BattleManager : Singleton<BattleManager>
{
    private const int ROW = 3;
    private const int COL = 3;

    [SerializeField] private PlayerSpawner _playerSpawner;
    [SerializeField] private GameObject[] _enemies;

    [SerializeField] private GameObject _placementPlanePrefab;
    [SerializeField] private GameObject _placementPlaneEnemyPrefab;

    private GameObject _selectedCharacter;
    private Vector3 _originPos;

    private Camera _cam;

    private Transform _placementRoot;
    private List<GameObject> _playerPlacementList = new List<GameObject>();

    private List<Vector3> _playerStartingPosList = new List<Vector3>();
    private List<Vector3> _enemyStartingPosList = new List<Vector3>();

    private List<GameObject> _playerCharacterList = new List<GameObject>();
    private Dictionary<GameObject, Unit> _gameObjectToUnit = new Dictionary<GameObject, Unit>();

    private List<GameObject> _enemyList = new List<GameObject>();
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
        _cam = Camera.main;

        CreatePlacementPlane();
        SpawnPlayerCharacters();
        SpawnEnemies();

        BattleUIManager.Instance.UpdateTotalHp(true, _playerCharacterList, _gameObjectToUnit);
        BattleUIManager.Instance.UpdateTotalHp(false, _enemyList, _enemyGoToUnit);

        // 시작 전 플레이어 / 적의 토탈 hp바 최대값 초기화
        BattleUIManager.Instance.InitBattleTotalHpBar(_playerCharacterList, _gameObjectToUnit, _enemyList, _enemyGoToUnit);


        BattleUIManager.Instance.SetSKillUI(_playerCharacterList, _gameObjectToUnit);
    }

    void Update()
    {
        if (_battleState == EBattleState.None)
        {
            DragAndDropMove();
        }

        if (_battleState == EBattleState.Start)
        {
            CheckPlayerStatus();
            CheckEnemiesStatus();
        }

        if (_battleState == EBattleState.Victory)
        {
            ResetTimeScale();

            _battleState = EBattleState.Finish;
            BattleUIManager.Instance.ShowResultPanel(true);

            CreateMVP();

            GameManager.Instance.IsStageStart = false;

            DestroyAll();
        }

        if (_battleState == EBattleState.Defeat)
        {
            ResetTimeScale();

            _battleState = EBattleState.Finish;
            BattleUIManager.Instance.ShowResultPanel(false);

            GameManager.Instance.IsStageStart = false;

            DestroyAll();
        }

    }

    public void UseSkill(int idx)
    {
        if (BattleUIManager.Instance.TrySkill(2)) // 모든 스킬 2칸 소모한다고 가정
        {
            _playerCharacterList[idx].GetComponent<Unit>().IsSkillUsed = true;
        }

        else
        {
            BattleUIManager.Instance.PopUpToastMessage(); // <- 스킬 게이지 부족 메시지 팝업. 0402 추가 진주
            // 스킬 게이지 부족
            Debug.Log("스킬 게이지 부족");
        }
        
    }

    private void CreateMVP()
    {
        Vector3 pos = new Vector3(4.44f, -3.5f, -80f);

        GameObject go = _playerSpawner.SpawnPlayer(GetMVP(), pos, Quaternion.identity);

        go.transform.localScale = Vector3.one * 4f;
    }

    private void DragAndDropMove()
    {
        // 드래그 앤 드랍으로 캐릭터 이동
        if (InputManager.Instance.IsMouseLeftDown)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    _selectedCharacter = hit.collider.gameObject;
                    _originPos = _selectedCharacter.transform.position;
                }
            }
        }

        // 캐릭터가 선택되었을 때 마우스 이동에 따라 캐릭터 이동
        if (InputManager.Instance.IsMouseLeftStay && _selectedCharacter != null)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                _selectedCharacter.transform.position = hitPoint;
            }
        }

        if (InputManager.Instance.IsMouseLeftUp && _selectedCharacter != null)
        {
            Vector3 pos = _originPos;
            Transform tr = null;

            foreach (GameObject go in _playerPlacementList)
            {
                float dist = Vector3.Distance(_selectedCharacter.transform.position, go.transform.position);

                if (dist < 1f)
                {
                    tr = go.transform;
                    pos = tr.position;
                }
            }

            if (pos != _originPos)
            {
                // 해당 슬롯에 이미 캐릭터가 있으면 자리 교체

                foreach (GameObject go in _playerCharacterList)
                {
                    if (_selectedCharacter == go)
                    {
                        continue;
                    }

                    float dist = Vector3.Distance(go.transform.position, pos);

                    if (dist < 1f)
                    {
                        go.transform.position = _originPos;
                        break;
                    }
                }

            }

            _selectedCharacter.transform.position = pos;
            _selectedCharacter = null;
        }
    }

    private void DestroyAll()
    {
        for (int i = 0; i < _playerCharacterList.Count; i++)
        {
            Destroy(_playerCharacterList[i]);
        }

        for (int i = 0; i < _enemyList.Count; i++)
        {
            Destroy(_enemyList[i]);
        }
    }

    private int GetMVP()
    {
        Unit mvp = null;
        int maxDamage = 0;

        foreach (Unit unit in _gameObjectToUnit.Values)
        {
            Debug.Log($"{unit.name} : {unit.TotalDamage}");
            if (unit.TotalDamage > maxDamage)
            {
                maxDamage = unit.TotalDamage;
                mvp = unit;
            }
        }

        return mvp.CharacterNumber;
    }

    private void CheckPlayerStatus()
    {
        BattleUIManager.Instance.IncreaseSKillGauge();
        BattleUIManager.Instance.UpdateSkillCool(_playerCharacterList, _gameObjectToUnit);

        foreach (Unit unit in _gameObjectToUnit.Values)
        {
            BattleUIManager.Instance.UpdateCurrentHp(unit.gameObject, unit);
        }

        int totalHp = BattleUIManager.Instance.UpdateTotalHp(true, _playerCharacterList, _gameObjectToUnit);

        // 플레이어 TotalHpBar 갱신 
        BattleUIManager.Instance.RefreshBattleTotalHpBar(_playerCharacterList, _gameObjectToUnit, _enemyList, _enemyGoToUnit);

        // 모두 죽었으면 패배
        if (totalHp <= 0)
        {
            _battleState = EBattleState.Defeat;
        }

    }

    private void CheckEnemiesStatus()
    {
        // 모두 죽으면 승리
        int totalHp = BattleUIManager.Instance.UpdateTotalHp(false, _enemyList, _enemyGoToUnit);

        // 몬스터 TotalHpBar 갱신
        BattleUIManager.Instance.RefreshBattleTotalHpBar(_playerCharacterList, _gameObjectToUnit, _enemyList, _enemyGoToUnit);

        if (totalHp <= 0)
        {
            _battleState = EBattleState.Victory;
        }
    }

    private void CreatePlacementPlane()
    {
        _placementRoot = new GameObject("PlacementRoot").transform;

        // 플레이어 배치 장소
        for (int i = 0; i < _playerStartingPosList.Count; i++)
        {
            GameObject go = Instantiate(_placementPlanePrefab, _playerStartingPosList[i] + (Vector3.up * 0.1f), PlayerStartingRotation);
            go.transform.SetParent(_placementRoot);
            _playerPlacementList.Add(go);
        }

        // 적 배치 장소
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
            if (_enemies[i] == null)
            {
                continue;
            }

            GameObject go = Instantiate(_enemies[i]);
            _enemyList.Add(go);

            Unit unit = go.GetComponent<Unit>();
            
            Debug.Log($"unit : {unit}, Hp : {unit.MaxHp} / {unit.CurHp}");

            _enemyGoToUnit[go] = unit;

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
        _playerCharacterList.Add(mainGo);

        Unit mainUnit = mainGo.GetComponent<Unit>();
        //mainUnit.enabled = false;
        _gameObjectToUnit[mainGo] = mainUnit;


        // 메인 캐릭터 제외한 나머지 캐릭터 세팅
        List<int> numberList = DataSource.Instance.GetCharacterList();

        int startPositionIdx = 1;    // 0번은 메인 캐릭터
        for (int i = 0; i < numberList.Count; i++)
        {
            GameObject go = _playerSpawner.SpawnPlayer(numberList[i], _playerStartingPosList[i + startPositionIdx], PlayerStartingRotation);
            _playerCharacterList.Add(go);

            Unit unit = go.GetComponent<Unit>();
            //unit.enabled = false;
            _gameObjectToUnit[go] = unit;
        }

        // UI에 캐릭터 슬롯 생성
        BattleUIManager.Instance.CreateChSlot(_playerCharacterList, _gameObjectToUnit);
    }

    public void StartBattle()
    {
        _battleState = EBattleState.Start;

        GameManager.Instance.IsStageStart = true;
      
        _placementRoot.gameObject.SetActive(false);    // 배치 플레인 숨김
    }

    private void ResetTimeScale()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
    }
}
