using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

    [SerializeField] private GradeStatusTable_SO _gradeStatusTable;

    public int MainCharacterIdx { get; set; } = -1;
    private List<int> _playerCharacterList = new List<int>();

    public PosRotData MainCharacterPosRot { get; set; }

    private List<PosRotData> _subCharacterPosRotList = new List<PosRotData>();

    public int CurrentIdx { get; set; } = 0;
    public float CartPosition { get; set; } = 6.5f;

    private int _gem = 10;
    private int _gold = 1000;

    public event Action OnCurrencyChanged;

    public int Gem
    {
        get { return _gem; }
        set
        {
            int newValue = Mathf.Max(0, value);

            if (_gem == newValue)
                return;

            _gem = newValue;
            SaveCurrency();
            OnCurrencyChanged?.Invoke();
        }
    }

    public int Gold
    {
        get { return _gold; }
        set
        {
            int newValue = Mathf.Max(0, value);

            if (_gold == newValue)
                return;

            _gold = newValue;
            SaveCurrency();
            OnCurrencyChanged?.Invoke();
        }
    }

    private List<PlayerRuntimeData> _playerRuntimeDataList = new List<PlayerRuntimeData>();

    public GradeStatusTable_SO GradeStatusTable { get { return _gradeStatusTable; } }


    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        LoadCurrency();

        AddCharacter(ECharacterNumber.TwoHand_TU);
        AddCharacter(ECharacterNumber.SpearMan_JH);
        AddCharacter(ECharacterNumber.BowMan_JJ);
        AddCharacter(ECharacterNumber.Wizard_01);
        AddCharacter(ECharacterNumber.Healer_01);

        for (int i = 0; i < _baseStatusList.Count; i++)
        {
            _playerRuntimeDataList.Add(new PlayerRuntimeData(_baseStatusList[i]));
        }
    }

    private void LoadCurrency()
    {
        CurrencySaveData data = CurrencySaveSystem.Load();
        _gem = data.gem;
        _gold = data.gold;
    }

    private void SaveCurrency()
    {
        CurrencySaveSystem.Save(_gem, _gold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        Gold += amount;
    }

    public bool UseGold(int amount)
    {
        if (amount <= 0)
            return false;

        if (_gold < amount)
            return false;

        Gold -= amount;
        return true;
    }

    public void AddGem(int amount)
    {
        if (amount <= 0)
            return;

        Gem += amount;
    }

    public bool UseGem(int amount)
    {
        if (amount <= 0)
            return false;

        if (_gem < amount)
            return false;

        Gem -= amount;
        return true;
    }

    public PlayerRuntimeData GetPlayerRuntimeData(int idx)
    {
        if (_playerRuntimeDataList.Count <= idx)
        {
            return null;
        }

        return _playerRuntimeDataList[idx];
    }

    public void LevelUp(int idx, EGrade grade)
    {
        if (_playerRuntimeDataList.Count <= idx)
        {
            return;
        }

        int bonus = 0;

        if (grade == EGrade.S)
        {
            bonus = _gradeStatusTable.SLevelUpBonus;
        }

        else if (grade == EGrade.A)
        {
            bonus = _gradeStatusTable.ALevelUpBonus;
        }

        else if (grade == EGrade.B)
        {
            bonus = _gradeStatusTable.BLevelUpBonus;
        }

        // 체력은 10배, 공격력, 방어력은 그대로 증가
        _playerRuntimeDataList[idx].DefaultMaxHp += bonus * 10;
        _playerRuntimeDataList[idx].DefaultAtk += bonus;
        _playerRuntimeDataList[idx].DefaultDef += bonus;

        _playerRuntimeDataList[idx].Level++;

        Debug.Log($"{_playerRuntimeDataList[idx].ChName}" +
            $"\n{_playerRuntimeDataList[idx].DefaultMaxHp}" +
            $"\n{_playerRuntimeDataList[idx].DefaultAtk}" +
            $"\n{_playerRuntimeDataList[idx].DefaultDef}");

        Debug.Log("레벨업 완료");
    }

    public int GetLevelUpRequiredGold(int level, EGrade grade)
    {
        return _gradeStatusTable.GetLevelUpRequiredGold(level, grade);
    }

    public BaseStatus_SO GetBaseStatusSO(int idx)
    {
        if (_baseStatusList.Count <= idx)
            return null;

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
            return null;

        return _subCharacterPosRotList[idx];
    }

    public IReadOnlyList<PosRotData> GetSubPosRotList()
    {
        return _subCharacterPosRotList;
    }
}