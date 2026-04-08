using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackDescription
{
    public static string GetAttackDescription(int idx)
    {
        if (idx == 0)
        {
            return "가로로 베어낸다.";
        }

        else if (idx == 1)
        {
            return "가로로 베어낸다.";
        }

        else if (idx == 2)
        {
            return "가로로 베어낸다.";
        }

        else if (idx == 3)
        {
            return "반드시 적중하는 화살을 발사한다.";
        }

        else if (idx == 4)
        {
            return "반드시 적중하는 에너지를 발사한다.";
        }

        else if (idx == 5)
        {
            return "반드시 적중하는 에너지를 발사한다.";
        }

        return "";
    }

    public static string GetSkillDescription(int idx)
    {
        if (idx == 0)
        {
            return "머리를 향해 2번 내려친다.";
        }

        else if (idx == 1)
        {
            return "도약 후 회전하여 6번 내려친다.";
        }

        else if (idx == 2)
        {
            return "전방의 적을 향해 4번 찌른다.";
        }

        else if (idx == 3)
        {
            return "기를 모아 강력한 화살을 발사한다.";
        }

        else if (idx == 4)
        {
            return "기를 모아 강력한 에너지를 발사한다.";
        }

        else if (idx == 5)
        {
            return "모든 아군의 체력을 자신의\n최대체력 20% 만큼 회복시킨다.";
        }

        return "";
    }
}
