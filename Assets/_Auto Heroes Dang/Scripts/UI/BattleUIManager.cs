using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("캐릭터 정보 UI 세팅")]
    [SerializeField] private Transform _chSlotParent;   //Character Group
    [SerializeField] private GameObject _chSlotPrefab;

    private Dictionary<GameObject, TMP_Text> _goToCurHpTmp = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<GameObject, TMP_Text> _goToAtkTmp = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<GameObject, TMP_Text> _goToDefTmp = new Dictionary<GameObject, TMP_Text>();

    [Header("상단 중앙 체력 난이도 UI")]
    [SerializeField] private TMP_Text _playerTotalHpTMP;
    [SerializeField] private TMP_Text _enemyTotalHpTMP;
    [SerializeField] private Image _difficulty;
    void Start()
    {
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
            //_readyPanel.SetActive(false);
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
    public void CreateChSlot(List<GameObject> chList, Dictionary<GameObject,Unit> goToUnit)
    {
        for (int i = 0; i < chList.Count; i++)
        {
            GameObject slot = Instantiate(_chSlotPrefab);
            slot.transform.SetParent(_chSlotParent);
            slot.transform.localScale = Vector3.one;
            slot.transform.localPosition = new Vector3(slot.transform.localPosition.x, slot.transform.localPosition.y, 0f);
            slot.transform.localRotation = Quaternion.identity;


            //Unit unit = chList[i].GetComponent<Unit>();
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

    public void UpdateCurrentHp(GameObject go, Unit unit)
    {
        _goToCurHpTmp[go].text = unit.CurHp.ToString();
    }

    public int UpdateTotalHp(bool isPlayer, List<GameObject> goList, Dictionary<GameObject,Unit> unitDict)
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

    private void ReturnField()
    {
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
