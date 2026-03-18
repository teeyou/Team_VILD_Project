using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<BaseStatus_SO> _baseStatsList;

    private void Awake()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        for (int i = 0; i < _baseStatsList.Count; i++)
        {
            GameObject go = Instantiate(_baseStatsList[i].Prefab);
            AutoAttack autoAttack = go.GetComponent<AutoAttack>();
            autoAttack.BaseStatsData = _baseStatsList[i];
        }
    }
}
