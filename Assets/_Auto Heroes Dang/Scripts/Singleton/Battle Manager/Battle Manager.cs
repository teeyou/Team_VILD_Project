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
public class BattleManager : Singleton<BattleManager>
{
    private const int ROW = 3;
    private const int COL = 3;

    private List<Vector3> _playerStartingPosList = new List<Vector3>();
    private List<Vector3> _enemyStartingPosList = new List<Vector3>();

    public bool IsStart { get; set; } = false;      // 시작 여부 (배치 후 스타트 버튼)
    public bool IsVictory { get; set; } = false;    // 배틀 승리 or 패배 여부
    public bool IsFinish { get; set; } = false;     // 배틀 종료 여부

    public GameObject[] _enemies;       // 테스트용 코드

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
        Vector3 pPos = new Vector3(-6f, 0f, 9f);   // 플레이어 기준점
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
        
        // 테스트용 코드
        //for (int i = 0; i < _enemies.Length; i++)
        //{
        //    GameObject go = Instantiate(_enemies[i]);
        //    go.transform.position = _enemyStartingPosList[i];
        //    go.transform.rotation = EnemyStartingRotation;
        //}
    }

    void Update()
    {
        
    }
}
