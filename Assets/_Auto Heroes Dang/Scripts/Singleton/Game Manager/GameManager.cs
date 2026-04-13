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
    private EGameStage _currentStage = EGameStage.Stage1_1;

    public EGameStage CurrentStage { get { return _currentStage; } set { _currentStage = value; } }

    public bool IsFirstPoint { get; set; } = true;
    
    public bool IsStageStart { get; set; } = false;

    public bool IsStageClear { get; set; } = false;
    public bool IsSave { get; set; } = false;


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
        if ((int)_currentStage % 3 == 2)
        {
            return ((int)_currentStage + 1) * 1000 * 5; // 보스 스테이지 클리어는 5배
        }

        return ((int)_currentStage + 1) * 1000;
    }

    public int GetCurrentStageGemReward()
    {
        return 10 + ((int)_currentStage * 5);
    }

    public ItemData GetCurrentStageItemReward(EGameStage stage)
    {
        return GetItemData(stage);
    }

    private ItemData GetItemData(EGameStage stage)
    {
        // 커먼 : 1 ~ 60
        // 언커먼 : 61 ~ 90
        // 레어 : 91 ~ 100

        int number = 0;

        if ((int)stage % 3 == 2)
            number = UnityEngine.Random.Range(61, 101); //보스 스테이지는 언커먼 이상만 받음
        
        else
            number = UnityEngine.Random.Range(1, 101);  // 1 ~ 100까지 랜덤

        int type = UnityEngine.Random.Range(0, 5); //모자, 검, 갑주, 반지, 신발

        if (number <= 60)
        {
            Grade grade = Grade.Common;
            int value = ItemDescription.GetBaseValue(grade);
            string dis = ItemDescription.GetBaseDescription(grade); 


            switch (type)
            {
                case 0:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 모자", ItemType.Hat, grade, 1, value, 100, dis);

                case 1:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 검", ItemType.Sword, grade, 1, value, 100, dis);

                case 2:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 갑주", ItemType.Armor, grade, 1, value, 100, dis);

                case 3:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 반지", ItemType.Ring, grade, 1, value, 100, dis);

                case 4:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 신발", ItemType.Shoes, grade, 1, value, 100, dis);

            }
        }

        else if (number <= 90)
        {
            Grade grade = Grade.Uncommon;
            int value = ItemDescription.GetBaseValue(grade);
            string dis = ItemDescription.GetBaseDescription(grade);

            switch (type)
            {
                case 0:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 모자", ItemType.Hat, grade, 1, value, 100, dis);

                case 1:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 검", ItemType.Sword, grade, 1, value, 100, dis);

                case 2:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 갑주", ItemType.Armor, grade, 1, value, 100, dis);

                case 3:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 반지", ItemType.Ring, grade, 1, value, 100, dis);

                case 4:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 신발", ItemType.Shoes, grade, 1, value, 100, dis);

            }
        }

        else
        {
            Grade grade = Grade.Rare;
            int value = ItemDescription.GetBaseValue(grade);
            string dis = ItemDescription.GetBaseDescription(grade);

            switch (type)
            {
                case 0:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 모자", ItemType.Hat, grade, 1, value, 100, dis);

                case 1:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 검", ItemType.Sword, grade, 1, value, 100, dis);

                case 2:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 갑주", ItemType.Armor, grade, 1, value, 100, dis);

                case 3:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 반지", ItemType.Ring, grade, 1, value, 100, dis);

                case 4:
                    return new ItemData($"{ItemName.GetPrefix(grade)} 신발", ItemType.Shoes, grade, 1, value, 100, dis);

            }
        }

        return new ItemData();
    }

}
