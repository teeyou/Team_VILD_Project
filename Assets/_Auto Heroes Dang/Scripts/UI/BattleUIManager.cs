using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : Singleton<BattleUIManager>
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _readyPanel;

    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private GameObject _victoryPanel;

    [SerializeField] private Button _defeatButton;
    [SerializeField] private Button _victoryButton;

    void Start()
    {
        _backButton.onClick.AddListener(ReturnField);

        _startButton.onClick.AddListener(() =>
        {
            _readyPanel.SetActive(false);
            BattleManager.Instance.StartBattle();
        });

        _defeatButton.onClick.AddListener(ReturnField);

        _victoryButton.onClick.AddListener(() =>
            {
                GameManager.Instance.IncreaseCurrentStage();
                ReturnField();
            });
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
