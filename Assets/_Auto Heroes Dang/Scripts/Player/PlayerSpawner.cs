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
WizardHealer_01 = 14,

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
    WizardHealer_01 = 14,
    TwoHand_TU = 15,
    SpearMan_JH = 16,
    BowMan_JJ = 17,
}

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<BaseStatus_SO> _baseStatsList;

    private void Awake()
    {
        //SpawnAllPlayer();

        //SpawnPlayer((int)ECharacterNumber.BowGirl_01, Vector3.zero, Quaternion.identity);

        // VFX 완료
        //SpawnPlayer((int)ECharacterNumber.TwoHand_TU, new Vector3(4f, 0f, 3f), Quaternion.identity);
        //SpawnPlayer((int)ECharacterNumber.SpearMan_JH, new Vector3(4f, 0f, -3f), Quaternion.identity);
        SpawnPlayer((int)ECharacterNumber.BowMan_JJ, new Vector3(4f, 0f, 0f), Quaternion.identity);

        //SpawnPlayer((int)ECharacterNumber.Shield_01, new Vector3(4f,0f,2f), Quaternion.identity);
    }

    private void Start()
    {
        //SpawnStarting();
        //TestSpawnPlayerBattleScene();
    }

    private void TestSpawnPlayerBattleScene()
    {
        IReadOnlyList<Vector3> pPosList = BattleManager.Instance.PlayerStartingPosList;
        for (int i = 0; i < pPosList.Count; i++)
        {
            SpawnPlayer(i, pPosList[i], BattleManager.Instance.PlayerStartingRotation);
        }
    }
    
    public void SpawnStarting()
    {
        /*
        BowGirl_01 = 0,
        Shield_01 = 4,
        SpearMan_01 = 6,
        TwoHand_01 = 10,
        Wizard_01 = 13,
        WizardHealer_01 = 14, 
        */

        IReadOnlyList<Vector3> pPosList = BattleManager.Instance.PlayerStartingPosList;
        //SpawnPlayer(0, pPosList[0], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer(4, pPosList[1], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer(6, pPosList[2], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer(10, pPosList[3], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer(13, pPosList[4], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer(14, pPosList[5], BattleManager.Instance.PlayerStartingRotation);

        SpawnPlayer((int)ECharacterNumber.TwoHand_TU, pPosList[0], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer((int)ECharacterNumber.SpearMan_JH, pPosList[1], BattleManager.Instance.PlayerStartingRotation);
        //SpawnPlayer((int)ECharacterNumber.BowMan_JJ, pPosList[3], BattleManager.Instance.PlayerStartingRotation);
    }

    public void SpawnAllPlayer()
    {
        for (int i = 0; i < _baseStatsList.Count; i++)
        {
            GameObject go = Instantiate(_baseStatsList[i].Prefab);
            go.transform.position = Vector3.zero;
            AutoAttack autoAttack = go.GetComponent<AutoAttack>();
            autoAttack.Init(_baseStatsList[i]);
        }
    }

    public GameObject SpawnPlayer(int idx, Vector3 pos, Quaternion rot)
    {
        GameObject go = Instantiate(_baseStatsList[idx].Prefab);
        go.transform.position = pos;
        go.transform.rotation = rot;
        AutoAttack autoAttack = go.GetComponent<AutoAttack>();
        autoAttack.Init(_baseStatsList[idx]);

        return go;
    }
}
