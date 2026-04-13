using GAP_LaserSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleUIManager : Singleton<BattleUIManager>
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _pauseButtonPause;
    [SerializeField] private GameObject _pauseButtonPlay;

    [SerializeField] private Button _speedButton;
    [SerializeField] private GameObject _speedButtonAccel;
    [SerializeField] private GameObject _speedButtonConstant;

    [SerializeField] private Button _backButton;
    [SerializeField] private Button _startButton;

    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] public GameObject _resultBlockPanel; // 빅토리 3초 대기용. 0406 진주

    [SerializeField] private Button _defeatButton;
    [SerializeField] private Button _victoryButton;

    [Header("결과창 골드 및 젬")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _goldText;
    [SerializeField] private TMP_Text _gemText;

    [Header("캐릭터 정보 UI")]
    [SerializeField] private Transform _chSlotParent;   // Character Group
    [SerializeField] private GameObject _chSlotPrefab;

    private Dictionary<GameObject, TMP_Text> _goToCurHpTmp = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<GameObject, TMP_Text> _goToAtkTmp = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<GameObject, TMP_Text> _goToDefTmp = new Dictionary<GameObject, TMP_Text>();

    [Header("상단 중앙 체력 난이도 UI")]
    [SerializeField] private TMP_Text _playerTotalHpTMP;
    [SerializeField] private TMP_Text _enemyTotalHpTMP;
    [SerializeField] private Image _difficulty;
    [SerializeField] private BattleTotalHpBar _battleTotalHpBar;

    [Header("스킬 UI")]
    [SerializeField] private Slider _skillSlider;
    [SerializeField] private Button[] _skillButtons;

    [SerializeField] private float _skillGaugeIncreaseRate = 0.001f;

    [Header("토스트 UI")]
    [SerializeField] private GameObject _toastUI;   // 스킬 게이지 부족 띄우는 거
    [SerializeField] private TMP_Text _toastText;   // 필요 시 메시지 수정해서 사용합니다.

    [Header("스킬 사용시 활성화 패널")]
    [SerializeField] private GameObject _skillPanel;

    [Header("월드 스페이스 스테이지 넘버")]
    [SerializeField] private string _stageNumberText;
    [SerializeField] private TMP_Text _worldSpaceStageNumberTMP;

    [Header("설정버튼, 캐릭터 상태 버튼")]
    //[SerializeField] private Button _configButton;
    [SerializeField] private Button _statusButton;

    //private Dictionary<GameObject, Image> _goToSkillIcon = new Dictionary<GameObject, Image>();
    private Dictionary<GameObject, Image> _goToSkillMask = new Dictionary<GameObject, Image>();
    private Dictionary<GameObject, TMP_Text> _goToSkillCoolTimeTmp = new Dictionary<GameObject, TMP_Text>();

    void Start()
    {
        for (int i = 0; i < _skillButtons.Length; i++)
        {
            _skillButtons[i].interactable = false;

            int idx = i;
            _skillButtons[i].onClick.AddListener(() =>
            {
                BattleManager.Instance.UseSkill(idx);
            });
        }

        _pauseButton.onClick.AddListener(() =>
        {
            if (Time.timeScale == 0f)
            {
                BattleManager.Instance.IsPause = false;
                Time.timeScale = BattleManager.Instance.CurrentTimeScale;

                _pauseButtonPause.SetActive(true);
                _pauseButtonPlay.SetActive(false); // 0403 조교님 피드백으로 추가 - 진주 (이하 주석 생략)
                
            }
            else
            {
                BattleManager.Instance.IsPause = true;
                Time.timeScale = 0f;

                _pauseButtonPause.SetActive(false);
                _pauseButtonPlay.SetActive(true); // 0403 조교님 피드백으로 추가 - 진주 (이하 주석 생략)
            }
        });

        _speedButton.onClick.AddListener(() =>
        {
            if (BattleManager.Instance.IsPause)
            {
                return;
            }

            if (Time.timeScale == 1f)
            {
                BattleManager.Instance.CurrentTimeScale = 2f;
                Time.timeScale = 2f;

                _speedButtonAccel.SetActive(false);
                _speedButtonConstant.SetActive(true);

            }
            else
            {
                BattleManager.Instance.CurrentTimeScale = 1f;
                Time.timeScale = 1f;

                _speedButtonAccel.SetActive(true);
                _speedButtonConstant.SetActive(false);
            }
        });

        _backButton.onClick.AddListener(ReturnField);

        _startButton.onClick.AddListener(() =>
        {
            _startButton.gameObject.SetActive(false);
            
            ShowStageNumber();                          // 코루틴 내부에서 GameUI 활성화 및 BattleStart 관련 호출
            
        });

        _defeatButton.onClick.AddListener(ReturnField);

        _victoryButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.CurrentStage == EGameStage.Stage3_3)
            {
                // 최종 스테이지 클리어 시, 엔딩씬
                SceneLoader.Instance.LoadScene(ESceneId.CharacterCapture);
                return;
            }

            GameManager.Instance.IncreaseCurrentStage();
            ReturnField();

        });

        if (_toastUI.activeSelf) _toastUI.SetActive(false); // 토스트 애니메이션용 한줄 추가. 0401 진주

        PopUpToastMessage("마우스로 위치를 변경할 수 있어요.");
    }

    public void CreateChSlot(List<GameObject> chList, Dictionary<GameObject, Unit> goToUnit)
    {
        for (int i = 0; i < chList.Count; i++)
        {
            GameObject slot = Instantiate(_chSlotPrefab);
            slot.transform.SetParent(_chSlotParent);
            slot.transform.localScale = Vector3.one;
            slot.transform.localPosition = new Vector3(slot.transform.localPosition.x, slot.transform.localPosition.y, 0f);
            slot.transform.localRotation = Quaternion.identity;

            Unit unit = goToUnit[chList[i]];
            GameObject ch = chList[i];

            // 슬롯 안에 있는 캐릭터 스프라이트 세팅
            Image[] images = slot.GetComponentsInChildren<Image>();

            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].name == "Character Image")
                {
                    images[j].sprite = Resources.Load<Sprite>($"CharacterSprite/{unit.ChName}_Battle");
                }

                else if (images[j].name == "Icon")
                {
                    images[j].sprite = Resources.Load<Sprite>($"AttackTypeIcon/{unit.AttackType}");
                }
            }

            // 슬롯 안에 있는 텍스트들 세팅
            TMP_Text[] tmps = slot.GetComponentsInChildren<TMP_Text>();

            for (int j = 0; j < tmps.Length; j++)
            {
                if (tmps[j].name == "Grade Text")
                {
                    tmps[j].text = unit.Grade.ToString();
                    tmps[j].color = unit.Grade switch
                    {
                        EGrade.S => Color.yellow,
                        EGrade.A => Color.red,
                        EGrade.B => Color.blue,
                        _ => Color.white
                    };
                }
                else if (tmps[j].name == "HP Text")
                {
                    tmps[j].text = unit.MaxHp.ToString();
                    _goToCurHpTmp[ch] = tmps[j];
                }
                else if (tmps[j].name == "Atk Text")
                {
                    tmps[j].text = unit.Atk.ToString();
                    _goToAtkTmp[ch] = tmps[j];
                }
                else if (tmps[j].name == "Def Text")
                {
                    tmps[j].text = unit.Def.ToString();
                    _goToDefTmp[ch] = tmps[j];
                }
            }
        }
    }

    // 스킬 버튼 안에 있는 아이콘, 마스크, 쿨타임 텍스트 세팅
    public void SetSKillUI(List<GameObject> chList, Dictionary<GameObject, Unit> goToUnit)
    {
        for (int i = 0; i < _skillButtons.Length; i++)
        {
            Image icon = _skillButtons[i].transform.GetChild(0).GetComponent<Image>();
            Image mask = _skillButtons[i].transform.GetChild(1).GetComponent<Image>();
            TMP_Text coolTimeTmp = _skillButtons[i].transform.GetChild(3).GetComponent<TMP_Text>(); //0403 계층구조 변경으로 수정.

            GameObject go = chList[i];

            goToUnit.TryGetValue(go, out Unit unit);

            if (unit != null)
            {
                _goToSkillMask[go] = mask;
                _goToSkillCoolTimeTmp[go] = coolTimeTmp;
            }
        }
    }

    public void IncreaseSKillGauge()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (_skillSlider.value < 1f)
        {
            if (Time.timeScale == 1f)
            {
                _skillSlider.value += _skillGaugeIncreaseRate;
            }
            else if (Time.timeScale == 2f)
            {
                _skillSlider.value += _skillGaugeIncreaseRate * 2f;
            }
        }
        else
        {
            _skillSlider.value = 1f;
        }
    }

    public bool TrySkill(int consume = 2)
    {
        bool isPossible = CheckSKillPossible(consume);

        if (isPossible)
        {
            _skillSlider.value -= _skillSlider.maxValue / 6f * consume;
            return true;
        }

        return false;
    }

    public bool CheckSKillPossible(int consume = 2)
    {
        float value = _skillSlider.value;
        float need = _skillSlider.maxValue / 6f * consume;
        return value >= need;
    }

    public void UpdateSkillCool(List<GameObject> chList, Dictionary<GameObject, Unit> goToUnit)
    {
        for (int i = 0; i < chList.Count; i++)
        {
            GameObject go = chList[i];

            if (goToUnit.TryGetValue(go, out Unit unit))
            {
                if (unit.IsDead)
                {
                    _skillButtons[i].interactable = false;
                    _goToSkillCoolTimeTmp[go].text = "Die";
                    _goToSkillMask[go].fillAmount = 1f;
                    continue;
                }

                float currentCool = unit.CurrentSkillCool;
                float maxCool = unit.MaxSkillCool;

                if (currentCool > 1000)
                {
                    _skillButtons[i].interactable = false;
                    _goToSkillCoolTimeTmp[go].text = "Used";
                }
                else if (currentCool > 0)
                {
                    _skillButtons[i].interactable = false;
                    _goToSkillCoolTimeTmp[go].text = Mathf.Ceil(currentCool).ToString();
                    _goToSkillMask[go].fillAmount = currentCool / maxCool;
                }
                else
                {
                    _skillButtons[i].interactable = true;

                    _goToSkillCoolTimeTmp[go].text = "";
                    _goToSkillMask[go].fillAmount = 0f;
                }
            }
        }
    }

    // 배틀 승리시에 결과창에서 보여줄 젬과 골드의 양
    public void SetStageReward(int gold, int gem)
    {
        if (_goldText != null)
            _goldText.text = gold.ToString("N0");

        if (_gemText != null)
            _gemText.text = gem.ToString("N0");
    }

    // 토스트 메시지 팝업 애니메이션 <- 0402 진주
    public void PopUpToastMessage(string msg)
    {
        if (_toastUI == null) return;

        _toastText.text = msg;
        _toastUI.SetActive(true);

        AnimateUI animate = _toastUI.GetComponent<AnimateUI>();

        if (animate != null)
        {
            animate.ResetAnimate();
            animate.PlayAnimate(0);
        }
    }

    public void UpdateCurrentHp(GameObject go, Unit unit)
    {
        _goToCurHpTmp[go].text = unit.CurHp.ToString();
    }

    // 플레이어/적 팀 전체 현재 HP를 계산하고
    // 상단 텍스트를 갱신한 뒤 totalHp를 반환하는 함수
    public int UpdateTotalHp(bool isPlayer, List<GameObject> goList, Dictionary<GameObject, Unit> unitDict)
    {
        int totalHp = 0;
        for (int i = 0; i < goList.Count; i++)
        {
            GameObject go = goList[i];
            Unit unit = unitDict[go];
            totalHp += unit.CurHp;
        }

        if (isPlayer)
        {
            _playerTotalHpTMP.text = totalHp.ToString();
        }

        else
        {
            _enemyTotalHpTMP.text = totalHp.ToString();
        }

        return totalHp;
    }

    // 전투 시작 시 한 번 호출해서
    // 플레이어/적 팀 전체 HP바의 최대값을 초기화하는 함수
    public void InitBattleTotalHpBar(List<GameObject> playerList, Dictionary<GameObject, Unit> playerUnitDict,
                                     List<GameObject> enemyList, Dictionary<GameObject, Unit> enemyUnitDict)
    {
        int playerTotalHp = UpdateTotalHp(true, playerList, playerUnitDict);
        int enemyTotalHp = UpdateTotalHp(false, enemyList, enemyUnitDict);

        if (_battleTotalHpBar != null)
        {
            _battleTotalHpBar.Init(playerTotalHp, enemyTotalHp);
        }
    }

    // 전투 중 누군가 데미지를 받거나 회복했을 때 호출해서
    // 플레이어/적 팀 전체 HP바를 현재 값 기준으로 갱신하는 함수
    public void RefreshBattleTotalHpBar(List<GameObject> playerList, Dictionary<GameObject, Unit> playerUnitDict,
                                        List<GameObject> enemyList, Dictionary<GameObject, Unit> enemyUnitDict)
    {
        int playerTotalHp = UpdateTotalHp(true, playerList, playerUnitDict);
        int enemyTotalHp = UpdateTotalHp(false, enemyList, enemyUnitDict);

        if (_battleTotalHpBar != null)
        {
            _battleTotalHpBar.UpdateBar(true, playerTotalHp);
            _battleTotalHpBar.UpdateBar(false, enemyTotalHp);
        }
    }

    private void ReturnField()
    {
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        SceneLoader.Instance.LoadScene(ESceneId.FieldScene);
    }

    public void ShowResultPanel(bool isVictory)
    {
        if (isVictory)
        {
            _victoryPanel.SetActive(true);
        }
        else
        {
            _defeatPanel.SetActive(true);
        }
    }

    public void ToggleSkillPanel(bool flag)
    {
        _skillPanel.SetActive(flag);
    }

    private void ShowStageNumber()
    {
        StartCoroutine(CoOneWordAnimation());
    }

    private IEnumerator CoOneWordAnimation()
    {
        BattleManager.Instance.BattleState = EBattleState.Ready;

        //_configButton.gameObject.SetActive(false);
        _statusButton.gameObject.SetActive(false);
        _worldSpaceStageNumberTMP.gameObject.SetActive(true);
        int length = _stageNumberText.Length;
        int n = 0;
        while (n < length)
        {
            _worldSpaceStageNumberTMP.text = _stageNumberText.Substring(0, n + 1);
            n++;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        //_configButton.gameObject.SetActive(true);
        _statusButton.gameObject.SetActive(true);
        _worldSpaceStageNumberTMP.gameObject.SetActive(false);

        _gamePanel.SetActive(true);

        BattleManager.Instance.StartBattle();
        AudioManager.Instance.PlayBattleBGMForCurrentStage();
    }
}