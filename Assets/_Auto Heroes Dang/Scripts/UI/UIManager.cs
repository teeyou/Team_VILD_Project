using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _autoProgressButton;
    [SerializeField] private Button _startStageButton;
    private void Awake()
    {
        _autoProgressButton.onClick.AddListener(MovePlayer);
        _startStageButton.onClick.AddListener(() => LoadBattleScene(GameManager.Instance.CurrentStage));
        
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
        FieldManager.Instance.MoveNextPoint();
    }

    private void ToggleProgressButton(bool flag)
    {
        _autoProgressButton.gameObject.SetActive(flag);
    }

    private void LoadBattleScene(EGameStage stage)
    {
        // 스테이지 진입하기 전에 현재 위치 정보 저장

        // 메인 캐릭터 저장
        DataSource.Instance.MainCharacterPosRot = new PosRotData(
            FieldManager.Instance.MainCharacterTr.position, 
            FieldManager.Instance.MainCharacterTr.rotation);

        // 서브 캐릭터 저장
        IReadOnlyList<PosRotData> list = DataSource.Instance.GetSubPosRotList();    // 필드 씬 위치정보 리스트

        IReadOnlyList<Transform> trList = FieldManager.Instance.GetSubCharacterTrList;  //필드 씬 트랜스폼 
        int len = trList.Count;

        // 위치정보 없으면 Add
        if (list.Count == 0)
        {
            for (int i = 0; i < len; i++)
            {
                DataSource.Instance.AddSubPosRot(trList[i].position, trList[i].rotation);
            }
        }

        // 위치정보 있으면 덮어쓰기
        else
        {
            for (int i = 0; i < len; i++)
            {
                DataSource.Instance.SetSubPosRot(i, trList[i].position, trList[i].rotation);
            }
        }
        
        CinemachineDollyCart cart = FindObjectOfType<CinemachineDollyCart>();

        if (cart == null)
        {
            Debug.Log("UIManager - cart NULL");
        }

        DataSource.Instance.CartPosition = cart.m_Position;

        SceneLoader.Instance.LoadScene(stage);
    }
}
