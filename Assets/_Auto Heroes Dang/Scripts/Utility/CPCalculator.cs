using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CPCalculator
{
    // 기준 공격력 방어력
    private const int BASE_ATK = 100;
    private const int BASE_DEF = 100;

    public static int CalculateCP(int atk, int def, int hp)
    {
        bool isCritical;
        float damage = DamageCalculator.CalculateDamage(atk, BASE_DEF, out isCritical);

        float survivability = hp / (1 + BASE_ATK);

        float defenseBonus = def * 0.5f;
        float hpBonus = Mathf.Sqrt(hp);

        // 최종 전투력
        float power = (damage * survivability) + defenseBonus + hpBonus;
        return Mathf.RoundToInt(power);
    }
}
