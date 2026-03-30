using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : Singleton<BattleUIManager>
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _readyPanel;

    void Start()
    {
        _backButton.onClick.AddListener(ReturnField);
        _startButton.onClick.AddListener(StartBattle);
    }

    private void ReturnField()
    {
        SceneLoader.Instance.LoadScene(ESceneId.FieldScene);
    }

    private void StartBattle()
    {
        _readyPanel.SetActive(false);
        BattleManager.Instance.StartBattle();
    }
}
