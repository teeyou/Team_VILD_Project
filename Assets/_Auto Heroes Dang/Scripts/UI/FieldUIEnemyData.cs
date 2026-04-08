using System.Collections.Generic;
using UnityEngine;

public enum FieldUIEnemyName
{
    BishopKnight_Normal,
    Werewolf_Normal,
    StingRay_Normal,
    Salamander_Normal,
    Rat_Normal,
    Mushroom_Normal,
    Cactus_Normal,
    EvilMage_Normal,
    Orc_Normal,

    EvilMage_Boss,
    Orc_Boss,
    DemonKing_Boss,

}

public enum AttackType
{
    Melee,
    Range,
    Tank
}


[System.Serializable]
public class FieldUIEnemyData
{
    public FieldUIEnemyName enemyName;
    public AttackType attackType;
    public Sprite enemySprite;
}

[System.Serializable]
public class FieldUIStageData
{
    public EGameStage stage;
    public List<FieldUIEnemyAmount> enemy;
}

[System.Serializable]
public class FieldUIEnemyAmount
{
    public FieldUIEnemyName enemyName;
    public string amount;
}