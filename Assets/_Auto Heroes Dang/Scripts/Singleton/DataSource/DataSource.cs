using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PosRotData
{
    [SerializeField] private Vector3 _pos;
    [SerializeField] private Quaternion _rot;

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

    public int CurrentIdx { get; set; } = 0;    // 트랙 웨이포인트 인덱스
    public float CartPosition { get; set; } = 6.5f;

    private int _gem = 10;
    private int _gold = 1000;

    public event Action OnCurrencyChanged;

    private SaveData _saveData = null;
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
            AudioManager.Instance.PlaySFX("CoinDrop");
            OnCurrencyChanged?.Invoke();
        }
    }

    private List<PlayerRuntimeData> _playerRuntimeDataList = new List<PlayerRuntimeData>();

    public GradeStatusTable_SO GradeStatusTable { get { return _gradeStatusTable; } }

    public int AtkBuff { get; set; } = 0;
    public int DefBuff { get; set; } = 0;

    public bool atkPotionOn { get; set; } // 0413 추가. 포션 버프 두줄.
    public bool defPotionOn { get; set; }

    SaveSystem _saveSystem = new SaveSystem();

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

    private void Start()
    {
        _saveData = Load();

        if (_saveData == null)
        {
            GameManager.Instance.IsSave = false;
            MakeInventoryData();
        }

        else
        {
            GameManager.Instance.IsSave = true;
            ApplyPotionData();
        }

        ShopManagerPotion.Instance.InitializePotion();
    }

    public void SetSaveData()
    {
        // 스타트씬에서 Continue 버튼 눌렀을 때 호출
        MainCharacterIdx = _saveData.mainCharacterIdx;
        _playerCharacterList = _saveData.playerCharacterList;
        MainCharacterPosRot = _saveData.mainTransform;
        _subCharacterPosRotList = _saveData.subTransformList;
        CartPosition = _saveData.cartPosition;
        CurrentIdx = _saveData.currentWayPointIdx;
        GameManager.Instance.CurrentStage = (EGameStage)_saveData.currentStage;
        _playerRuntimeDataList = _saveData.playerRuntimeDataList;
        GameManager.Instance.IsFirstPoint = _saveData.isFirstPoint;
        GameManager.Instance.IsStageClear = _saveData.isStageClear;
        // isSpawnPossible은 필드매니저에서 세팅
        MakeInventoryData(); // 0413 아이템 데이터 저장용
    }

    public SaveData GetSaveData => _saveData;

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

    public void DeleteSaveFile()
    {
        _saveSystem.Delete();
        _saveData = null;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        Gold += amount;

        if (SceneManager.GetActiveScene().name == ESceneId.FieldScene.ToString())
        {
            UIManager.Instance.ShowGoldGain(amount);
        }

        //if (UIManager.Instance != null)
        //{
        //    UIManager.Instance.ShowGoldGain(amount);
        //    Debug.Log("if DataSource AddGold");
        //}
        //else
        //{
        //    Debug.Log("else DataSource AddGold");
        //}
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

        if (SceneManager.GetActiveScene().name == ESceneId.FieldScene.ToString())
        {
            UIManager.Instance.ShowGemGain(amount);
        }

        //if (UIManager.Instance != null)
        //    UIManager.Instance.ShowGemGain(amount);
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

    public List<PlayerRuntimeData> GetPlayerRuntimeDataList()
    {
        return _playerRuntimeDataList;
    }

    public PlayerRuntimeData GetPlayerRuntimeData(int idx)
    {
        if (_playerRuntimeDataList.Count <= idx)
        {
            return null;
        }

        return _playerRuntimeDataList[idx];
    }

    public void InitializeBuffValue()
    {
        AtkBuff = 0;
        DefBuff = 0;
    }

    // 장비 착용, 아이템 사용 시 양수, 해제 시 음수
    public void IncreaseAtk(int value, bool usePotion = false)
    {
        if (usePotion)
        {
            AtkBuff = value;
        }

        for (int i = 0; i < _playerRuntimeDataList.Count; i++)
        {
            _playerRuntimeDataList[i].DefaultAtk += value;
        }
    }

    public void IncreaseDef(int value, bool usePotion = false)
    {
        if (usePotion)
        {
            DefBuff = value;
        }

        for (int i = 0; i < _playerRuntimeDataList.Count; i++)
        {
            _playerRuntimeDataList[i].DefaultDef += value;
        }
    }

    public void IncreaseHp(int value)
    {
        for (int i = 0; i < _playerRuntimeDataList.Count; i++)
        {
            _playerRuntimeDataList[i].DefaultMaxHp += value;
        }
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

        Save();
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

    public void UpdatePosRotList(int idx, Vector3 pos, Quaternion rot)
    {
        if (idx < _subCharacterPosRotList.Count)
        {
            // 이미 존재하면 덮어쓰기
            _subCharacterPosRotList[idx].Pos = pos;
            _subCharacterPosRotList[idx].Rot = rot;
        }
        else
        {
            // 없으면 추가
            _subCharacterPosRotList.Add(new PosRotData(pos, rot));
        }
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

    public List<PosRotData> GetSubTransformList()
    {
        return _subCharacterPosRotList;
    }

    public void Save()
    {
        // FieldManager Start에서 Save
        // FieldAutoMove Stop에서 Save
        // 레벨업 할 때 Save
        _saveSystem.Save();
    }

    public SaveData Load()
    {
        return _saveSystem.Load();
    }

    // 0413 아이템 저장용 추가
    public void MakeInventoryData()
    {
        InventoryManager.Instance.Clear();

        if ((_saveData == null || _saveData.inventoryItems == null || _saveData.inventoryItems.Count == 0) && IsEquipmentEmpty()) //  
        {
            // 게임 초기 아이템 추가
            InventoryManager.Instance.AddItemGrade(Grade.Common, ItemType.Sword);
            InventoryManager.Instance.AddItemGrade(Grade.Common, ItemType.Armor);
            InventoryManager.Instance.AddItemGrade(Grade.Common, ItemType.Hat);
            InventoryManager.Instance.AddItemGrade(Grade.Common, ItemType.Ring);
            InventoryManager.Instance.AddItemGrade(Grade.Common, ItemType.Shoes);

            return;
        }

        foreach (ItemData item in _saveData.inventoryItems)
        {
            InventoryManager.Instance.AddItem(item);
        }

        if (_saveData.equipments != null)
        {
            InventoryManager.Instance.SetEquipments(_saveData.equipments);

            /*
            스탯 적용. 현 구조에선 사용 안함
            foreach (ItemData item in _saveData.equipments)
            {
                if (item.uniqueId != 0)
                {
                    bool isAtk = item.type == ItemType.Sword || item.type == ItemType.Ring;

                    if (isAtk)
                        IncreaseAtk(item.value);
                    else
                        IncreaseDef(item.value);
                }
            }
            */
        }

    }

    private bool IsEquipmentEmpty()
    {
        if (_saveData == null || _saveData.equipments == null)
            return true;

        for (int i = 0; i < 5; i++)
        {
            if (_saveData.equipments[i].uniqueId != 0)
                return false;
        }

        return true;
    }

    private void ApplyPotionData()
    {
        if (_saveData == null)
            return;

        atkPotionOn = _saveData.atkPotionOn;
        defPotionOn = _saveData.defPotionOn;

        /*
        현 구조에선 사용 안함
        if (atkPotionOn) IncreaseAtk(10, true);
        if (defPotionOn) IncreaseDef(10, true);
        */
    }

}