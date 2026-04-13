using UnityEngine;

public static class ItemForgeHelper
{
    public static bool IsEquip(ItemType type)
    {
        return type == ItemType.Sword ||
               type == ItemType.Hat ||
               type == ItemType.Armor ||
               type == ItemType.Shoes ||
               type == ItemType.Ring;
    }

    public static int GetEnhanceSuccessPercent(int currentLevel)
    {
        return Mathf.Max(10, 90 - currentLevel * 10);
    }

    public static int GetEnhanceCost(ItemData item)
    {
        int baseCost = 0;             // 0강 장비 강화 시도 비용
        int increasePerLevel = 200;   // 강화 레벨당 증가 비용

        return baseCost + (item.level * increasePerLevel);
    }

    public static int GetFusionGemCost(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common: return 10;
            case Grade.Uncommon: return 20;
            case Grade.Rare: return 30;
            case Grade.Elite: return 40;
            case Grade.Epic: return 50;
            default: return 10;
        }
    }

    public static int GetFusionSuccessPercent(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common: return 50;
            case Grade.Uncommon: return 40;
            case Grade.Rare: return 30;
            case Grade.Elite: return 20;
            case Grade.Epic: return 0;
            default: return 0;
        }
    }

    public static bool CanFuse(ItemData left, ItemData right)
    {
        return left.type == right.type &&
               left.grade == right.grade &&
               IsEquip(left.type) &&
               left.grade != Grade.Epic;
    }

    public static Grade GetNextGrade(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common: return Grade.Uncommon;
            case Grade.Uncommon: return Grade.Rare;
            case Grade.Rare: return Grade.Elite;
            case Grade.Elite: return Grade.Epic;
            default: return grade;
        }
    }
}