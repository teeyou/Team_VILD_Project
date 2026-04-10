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
    public int uniqueId; // 같은 값의 장비를 가지고 있을때 동일한 장비로 취급 되는걸 막기 위한 ID
    public string name;
    public ItemType type;
    public Grade grade;
    public int level;
    public int value;       // 공격력 / 방어력 값
    public int price;
    public string description;

    public ItemState state;

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