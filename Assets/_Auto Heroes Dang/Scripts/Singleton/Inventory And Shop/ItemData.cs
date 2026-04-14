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

    public static string GetBaseDescription(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return "흔한 싸구려 장비.\n없는 것 보다는 낫다.";
            case Grade.Uncommon:
                return "가성비 좋은 장비.\n많은 모험가들이 애용한다.";
            case Grade.Rare:
                return "아껴쓰고 싶은 귀한 장비.\n이거 진짜 좋은 거에요.";
            case Grade.Elite:
                return "이정도면 남들에게 자랑해도 될 것 같다.\n아무나 가질 수 없는 장비";
            case Grade.Epic:
                return "이걸 봤다고?\n당신은 운이 좋으신 겁니다.";
            default:
                return "";
        }
    }

    public static int GetPrice(Grade grade)
    {
        switch (grade)
        {
            case Grade.Common:
                return 500;
            case Grade.Uncommon:
                return 2000;
            case Grade.Rare:
                return 10000;
            case Grade.Elite:
                return 30000;
            case Grade.Epic:
                return 77777;
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
        this.uniqueId = uniqueId < 0 ? GenerateLongID() : uniqueId;

        this.name = name;
        this.type = type;
        this.grade = grade;
        this.level = level;
        this.value = value;
        this.price = price;
        this.description = description;
        this.state = state;
    }

    public static int GenerateLongID()
    {
        byte[] i = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt32(i, 0);
    }


}