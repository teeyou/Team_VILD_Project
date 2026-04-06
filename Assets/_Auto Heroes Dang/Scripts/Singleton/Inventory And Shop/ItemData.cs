using System;

[Serializable]
public enum ItemType
{
    Sword,
    Hat,
    Armor,
    Shoes,
    Ring,
}

[Serializable]
public enum Grade
{
    Common,
    Uncommon,
    Rare,
    Elite,
    Epic,
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

    public ItemData(string name, ItemType type, Grade grade, int level,int price, string description)
    {
        this.name = name;
        this.type = type;
        this.grade = grade;
        this.level = level;
        this.price = price;

        this.description = description;
    }
}