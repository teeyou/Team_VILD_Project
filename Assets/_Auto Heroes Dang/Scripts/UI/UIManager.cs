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
    }

    private void SetSlot()
    {
        int count = _slotParent.childCount;

        for (int i = 0; i < count; i++)
        {
            TMP_Text[] tmps = _slotParent.GetChild(i).GetComponentsInChildren<TMP_Text>();
            
            for (int j = 0; j < tmps.Length; j++)
            {
                Debug.Log($"{tmps[j].name} - {tmps[j].text}");
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
    }

    public void CloseStagePanel()
    {
        _fieldUI.ToggleStagePanel();
        ToggleStageButton(true);
    }
}
