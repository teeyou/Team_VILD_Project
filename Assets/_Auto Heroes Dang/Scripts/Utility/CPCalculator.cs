using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CPCalculator
{
    public static int CalculateCP(int atk, int def, int hp)
    {
        float a = atk * 2f;
        float d = def * 1.5f;
        float h = hp * 0.1f;

        // 최종 전투력
        float power = a + d + h;
        return Mathf.RoundToInt(power);
    }
}
