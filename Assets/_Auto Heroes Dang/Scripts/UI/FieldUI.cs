using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldUI : MonoBehaviour
{
    [SerializeField] private GameObject _stagePanel;    // 전체 스테이지 패널 ( Enemy Panel / Boss Panel / 스타트 버튼)
    [SerializeField] private GameObject _enemy;         // Enemy Panel
    [SerializeField] private GameObject _boss;          // Boss Panel

    private bool _isBoss = false;    // 현재 스테이지가 보스인지에 대한 여부

    private void Awake()
    {
        if (_stagePanel.activeSelf)     // 기본적으로 스테이지 패널은 OFF 상태입니다.
        { 
            _stagePanel.SetActive(false); 
        }
    }

    // 스테이지 정보를 적을 내용을 만드는 곳.
    private void SetStagePanel() 
    {
        // 일단 보스인지를 먼저 알아낸다는 내용
        // _isBoss = ? true : false;

        // 이것저것 정보를 받아와서 세팅한다는 내용

    }

    // 스테이지 정보 팝업. 보스인지 여부에 따라 다른 패널로 출력 (디자인을 바꿔야 해서...)
    public void PopUpFieldInfo()
    {
        ToggleStagePanel(true);
        //_stagePanel.SetActive(true);  // <- 트랜지션 애니메이션 필요 시 이부분 확장
        SetStagePanel(); // <----------------------------------------만들어야 해ㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐㅐ

        if (_isBoss)
        {
            _enemy.SetActive(false);
            _boss.SetActive(true);
        }
        else
        {
            _enemy.SetActive(true);
            _boss.SetActive(false);
        }
    }

    public void ToggleStagePanel(bool flag)
    {
        _stagePanel.SetActive(true);
    }

}
