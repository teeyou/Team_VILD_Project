using System;

static class ItemName
{
    public static string GetPrefix(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return "흔한";
            case Grade.Uncommon:
                return "쓸만한";
            case Grade.Rare:
                return "귀한";
            case Grade.Elite:
                return "영웅의";
            case Grade.Epic:
                return "전설의";
            default:
                return "NULL";
        }
    }
}

static class ItemDescription
{
    public static int GetBaseValue(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return 10;
            case Grade.Uncommon:
                return 60;
            case Grade.Rare:
                return 110;
            case Grade.Elite:
                return 160;
            case Grade.Epic:
                return 210;
            default:
                return 0;
        }
    }
}

[Serializable]
public enum ItemType
{
    Sword,
    Hat,
    Armor,
    Shoes,
    Ring,

    AtkBuff,
    DefBuff
}

[Serializable]
public enum Grade
{
    Common,
    Uncommon,
    Rare,
    Elite,
    Epic,

    AtkBuff,
    DefBuff
}

public enum ItemState // 4월 7일 추가내용
{
    Available,
    SoldOut
}

[Serializable]
public struct ItemData
{
    public int uniqueId; // 같은 값의 장비를 가지고 있을때 동일한 장비로 취급 되는걸 막기 위한 ID
    public string name;
    public ItemType type;
    public Grade grade;
    public int level;
    public int value;       // 공격력 / 방어력 값
    public int price;
    public string description;

    public ItemState state;

    public string FullDescription // 0413 추가
    {
        get
        {
            bool isAtk = type == ItemType.Sword || type == ItemType.Ring || type == ItemType.AtkBuff;
            string stat = isAtk ? "Atk" : "Def";

            return $"{description}\n{stat} +{value}";
        }
    }

    public ItemData(
        string name,
        ItemType type,
        Grade grade,
        int level,
        int value,
        int price,
        string description,
        ItemState state = ItemState.Available,
        int uniqueId = -1)
    {
        this.uniqueId = uniqueId < 0 ? ItemIdGenerator.GetNextId() : uniqueId;

        this.name = name;
        this.type = type;
        this.grade = grade;
        this.level = level;
        this.value = value;
        this.price = price;
        this.description = description;
        this.state = state;
    }
}