using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
스타팅

BowGirl_01 = 0,
Shield_01 = 4,
SpearMan_01 = 6,
TwoHand_01 = 10,
Wizard_01 = 13,
Healer_01 = 14,

*/
public enum ECharacterNumber
{
    BowGirl_01 = 0,
    BowMan_01 = 1,
    DoubleAx_01 = 2,
    DoubleSword_01 = 3,
    Shield_01 = 4,
    Shield_02 = 5,
    SpearMan_01 = 6,
    SpearMan_02 = 7,
    StrongShield_01 = 8,
    StrongShield_02 = 9,
    TwoHand_01 = 10,
    TwoHand_02 = 11,
    TwoHand_03 = 12,
    Wizard_01 = 13,
    Healer_01 = 14,
    TwoHand_TU = 15,
    SpearMan_JH = 16,
    BowMan_JJ = 17,
}

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<BaseStatus_SO> _baseStatsList;

    //public void SpawnAllPlayer()
    //{
    //    for (int i = 0; i < _baseStatsList.Count; i++)
    //    {
    //        GameObject go = Instantiate(_baseStatsList[i].Prefab);
    //        go.transform.position = Vector3.zero;
    //        AutoAttack autoAttack = go.GetComponent<AutoAttack>();
    //        autoAttack.Init(_baseStatsList[i], i);
    //    }
    //}

    public GameObject SpawnPlayer(int idx, Vector3 pos, Quaternion rot)
    {
        GameObject go = Instantiate(_baseStatsList[idx].Prefab);
        go.transform.position = pos;
        go.transform.rotation = rot;
        AutoAttack autoAttack = go.GetComponent<AutoAttack>();
        autoAttack.Init(_baseStatsList[idx], idx);

        return go;
    }
}
