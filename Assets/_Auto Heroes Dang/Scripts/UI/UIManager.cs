using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _autoProgressButton;

    private void Awake()
    {
        _autoProgressButton.onClick.AddListener(MovePlayer);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void MovePlayer()
    {
        ToggleProgressButton(false);
        GameManager.Instance.MoveNextPoint();
    }

    private void ToggleProgressButton(bool flag)
    {
        _autoProgressButton.gameObject.SetActive(flag);
    }
}
