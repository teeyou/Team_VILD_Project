using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToStart : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _newButton;
    [SerializeField] private Button _continueButton;

    private void Awake()
    {
        _panel.SetActive(false);
    }

    private void Start()
    {
        _newButton.onClick.AddListener(() =>
        {
            DataSource.Instance.Gem = 10;
            DataSource.Instance.Gold = 1000;

            SceneLoader.Instance.LoadScene(ESceneId.SelectScene);
        });

        _continueButton.onClick.AddListener(() =>
        {
            DataSource.Instance.SetSaveData();

            SceneLoader.Instance.LoadScene(ESceneId.FieldScene);
        });
    }

    public void LoadScene()
    {
        if (GameManager.Instance.IsSave)
        {
            _panel.SetActive(true);
        }

        else
        {
            SceneLoader.Instance.LoadScene(ESceneId.SelectScene);
        }
    }
}
