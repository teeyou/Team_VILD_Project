using UnityEngine;

public static class DamageCalculator
{
    public static int CalculateDamage(int yourAtk, int targetDef)
    {
        // 공격력 * (공격력 / (공격력 + 방어력))
        int totalDamage = yourAtk * (yourAtk / (yourAtk + targetDef));

        if (CalculateCriticalProb())
        {
            return (int)(totalDamage * 1.5f);   // 치명타 1.5배
        }

        return totalDamage;
    }

    private static bool CalculateCriticalProb()
    {
        int num = Random.Range(0, 10);

        if (num <= 1)   // 치명타 확률 20%
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
