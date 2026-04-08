using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldUI : MonoBehaviour
{
    [SerializeField] private GameObject _stagePanel;    // 전체 스테이지 패널 ( Enemy Panel / Boss Panel / 스타트 버튼)
    [SerializeField] private GameObject _enemy;         // Enemy Panel
    [SerializeField] private GameObject _boss;          // Boss Panel

    [SerializeField] private AnimateUI _uiAnimate;

    [SerializeField] private StagePanelUI _stagePanelUI; // 스테이지 패널 연결

    [SerializeField] private GameObject _enemySlotPrefab;
    [SerializeField] private Transform _enemySlotParent;

    [SerializeField] private TMP_Text _bossName; // 0409 추가
    [SerializeField] private Image _bossImage; // 보스는 이미지 위치에서 스프라이트만 바뀌는 식으로 구현
    [SerializeField] private Image _bossType;

    [SerializeField] private List<FieldUIEnemyData> enemyDB;  // 이름, 어택타입, 이미지
    [SerializeField] private List<FieldUIStageData> stageSettings; // 인스펙터에서 스테이지별로 세팅
    private Dictionary<FieldUIEnemyName, FieldUIEnemyData> _enemyDictionary; // 이름 데이터 매칭용

    [SerializeField] private Sprite _meleeSprite;
    [SerializeField] private Sprite _rangeSprite;
    [SerializeField] private Sprite _tankSprite;

    [SerializeField] private Image _bossAttackIcon;
    [SerializeField] private TMP_Text _bossAttackName;
    [SerializeField] private Image _bossSkillIcon;
    [SerializeField] private TMP_Text _bossSkillName;
    [SerializeField] private List<Sprite> _bossSkillIcons;
    [SerializeField] private List<string> _bossSkillTextList;     // 여기까지가 0409 추가분


    private AnimateUI _enemyAnimate;
    private AnimateUI _bossAnimate;

    private bool _isBoss = false;    // 현재 스테이지가 보스인지에 대한 여부

    private bool _isSetstageInfo = false;

    private void Awake()
    {
        if (!_stagePanel.activeSelf)     // <----- stagePanel 상시 On 상태로 변경
        {
            _stagePanel.SetActive(true);
        }

        if (_enemy != null) _enemyAnimate = _enemy.GetComponent<AnimateUI>();
        if (_boss != null) _bossAnimate = _boss.GetComponent<AnimateUI>();
    }

    //0409 추가 딕셔너리에서 데이터 넣기
    private void Start()
    {
        _enemyDictionary = new Dictionary<FieldUIEnemyName, FieldUIEnemyData>();

        for (int i = 0; i < enemyDB.Count; i++)
        {
            FieldUIEnemyData data = enemyDB[i];

            if (!_enemyDictionary.ContainsKey(data.enemyName))
            {
                _enemyDictionary.Add(data.enemyName, data);
            }
        }
    }


    // 스테이지 정보를 적을 내용을 만드는 곳. -----------> 완성 후 PopUpFieldInfo() 주석 해제할 것
    private void SetStagePanel()
    {
        if (!_isSetstageInfo)
        {
            _stagePanelUI.RefreshUI();

            _isSetstageInfo = true;

            SetStageInfo(GameManager.Instance.CurrentStage);
        }
    }

    // 스테이지 정보 팝업. 보스인지 여부에 따라 다른 패널로 출력
    public void PopUpFieldInfo()
    {
        int currentStage = (int)GameManager.Instance.CurrentStage;

        _isBoss = (currentStage % 3 == 2); // 스테이지 번호가 2, 5, 8인 경우 보스 스테이지

        SetStagePanel();

        if (_isBoss)
        {
            _enemy.SetActive(false);
            _boss.SetActive(true);
            _bossAnimate.PlayAnimate(0);
        }
        else
        {
            _enemy.SetActive(true);
            _boss.SetActive(false);
            _enemyAnimate.PlayAnimate(0);
        }

    }

    public void ToggleStagePanel()
    {
        if (_isBoss)
        {
            if (_bossAnimate != null) _bossAnimate.PlayAnimate(1);
        }
        else
        {
            if (_enemyAnimate != null) _enemyAnimate.PlayAnimate(1);
        }

    }

    public void SetStageInfo(EGameStage stage)
    {
        FieldUIStageData stageData = GetStageData(stage);

        if (stageData == null)
        {
            Debug.LogWarning("StageData 없음: " + stage);
            return;
        }

        if (!_isBoss)
        {
            MakeEnemySlots(stageData.enemy);
        }
        else 
        { 
            MakeBossSlot(stageData.enemy);
        }

    }


    // --------------------------------------------------------0409 추가분

    public FieldUIEnemyData GetEnemyData(FieldUIEnemyName name)
    {
        if (!_enemyDictionary.ContainsKey(name))
        {
            Debug.LogError($" 딕셔너리에 데이터 없음 : {name}");
            return null;
        }
        return _enemyDictionary[name];

    }

    private FieldUIStageData GetStageData(EGameStage stage)
    {
        for (int i = 0; i < stageSettings.Count; i++)
        {
            if (stageSettings[i].stage == stage)
                return stageSettings[i];
        }

        return null;
    }

    private Sprite GetAttackTypeSprite(AttackType type)
    {
        switch (type)
        {
            case AttackType.Melee:
                return _meleeSprite;

            case AttackType.Range:
                return _rangeSprite;

            case AttackType.Tank:
                return _tankSprite;
        }

        return null;
    }


    private void MakeEnemySlots(List<FieldUIEnemyAmount> enemyList)
    {
        // 초기화
        for (int i = 0; i < _enemySlotParent.childCount; i++)
        {
            Destroy(_enemySlotParent.GetChild(i).gameObject);
        }

        // 생성
        for (int i = 0; i < enemyList.Count; i++)
        {
            FieldUIEnemyAmount enemyAmount = enemyList[i];
            FieldUIEnemyData data = GetEnemyData(enemyAmount.enemyName);

            if (data == null)
            {
                Debug.LogWarning("EnemyData 없음: " + enemyAmount.enemyName);
                continue;
            }

            GameObject obj = Instantiate(_enemySlotPrefab, _enemySlotParent);

            Image[] image = obj.GetComponentsInChildren<Image>();
            TMP_Text[] tmp = obj.GetComponentsInChildren<TMP_Text>();

            for (int j = 0; j < image.Length; j++)
            {
                if (image[j].name == "Enemy Icon")
                {
                    image[j].sprite = data.enemySprite;
                    image[j].SetNativeSize();           // <---------- 이미지 깨짐 방지 보험 
                }
                else if (image[j].name == "Icon")
                {
                    image[j].sprite = GetAttackTypeSprite(data.attackType);
                }
            }

            for (int j = 0; j < tmp.Length; j++)
            {
                if (tmp[j].name == "Level Text")
                {
                    tmp[j].text = enemyAmount.amount;
                }
            }
        }

    }

    private void MakeBossSlot(List<FieldUIEnemyAmount> boss) 
    {
        FieldUIEnemyData data = GetEnemyData(boss[0].enemyName); // 어차피 보스 하나만 있기 때문.

        string name = data.enemyName.ToString();
        _bossName.text = name.Split('_')[0];

        _bossImage.sprite = data.enemySprite;
        _bossImage.SetNativeSize();
        _bossType.sprite = GetAttackTypeSprite(data.attackType);


        int bossIndex = ((int)GameManager.Instance.CurrentStage)/3;

        _bossAttackIcon.sprite = _bossSkillIcons[bossIndex*2];
        _bossAttackIcon.SetNativeSize();
        _bossAttackName.text = _bossSkillTextList[bossIndex * 2];

        _bossSkillIcon.sprite = _bossSkillIcons[bossIndex * 2 + 1];
        _bossSkillIcon.SetNativeSize();
        _bossSkillName.text = _bossSkillTextList[bossIndex * 2 + 1];

    }

    // -------공부는 되어서 좋았는데, 노가다는 똑같은데요????
}