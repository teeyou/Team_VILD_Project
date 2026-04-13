using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageTotalCP
{
    public static int GetTotalCP(EGameStage stage)
    {
        switch (stage)
        {
            case EGameStage.Stage1_1:
                return 100;
            case EGameStage.Stage1_2:
                return 100;
            case EGameStage.Stage1_3:
                return 100;
            
            case EGameStage.Stage2_1:
                return 100;
            case EGameStage.Stage2_2:
                return 100;
            case EGameStage.Stage2_3:
                return 100;
            
            case EGameStage.Stage3_1:
                return 100;
            case EGameStage.Stage3_2:
                return 100;
            case EGameStage.Stage3_3:
                return 100;
            
            default:
                return 0;
        }
    }
}
