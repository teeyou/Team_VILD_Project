using GAP_LaserSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleUIManager : Singleton<BattleUIManager>
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _speedButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _startButton;

    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private GameObject _victoryPanel;

    [SerializeField] private Button _defeatButton;
    [SerializeField] private Button _victoryButton;

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

    private Dictionary<GameObject, Image> _goToSkillIcon = new Dictionary<GameObject, Image>();
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
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 0f;
            }
        });

        _speedButton.onClick.AddListener(() =>
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 2f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        });

        _backButton.onClick.AddListener(ReturnField);

        _startButton.onClick.AddListener(() =>
        {
            _startButton.gameObject.SetActive(false);
            _gamePanel.SetActive(true);
            BattleManager.Instance.StartBattle();
        });

        _defeatButton.onClick.AddListener(ReturnField);

        _victoryButton.onClick.AddListener(() =>
        {
            GameManager.Instance.IncreaseCurrentStage();
            ReturnField();
        });
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
            TMP_Text coolTimeTmp = _skillButtons[i].transform.GetChild(2).GetComponent<TMP_Text>();

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
            _skillSlider.value += _skillGaugeIncreaseRate;
        }
        else
        {
            _skillSlider.value = 1f;
        }
    }

    public bool TrySkill(int consume)
    {
        bool isPossible = CheckSKillPossible(consume);

        if (isPossible)
        {
            _skillSlider.value -= _skillSlider.maxValue / 6f * consume;
            return true;
        }

        return false;
    }

    public bool CheckSKillPossible(int consume)
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
                    if (CheckSKillPossible(2))
                    {
                        _skillButtons[i].interactable = true;
                    }
                    else
                    {
                        _skillButtons[i].interactable = false;
                    }

                    _goToSkillCoolTimeTmp[go].text = "";
                    _goToSkillMask[go].fillAmount = 0f;
                }
            }
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
}