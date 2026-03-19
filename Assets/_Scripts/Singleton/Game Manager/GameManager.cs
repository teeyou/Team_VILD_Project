using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

- 플레이어 생성함
 
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
    public Transform PlayerTr { get; private set; } = null;
    private FieldAutoMove _autoMove;


    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayerTr = _playerSpawner.
            SpawnPlayer((int)ECharacterNumber.TwoHand_01, 
            _startPosition, 
            Quaternion.identity).transform;
    }

    private void Update()
    {
        
    }

    public void MoveNextPoint()
    {
        if (_autoMove == null)
        {
            _autoMove = PlayerTr.GetComponent<FieldAutoMove>();
        }

        // 이미 이동중이면 종료
        if (_autoMove.IsMoving)
        {
            return;
        }

        _autoMove.MoveNextPoint();
    }
}
