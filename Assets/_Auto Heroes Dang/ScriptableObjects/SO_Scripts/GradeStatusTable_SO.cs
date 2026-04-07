using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/Grade Status Table", fileName = "GradeStatusTable")]
public class GradeStatusTable_SO : ScriptableObject
{
    [SerializeField] LevelUpRequiredGold_SO _levelUpRequiredGold;

    [Header("Level UP Bonus")]
    [SerializeField] private int _sLevelUpBonus;
    [SerializeField] private int _aLevelUpBonus;
    [SerializeField] private int _bLevelUpBonus;
    public int SLevelUpBonus { get { return _sLevelUpBonus; } }
    public int ALevelUpBonus { get { return _aLevelUpBonus; } }
    public int BLevelUpBonus { get { return _bLevelUpBonus; } }

    public int GetLevelUpRequiredGold(int level, EGrade grade)
    {
        switch (grade)
        {
            case EGrade.S:
                return level * _levelUpRequiredGold.SLevelUpRequiredGold;
            case EGrade.A:
                return level * _levelUpRequiredGold.ALevelUpRequiredGold;
            case EGrade.B:
                return level * _levelUpRequiredGold.BLevelUpRequiredGold;
            default:
                return 0;
        }
    }

}
