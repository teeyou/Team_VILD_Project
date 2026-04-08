using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldUI : MonoBehaviour
{
    [SerializeField] private GameObject _stagePanel;    // 전체 스테이지 패널 ( Enemy Panel / Boss Panel / 스타트 버튼)
    [SerializeField] private GameObject _enemy;         // Enemy Panel
    [SerializeField] private GameObject _boss;          // Boss Panel

    [SerializeField] private AnimateUI _uiAnimate;

    [SerializeField] private StagePanelUI _stagePanelUI; // 스테이지 패널 연결

    [SerializeField] private GameObject _enemySlotPrefab;
    [SerializeField] private Transform _enemySlotParent;

    private AnimateUI _enemyAnimate;
    private AnimateUI _bossAnimate;

    private bool _isBoss = false;    // 현재 스테이지가 보스인지에 대한 여부

    private bool _isSetstageInfo = false;

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
        if (!_isSetstageInfo)
        {
            _stagePanelUI.RefreshUI();

            _isSetstageInfo = true;

            SetStageInfo(GameManager.Instance.CurrentStage);
        }
    }

    // 스테이지 정보 팝업. 보스인지 여부에 따라 다른 패널로 출력
    public void PopUpFieldInfo()
    {
        int currentStage = (int)GameManager.Instance.CurrentStage;

        _isBoss = (currentStage % 3 == 2); // 스테이지 번호가 2, 5, 8인 경우 보스 스테이지

        SetStagePanel();

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

    public void SetStageInfo(EGameStage stage)
    {
        if (stage == EGameStage.Stage1_1)
        {
            SetStage1_1();
        }

        else if (stage == EGameStage.Stage1_2)
        {
            SetStage1_2();
        }
        else if (stage == EGameStage.Stage1_3)
        {
            SetStage1_3();
        }
        else if (stage == EGameStage.Stage2_1)
        {
        }
        else if (stage == EGameStage.Stage2_2)
        {
        }
        else if (stage == EGameStage.Stage2_3)
        {
        }
        else if (stage == EGameStage.Stage3_1)
        {
        }
        else if (stage == EGameStage.Stage3_2)
        {
        }
        else if (stage == EGameStage.Stage3_3)
        {
        }


            /*
            1-1 버섯3 선인장2 이상한놈1
            1-2 버섯3 선인장2 이상한놈2
            1-3 버섯3 선인장2 이상한놈2 마법사1

            2-1 쥐3 웨어울프2 샐러맨더1
            2-2 쥐3 웨어울프2 샐러맨더2
            2-3 쥐3 웨어울프2 샐러맨더2 오크1

            3-1 웨어울프3 오크1 이상한놈1 마법사1
            3-2 웨어울프3 오크1 이상한놈1 마법사1 비숍1
            3-3 웨어울프3 오크1 이상한놈1 마법사1 비숍1 디몬킹보스1

            */

            // EnemySprites/
            // Werewolf_Normal

            // StingRay_Normal
            // Salamander_Normal
            // Rat_Normal
            // Orc_Normal

            // Mushroom_Normal
            // EvilMage_Normal

            // Cactus_Normal
            // BishopKnight_Normal

            // Werewolf_Boss
            // Orc_Boss
            // DemonKing_Boss
        }

    private void SetStage1_1()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(_enemySlotPrefab, _enemySlotParent);

            Image[] images = go.GetComponentsInChildren<Image>();
            TMP_Text[] tmps = go.GetComponentsInChildren<TMP_Text>();

            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].name == "Enemy Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/Mushroom_Normal");
                    }

                    else if (i == 1)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/Cactus_Normal");
                    }

                    else
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/StingRay_Normal");
                    }
                }

                else if (images[j].name == "Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Melee");
                    }

                    else if (i == 1)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Melee");
                    }

                    else
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Range");
                    }
                }
            }

            for (int j = 0; j < tmps.Length; j++)
            {
                if (tmps[j].name == "Level Text")
                {
                    if (i == 0)
                    {
                        tmps[j].text = "3";
                    }
                    else if (i == 1)
                    {
                        tmps[j].text = "2";
                    }
                    else
                    {
                        tmps[j].text = "1";
                    }
                }
            }
        }
    }

    private void SetStage1_2()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = Instantiate(_enemySlotPrefab, _enemySlotParent);

            Image[] images = go.GetComponentsInChildren<Image>();
            TMP_Text[] tmps = go.GetComponentsInChildren<TMP_Text>();

            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].name == "Enemy Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/Mushroom_Normal");
                    }

                    else if (i == 1)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/Cactus_Normal");
                    }

                    else
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/StingRay_Normal");
                    }
                }

                else if (images[j].name == "Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Melee");
                    }

                    else if (i == 1)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Melee");
                    }

                    else
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Range");
                    }
                }
            }

            for (int j = 0; j < tmps.Length; j++)
            {
                if (tmps[j].name == "Level Text")
                {
                    if (i == 0)
                    {
                        tmps[j].text = "3";
                    }
                    else if (i == 1)
                    {
                        tmps[j].text = "2";
                    }
                    else
                    {
                        tmps[j].text = "2";
                    }
                }
            }
        }
    }

    private void SetStage1_3()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject go = Instantiate(_enemySlotPrefab, _enemySlotParent);

            Image[] images = go.GetComponentsInChildren<Image>();
            TMP_Text[] tmps = go.GetComponentsInChildren<TMP_Text>();

            for (int j = 0; j < images.Length; j++)
            {
                if (images[j].name == "Enemy Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/Mushroom_Normal");
                    }

                    else if (i == 1)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/Cactus_Normal");
                    }

                    else if (i == 2)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/StingRay_Normal");
                    }

                    else if (i == 3)
                    {
                        images[j].sprite = Resources.Load<Sprite>("EnemySprites/EvilMage_Normal");
                    }
                }

                else if (images[j].name == "Icon")
                {
                    if (i == 0)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Melee");
                    }

                    else if (i == 1)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Melee");
                    }

                    else if (i == 2)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Range");
                    }

                    else if (i == 3)
                    {
                        images[j].sprite = Resources.Load<Sprite>("AttackTypeIcon/Range");
                    }
                }
            }

            for (int j = 0; j < tmps.Length; j++)
            {
                if (tmps[j].name == "Level Text")
                {
                    if (i == 0)
                    {
                        tmps[j].text = "3";
                    }
                    else if (i == 1)
                    {
                        tmps[j].text = "2";
                    }
                    else if (i == 2)
                    {
                        tmps[j].text = "2";
                    }

                    else if (i == 3)
                    {
                        tmps[j].text = "BOSS";
                    }
                }
            }
        }
    }
}
