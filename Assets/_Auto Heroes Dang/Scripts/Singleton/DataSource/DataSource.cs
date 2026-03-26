using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosRotData
{
    private Vector3 _pos;
    private Quaternion _rot;

    public Vector3 Pos { get { return _pos; } set { _pos = value; } }
    public Quaternion Rot { get { return _rot; } set { _rot = value; } }
    public PosRotData(Vector3 pos, Quaternion rot)
    {
        _pos = pos;
        _rot = rot;
    }
}

public class DataSource : Singleton<DataSource>
{
    public int MainCharacterIdx { get; set; } = -1;
    private List<int> _playerCharacterList = new List<int>();

    public PosRotData MainCharacterPosRot { get; set; }

    private List<PosRotData> _subCharacterPosRotList = new List<PosRotData>();

    public int CurrentIdx { get; set; } = 0;    // track 웨이포인트의 idx

    public float CartPosition { get; set; } = 6.5f;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        AddCharacter(ECharacterNumber.TwoHand_TU);
        AddCharacter(ECharacterNumber.SpearMan_JH);
        AddCharacter(ECharacterNumber.BowMan_JJ);

        AddCharacter(ECharacterNumber.Wizard_01);
        AddCharacter(ECharacterNumber.Healer_01);
    }

    public List<int> GetCharacterList() => _playerCharacterList;

    public void AddCharacter(ECharacterNumber number)
    {
        _playerCharacterList.Add((int)number);
    }

    public void AddSubPosRot(Vector3 pos, Quaternion rot)
    {
        _subCharacterPosRotList.Add(new PosRotData(pos, rot));
    }

    public void SetSubPosRot(int idx, Vector3 pos, Quaternion rot)
    {
        _subCharacterPosRotList[idx].Pos = pos;
        _subCharacterPosRotList[idx].Rot = rot;
    }

    public PosRotData GetSubPosRot(int idx)
    {
        if (_subCharacterPosRotList.Count <= idx)
        {
            return null;
        }

        return _subCharacterPosRotList[idx];
    }
    public IReadOnlyList<PosRotData> GetSubPosRotList()
    {
        return _subCharacterPosRotList;
    }
}
