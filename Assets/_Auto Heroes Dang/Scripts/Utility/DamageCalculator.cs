using UnityEngine;

public static class DamageCalculator
{
    public static int CalculateDamage(int yourAtk, int targetDef)
    {
        float atk = yourAtk;
        float def = targetDef;

        // 공격력 * (공격력 / (공격력 + 방어력))
        float totalDamage = atk * (atk / (atk + def));

        if (CalculateCriticalProb())
        {
            totalDamage *= 1.5f;
        }

        return Mathf.Max(1, Mathf.RoundToInt(totalDamage));
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