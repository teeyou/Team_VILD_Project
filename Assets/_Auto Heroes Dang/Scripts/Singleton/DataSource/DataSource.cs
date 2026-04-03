using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosRotData
{
    private Vector3 _pos;
    private Quaternion _rot;

    public Vector3 Pos { get { return _pos; } set { _pos = value; } }
    public Quaternion Rot { get { return _rot; } set { _rot = value; } }
    public PosRotData(Vector3 pos, Quaternion rot)
    {
        _pos = pos;
        _rot = rot;
    }
}

public class DataSource : Singleton<DataSource>
{
    [SerializeField] private List<BaseStatus_SO> _baseStatusList;

    public int MainCharacterIdx { get; set; } = -1;
    private List<int> _playerCharacterList = new List<int>();   // 메인 캐릭터 제외한 나머지 캐릭터 idx 저장

    public PosRotData MainCharacterPosRot { get; set; }

    private List<PosRotData> _subCharacterPosRotList = new List<PosRotData>();

    public int CurrentIdx { get; set; } = 0;    // track 웨이포인트의 idx

    public float CartPosition { get; set; } = 6.5f;

    public int Gem { get; set; } = 10;
    public int Gold { get; set; } = 1000;

    //private List<BaseStatus_SO> _copyStatusList = new List<BaseStatus_SO>();
    private List<PlayerRuntimeData> _playerRuntimeDataList = new List<PlayerRuntimeData>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        AddCharacter(ECharacterNumber.TwoHand_TU);
        AddCharacter(ECharacterNumber.SpearMan_JH);
        AddCharacter(ECharacterNumber.BowMan_JJ);

        AddCharacter(ECharacterNumber.Wizard_01);
        AddCharacter(ECharacterNumber.Healer_01);

        // SO 데이터 -> 런타임 데이터로 변환
        for (int i = 0; i < _baseStatusList.Count; i++)
        {
            _playerRuntimeDataList.Add(new PlayerRuntimeData(_baseStatusList[i]));
        }
    }

    public PlayerRuntimeData GetPlayerRuntimeData(int idx)
    {
        if (_playerRuntimeDataList.Count <= idx)
        {
            return null;
        }
        return _playerRuntimeDataList[idx];
    }

    public BaseStatus_SO GetBaseStatusSO(int idx)
    {
        if (_baseStatusList.Count <= idx)
        {
            return null;
        }

        return _baseStatusList[idx];
    }

    public List<int> GetCharacterList() => _playerCharacterList;

    public void AddCharacter(ECharacterNumber number)
    {
        _playerCharacterList.Add((int)number);
    }

    public void AddSubPosRot(Vector3 pos, Quaternion rot)
    {
        _subCharacterPosRotList.Add(new PosRotData(pos, rot));
    }

    public void SetSubPosRot(int idx, Vector3 pos, Quaternion rot)
    {
        _subCharacterPosRotList[idx].Pos = pos;
        _subCharacterPosRotList[idx].Rot = rot;
    }

    public PosRotData GetSubPosRot(int idx)
    {
        if (_subCharacterPosRotList.Count <= idx)
        {
            return null;
        }

        return _subCharacterPosRotList[idx];
    }
    public IReadOnlyList<PosRotData> GetSubPosRotList()
    {
        return _subCharacterPosRotList;
    }
}
