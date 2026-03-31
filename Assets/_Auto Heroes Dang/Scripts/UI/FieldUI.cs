using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldUI : MonoBehaviour
{
    [SerializeField] private GameObject _stagePanel;    // 전체 스테이지 패널 ( Enemy Panel / Boss Panel / 스타트 버튼)
    [SerializeField] private GameObject _enemy;         // Enemy Panel
    [SerializeField] private GameObject _boss;          // Boss Panel

    [SerializeField] private AnimateUI _uiAnimate;

    private AnimateUI _enemyAnimate;
    private AnimateUI _bossAnimate;

    private bool _isBoss = false;    // 현재 스테이지가 보스인지에 대한 여부

    private void Awake()
    {
        if (!_stagePanel.activeSelf)     // <----- stagePanel 상시 On 상태로 변경
        { 
            _stagePanel.SetActive(true); 
        }


        if (_enemy != null) _enemyAnimate = _enemy.GetComponent<AnimateUI>();
        if (_boss != null) _bossAnimate = _boss.GetComponent<AnimateUI>();
    }


    // 스테이지 정보를 적을 내용을 만드는 곳. -----------> 완성 후 PopUpFieldInfo() 주석 해제할 것
    private void SetStagePanel() 
    {
        // 일단 보스인지를 먼저 알아낸다는 내용
        // _isBoss = ? true : false;

        // 이것저것 정보를 받아와서 세팅한다는 내용

    }

    // 스테이지 정보 팝업. 보스인지 여부에 따라 다른 패널로 출력
    public void PopUpFieldInfo()
    {
        /*
         
        if(현재 스테이지!= 다음 스테이지)
        {
            SetStagePanel();
        }
         
         */

        if (_isBoss)
        {
            _enemy.SetActive(false);
            _boss.SetActive(true);
            _bossAnimate.PlayAnimate(0);
        }
        else
        {
            _enemy.SetActive(true);
            _boss.SetActive(false);
            _enemyAnimate.PlayAnimate(0);
        }

    }

    public void ToggleStagePanel()
    {
        if (_isBoss)
        {
            if (_bossAnimate != null) _bossAnimate.PlayAnimate(1);
        }
        else
        {
            if (_enemyAnimate != null) _enemyAnimate.PlayAnimate(1);
        }


    }

}
