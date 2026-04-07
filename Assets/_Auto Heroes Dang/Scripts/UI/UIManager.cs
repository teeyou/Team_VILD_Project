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
    [SerializeField] private StagePanelUI _stagePanelUI; // 스테이지 패널 연결

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

    [Header("상자 보상 아이콘 도착 위치")]
    [SerializeField] private RectTransform _goldTargetUI;
    [SerializeField] private RectTransform _gemTargetUI;

    public Vector3 GoldTargetUIPosition => _goldTargetUI.position;
    public Vector3 GemTargetUIPosition => _gemTargetUI.position;


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

            _slotParent.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                //Debug.Log($"idx : {idx}");

                _detailStatusPanel.SetActive(true);

            });
        }

        for (int i = 0; i < count; i++)
        {
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
                    tmps[j].text = $"Lv. {data.Level.ToString()}";
                }
            }
        }
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
        _stagePanelUI.RefreshUI();
    }

    public void CloseStagePanel()
    {
        _fieldUI.ToggleStagePanel();
        ToggleStageButton(true);
    }
}
