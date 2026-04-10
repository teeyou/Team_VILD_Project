using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Button _autoProgressButton;
    [SerializeField] private Button _startStageButtonEnemy;
    [SerializeField] private Button _startStageButtonBoss;
    [SerializeField] private Button _stageButton;
    [SerializeField] private Button _enemyPanelBackButton;
    [SerializeField] private Button _bossPanelBackButton;
    [SerializeField] private FieldUI _fieldUI;

    [Header("왼쪽 하단 스테이터스")]
    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private Button _statusButton;
    [SerializeField] private Transform _slotParent;
    //[SerializeField] private GameObject _chSlotPrefab;

    [Header("오른쪽 상단 재화")]
    [SerializeField] private TMP_Text _gemText;
    [SerializeField] private TMP_Text _goldText;

    [Header("캐릭터 스테이터스 팝업 패널")]
    [SerializeField] private GameObject _detailStatusPanel;

    [Header("보상 아이콘 생성 부모")]
    [SerializeField] private RectTransform _rewardCanvasRoot;

    [Header("상자 보상 아이콘 도착 위치")]
    [SerializeField] private RectTransform _goldTargetUI;
    [SerializeField] private RectTransform _gemTargetUI;

    [Header("왼쪽 상단 메인 캐릭터 UI")]
    [SerializeField] private Image _mainCharacterImage;
    [SerializeField] private TMP_Text _totalCpTMP;

    [Header("토스트 메세지")]
    [SerializeField] private GameObject _toastMessageGo;
    [SerializeField] private TMP_Text _toastMessage;

    [Header("왼쪽 하단 인벤토리, 상점 UI")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _shopButton;

    public Vector3 GoldTargetUIPosition => _goldTargetUI.position;
    public Vector3 GemTargetUIPosition => _gemTargetUI.position;

    private TMP_Text _detailLevelTMP;
    private TMP_Text _detailCpTMP;
    private TMP_Text _detailAtkTMP;
    private TMP_Text _detailDefTMP;
    private TMP_Text _detailRequiredGoldTMP;

    private TMP_Text[] _slotLevelTMPs = new TMP_Text[6];

    [SerializeField] private SkillDescriptor _skillDescriptor;

    private Coroutine _levelUpButtonRoutine = null;
    protected override void Awake()
    {
        base.Awake();

        _autoProgressButton.onClick.AddListener(MovePlayer);
        _startStageButtonEnemy.onClick.AddListener(() => LoadBattleScene(GameManager.Instance.CurrentStage));
        _startStageButtonBoss.onClick.AddListener(() => LoadBattleScene(GameManager.Instance.CurrentStage));
        _stageButton.onClick.AddListener(ShowStagePanel);
        _enemyPanelBackButton.onClick.AddListener(CloseStagePanel);
        _bossPanelBackButton.onClick.AddListener(CloseStagePanel);

        _statusButton.onClick.AddListener(() => {
            SetSlot();
            _statusPanel.SetActive(true);
        });

        _gemText.text = DataSource.Instance.Gem.ToString();
        _goldText.text = DataSource.Instance.Gold.ToString();
    }

    private void Start()
    {
        RefreshCurrencyUI();

        if (GameManager.Instance.IsFirstPoint)
        {
            Debug.Log("FirstPoint");
            ToggleProgressButton(true);
            ToggleStageButton(false);

            return;
        }

        if (GameManager.Instance.IsStageClear)
        {
            Debug.Log("StageClear");
            ToggleProgressButton(true);
            ToggleStageButton(false);
        }

        else
        {
            Debug.Log("Defeat");
            ToggleProgressButton(false);
            ToggleStageButton(true);
        }

    }

    private void Update()
    {
        if (_mainCharacterImage.sprite == null)
        {
            SetProfile();
        }

        if (InputManager.Instance.IsPressedS)
        {
            if (_statusPanel.activeSelf)
            {
                _statusPanel.SetActive(false);
            }

            else
            {
                SetSlot();
                _statusPanel.SetActive(true);
            }
        }

        if (InputManager.Instance.IsPressedI)
        {
            // 인벤토리 열기
            if (_inventoryPanel.activeSelf)
            {
                _inventoryPanel.SetActive(false);
            }

            else
            {
                _inventoryPanel.SetActive(true);
            }
        }

        if (InputManager.Instance.IsPressedH)
        {
            // 아이템 상점 열기
            if (_shopPanel.activeSelf)
            {
                _shopPanel.SetActive(false);
            }

            else
            {
                _shopPanel.SetActive(true);
            }
        }
    }

    private void SetProfile()
    {
        _mainCharacterImage.sprite = Resources.Load<Sprite>($"CharacterSprite/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.MainCharacterIdx).ChName}");

        UpdateTotalCp();
    }

    private void UpdateTotalCp()
    {
        int totalCp = 0;

        // 메인 캐릭터
        PlayerRuntimeData mainData = DataSource.Instance.GetPlayerRuntimeData((DataSource.Instance.MainCharacterIdx));
        totalCp += CPCalculator.CalculateCP(mainData.DefaultAtk, mainData.DefaultDef, mainData.DefaultMaxHp);

        // 서브 캐릭터
        for (int i = 0; i < DataSource.Instance.GetCharacterList().Count; i++)
        {
            PlayerRuntimeData data = DataSource.Instance.GetPlayerRuntimeData((DataSource.Instance.GetCharacterList()[i]));
            totalCp += CPCalculator.CalculateCP(data.DefaultAtk, data.DefaultDef, data.DefaultMaxHp);
        }

        _totalCpTMP.text = totalCp.ToString();
    }

    public void RefreshCurrencyUI()
    {
        if (_gemText != null)
            _gemText.text = DataSource.Instance.Gem.ToString();

        if (_goldText != null)
            _goldText.text = DataSource.Instance.Gold.ToString();
    }

    private void OnEnable()
    {
        if (DataSource.Instance != null)
            DataSource.Instance.OnCurrencyChanged += RefreshCurrencyUI;
    }

    private void OnDisable()
    {
        if (DataSource.Instance != null)
            DataSource.Instance.OnCurrencyChanged -= RefreshCurrencyUI;
    }

    private void SetSlot()
    {
        int count = _slotParent.childCount;

        for (int i = 0; i < count; i++)
        {
            int idx = i;

            // Slot 버튼 리스너 추가 및 디테일 패널 이미지, 텍스트 세팅
            _slotParent.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                int requiredGold = 0;

                Image[] images = _detailStatusPanel.GetComponentsInChildren<Image>();
                TMP_Text[] tmps = _detailStatusPanel.GetComponentsInChildren<TMP_Text>();

                // 각 캐릭터 별 스프라이트 세팅
                for (int i = 0; i < images.Length; i++)
                {
                    // 캐릭터 이미지
                    if (images[i].name == "Sprite")
                    {
                        if (idx == 0)
                        {
                            images[i].sprite = Resources.Load<Sprite>($"CharacterSprite/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.MainCharacterIdx).ChName}");
                        }
                        else
                        {
                            images[i].sprite = Resources.Load<Sprite>($"CharacterSprite/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[idx - 1]).ChName}");
                        }
                    }

                    // 공격 아이콘
                    else if(images[i].name == "AttackIcon")
                    {
                        if (idx == 0)
                        {
                            images[i].sprite = Resources.Load<Sprite>($"SkillIcon/Shield_01_Attack");
                        }
                        else
                        {
                            images[i].sprite = Resources.Load<Sprite>($"SkillIcon/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[idx - 1]).ChName}_Attack");
                        }
                    }

                    // 스킬 아이콘
                    else if (images[i].name == "SkillIcon")
                    {
                        if (idx == 0)
                        {
                            images[i].sprite = Resources.Load<Sprite>($"SkillIcon/Shield_01_Skill");
                        }
                        else
                        {
                            images[i].sprite = Resources.Load<Sprite>($"SkillIcon/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[idx - 1]).ChName}_Skill");
                        }
                    }
                }

                // 각 캐릭터별 텍스트 세팅
                for (int i = 0; i < tmps.Length; i++)
                {
                    PlayerRuntimeData data;
                    if (idx == 0)
                    {
                        data = DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.MainCharacterIdx);
                    }
                    else
                    {
                        data = DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[idx - 1]);
                    }

                    if (tmps[i].name == "Rank")
                    {
                        tmps[i].color = data.Grade switch
                        {
                            EGrade.S => Color.yellow,
                            EGrade.A => Color.red,
                            EGrade.B => Color.blue,
                            _ => Color.white
                        };

                        tmps[i].text = data.Grade.ToString();
                    }
                    
                    else if (tmps[i].name == "Level")
                    {
                        _detailLevelTMP = tmps[i];
                        tmps[i].text = data.Level.ToString();
                    }
                    
                    else if (tmps[i].name == "CP")
                    {
                        _detailCpTMP = tmps[i];
                        string cp = CPCalculator.CalculateCP(data.DefaultAtk, data.DefaultDef, data.DefaultMaxHp).ToString();
                        tmps[i].text = cp;
                    }
                    
                    else if (tmps[i].name == "ATK")
                    {
                        _detailAtkTMP = tmps[i];
                        tmps[i].text = data.DefaultAtk.ToString();
                    }
                    
                    else if (tmps[i].name == "DEF")
                    {
                        _detailDefTMP = tmps[i];
                        tmps[i].text = data.DefaultDef.ToString();
                    }

                    else if (tmps[i].name == "Required Gold")
                    {
                        _detailRequiredGoldTMP = tmps[i];
                        requiredGold = DataSource.Instance.GetLevelUpRequiredGold(data.Level, data.Grade);
                        tmps[i].text = requiredGold.ToString();
                    }

                    else if (tmps[i].name == "Attack Description")
                    {
                        tmps[i].text = _skillDescriptor.GetAttackDescription(idx);
                    }

                    else if (tmps[i].name == "Skill Description")
                    {
                        tmps[i].text = _skillDescriptor.GetSkillDescription(idx);
                    }

                    else if (tmps[i].name == "Character Name")
                    {
                        tmps[i].text = data.DisplayName;
                    }
                }


                Button[] buttons = _detailStatusPanel.GetComponentsInChildren<Button>();

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].name == "Level Up Button")
                    {
                        int btnIdx = i;

                        // 기존 리스너 제거
                        buttons[btnIdx].onClick.RemoveAllListeners();

                        buttons[btnIdx].onClick.AddListener(() =>
                        {
                            // 레벨업 로직
                            if (DataSource.Instance.Gold >= requiredGold)
                            {
                                if (_levelUpButtonRoutine == null)
                                {
                                    DataSource.Instance.Gold -= requiredGold;

                                    int chIdx = -1;
                                    PlayerRuntimeData data;
                                    if (idx == 0)
                                    {
                                        chIdx = DataSource.Instance.MainCharacterIdx;
                                    }

                                    else
                                    {
                                        chIdx = DataSource.Instance.GetCharacterList()[idx - 1];
                                    }

                                    data = DataSource.Instance.GetPlayerRuntimeData(chIdx);
                                    DataSource.Instance.LevelUp(chIdx, data.Grade);

                                    RefreshUI(idx, data, _detailLevelTMP, _detailCpTMP, _detailAtkTMP, _detailDefTMP, _detailRequiredGoldTMP);

                                    _levelUpButtonRoutine = StartCoroutine(DelayLevelUpButton());
                                }

                                else
                                {
                                    PopUpToastMessage("잠시 후 눌러주세요.", 1f);
                                }


                                
                            }

                            else
                            {
                                PopUpToastMessage("골드가 부족합니다.", 1f);
                            }

                        });
                    }
                }

                _detailStatusPanel.SetActive(true);

            } // Slot 버튼 리스너 끝
            
            ); // _slotParent 반복문 끝
        }


        // Slot의 이미지, 텍스트 세팅
        for (int i = 0; i < count; i++)
        {
            // Slot 이미지 세팅
            Image[] images = _slotParent.GetChild(i).GetComponentsInChildren<Image>();
            
            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].name == "Character Sprite")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>($"CharacterSprite/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.MainCharacterIdx).ChName}");
                    }

                    else
                    {
                        images[j].sprite = Resources.Load<Sprite>($"CharacterSprite/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[i-1]).ChName}");
                    }
                }

                if (images[j].name == "Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>($"AttackTypeIcon/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.MainCharacterIdx).AttackType.ToString()}");
                    }
                    else
                    {
                        images[j].sprite = Resources.Load<Sprite>($"AttackTypeIcon/{DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[i - 1]).AttackType.ToString()}");
                    }
                }
            }

            // Slot 텍스트 세팅
            TMP_Text[] tmps = _slotParent.GetChild(i).GetComponentsInChildren<TMP_Text>();
            
            for (int j = 0; j < tmps.Length; j++)
            {
                PlayerRuntimeData data;

                if (i == 0)
                {
                    data = DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.MainCharacterIdx);
                }
                else
                {
                    data = DataSource.Instance.GetPlayerRuntimeData(DataSource.Instance.GetCharacterList()[i - 1]);
                }

                if (tmps[j].name == "Rank")
                {

                    tmps[j].text = data.Grade.ToString();
                    tmps[j].color = data.Grade switch
                    {
                        EGrade.S => Color.yellow,
                        EGrade.A => Color.red,
                        EGrade.B => Color.blue,
                        _ => Color.white
                    };
                }

                else if (tmps[j].name == "Level")
                {
                    _slotLevelTMPs[i] = tmps[j];
                    tmps[j].text = $"Lv. {data.Level.ToString()}";
                }

                else if (tmps[j].name == "Character Name")
                {
                    tmps[j].text = data.DisplayName;
                }
            }
        }
    }

    private IEnumerator DelayLevelUpButton()
    {
        yield return new WaitForSeconds(0.5f);

        _levelUpButtonRoutine = null;
    }

    private void RefreshUI(int idx, PlayerRuntimeData data, TMP_Text levelTMP, TMP_Text CpTMP, TMP_Text atkTMP, TMP_Text defTMP, TMP_Text requiredGoldTMP)
    {
        // 캐릭터 디테일 화면 텍스트 업데이트
        levelTMP.text = data.Level.ToString();
        CpTMP.text = CPCalculator.CalculateCP(data.DefaultAtk, data.DefaultDef, data.DefaultMaxHp).ToString();
        atkTMP.text = data.DefaultAtk.ToString();
        defTMP.text = data.DefaultDef.ToString();
        requiredGoldTMP.text = DataSource.Instance.GetLevelUpRequiredGold(data.Level, data.Grade).ToString();

        // 슬롯 레벨 텍스트 업데이트
        if (_slotLevelTMPs[idx] != null)
        {
            _slotLevelTMPs[idx].text = $"Lv. {data.Level.ToString()}";
        }

        // 필드씬 왼쪽상단 토탈 CP 업데이트
        UpdateTotalCp();
    }

    private void MovePlayer()
    {
        ToggleProgressButton(false);
        FieldManager.Instance.MoveNextPoint();
    }

    public void ToggleProgressButton(bool flag)
    {
        _autoProgressButton.gameObject.SetActive(flag);
    }

    public void ToggleStageButton(bool flag)
    {
        _stageButton.gameObject.SetActive(flag);
    }

    private void LoadBattleScene(EGameStage stage)
    {
        FieldManager.Instance.IsSpawnPossible = false; // 스테이지로 씬 전환 전에 스폰 불가능하도록 막음

        SceneLoader.Instance.LoadScene(stage);
    }

    public void ShowStagePanel()
    {
        ToggleStageButton(false);
        _fieldUI.PopUpFieldInfo();
    }

    public void CloseStagePanel()
    {
        _fieldUI.ToggleStagePanel();
        ToggleStageButton(true);
    }

    public RectTransform GetRewardCanvasRoot()
    {
        return _rewardCanvasRoot;
    }

    public void PopUpToastMessage(string msg, float duration)
    {
        if (_toastMessageGo == null) 
            return;

        AnimateUI animate = _toastMessageGo.GetComponent<AnimateUI>();
        animate.SetAnimDuration(duration);

        _toastMessage.text = msg;
        _toastMessageGo.SetActive(true);


        if (animate != null)
        {
            animate.ResetAnimate();
            animate.PlayAnimate(0);
        }
    }
}
