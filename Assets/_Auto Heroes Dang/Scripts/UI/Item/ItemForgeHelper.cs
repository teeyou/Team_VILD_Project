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
        return item.price;
    }

    public static int GetFusionSuccessPercent(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common: return 90;
            case Grade.Uncommon: return 80;
            case Grade.Rare: return 70;
            case Grade.Elite: return 60;
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