using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGameStage
{
    Stage1_1 = 0,
    Stage1_2 = 1,
    Stage1_3 = 2,
    Stage2_1 = 3,
    Stage2_2 = 4,
    Stage2_3 = 5,
    Stage3_1 = 6,
    Stage3_2 = 7,
    Stage3_3 = 8,
}

public class GameManager : Singleton<GameManager>
{
    private EGameStage _currentStage = EGameStage.Stage1_3;

    public EGameStage CurrentStage { get { return _currentStage; } }

    public bool IsFirstPoint { get; set; } = true;
    
    public bool IsStageStart { get; set; } = false;

    public bool IsStageClear { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    

    }

    private void Update()
    {
        
    }

    public void IncreaseCurrentStage()
    {
        EGameStage nextStage = (EGameStage)((int)_currentStage + 1);

        // 다음 스테이지가 있으면 증가시킴
        if (Enum.IsDefined(typeof(EGameStage), nextStage))
        {
            _currentStage = nextStage;
            return;
        }

        // 다음 스테이지가 없으면 아무것도 안함
    }

    public string GetCurrentStageDisplayName()
    {
        string stageName = _currentStage.ToString();   // Stage1_1
        stageName = stageName.Replace("Stage", "Stage ");
        stageName = stageName.Replace("_", "-");
        return stageName;                              // Stage 1-1
    }

    public int GetCurrentStageGoldReward()
    {
        return ((int)_currentStage + 1) * 1000;
    }

    public int GetCurrentStageGemReward()
    {
        return 10 + ((int)_currentStage * 5);
    }
}
