using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/LevelUp Required Gold", fileName = "LevelUpRequiredGoldTable")]
public class LevelUpRequiredGold_SO : ScriptableObject
{
    [SerializeField] private int _sLevelUpRequiredGold;
    [SerializeField] private int _aLevelUpRequiredGold;
    [SerializeField] private int _bLevelUpRequiredGold;

    public int SLevelUpRequiredGold { get { return _sLevelUpRequiredGold; } }
    public int ALevelUpRequiredGold { get { return _aLevelUpRequiredGold; } }
    public int BLevelUpRequiredGold { get { return _bLevelUpRequiredGold; } }

}
