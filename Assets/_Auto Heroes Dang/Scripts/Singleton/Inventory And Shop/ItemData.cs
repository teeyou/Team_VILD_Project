using System;

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
    public string name;
    public ItemType type;
    public Grade grade;
    public int level;
    public int price;
    public string description;

    public ItemState state;

    public ItemData(string name, ItemType type, Grade grade, int level,int price, string description, ItemState state = 0)
    {
        this.name = name;
        this.type = type;
        this.grade = grade;
        this.level = level;
        this.price = price;

        this.description = description;

        this.state = state;
    }
}