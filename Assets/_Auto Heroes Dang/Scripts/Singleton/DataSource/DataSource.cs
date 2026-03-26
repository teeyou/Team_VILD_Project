using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSource : Singleton<DataSource>
{
    public int MainCharacterIdx { get; set; } = -1;
    private List<int> _playerCharacterList = new List<int>();

    private List<Transform> _characterTransformList = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        AddCharacter(ECharacterNumber.TwoHand_TU);
        AddCharacter(ECharacterNumber.SpearMan_JH);
        AddCharacter(ECharacterNumber.BowMan_JJ);
    }

    public List<int> GetCharacterList() => _playerCharacterList;

    public void AddCharacter(ECharacterNumber number)
    {
        _playerCharacterList.Add((int)number);
    }

    public void SetTransform(int idx, Transform t)
    {
        Debug.Log($"{idx} - Transform Set {t}");
        _characterTransformList[idx] = t;
    }

    public Transform GetTransform(int idx) => _characterTransformList[idx];
}
