using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    void Start()
    {
        _backButton.onClick.AddListener(ReturnField);
    }

    void Update()
    {
        
    }

    private void ReturnField()
    {
        SceneLoader.Instance.LoadScene(ESceneId.FieldScene);
    }
}
