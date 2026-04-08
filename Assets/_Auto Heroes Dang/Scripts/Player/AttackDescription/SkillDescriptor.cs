using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDescriptor : MonoBehaviour
{
    [SerializeField] private string[] attackDescriptions;
    [SerializeField] private string[] skillDescriptions;

    public string GetAttackDescription(int idx)
    {
        if (idx < 0 || idx >= attackDescriptions.Length)
        {
            return "";
        }

        return attackDescriptions[idx];
    }

    public string GetSkillDescription(int idx)
    {
        if (idx < 0 || idx >= attackDescriptions.Length)
        {
            return "";
        }

        return skillDescriptions[idx];
    }
}
