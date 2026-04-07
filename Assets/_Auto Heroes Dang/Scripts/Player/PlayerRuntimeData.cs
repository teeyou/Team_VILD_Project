using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRuntimeData
{
    private string _id;
    private string _chName;
    private int _defaultMaxHp;
    private int _defaultAtk;
    private int _defaultDef;
    private float _range;
    private float _moveSpeed;
    private float _skillCool;
    private float _skillMultiplier;
    private EAttackType _attackType;
    private GameObject _prefab;

    private int _level;
    private EGrade _grade;

    public PlayerRuntimeData(BaseStatus_SO baseStatus)
    {
        _id = baseStatus.Id;
        _chName = baseStatus.ChName;
        _defaultMaxHp = baseStatus.DefaultMaxHp;
        _defaultAtk = baseStatus.DefaultAtk;
        _defaultDef = baseStatus.DefaultDef;
        _range = baseStatus.Range;
        _moveSpeed = baseStatus.MoveSpeed;
        _skillCool = baseStatus.SkillCool;
        _skillMultiplier = baseStatus.SkillMultiplier;
        _attackType = baseStatus.AttackType;
        _prefab = baseStatus.Prefab;
        _level = baseStatus.Level;
        _grade = baseStatus.Grade;
    }

    public string Id { get { return _id; } set { _id = value; } }
    public string ChName { get { return _chName; } set { _chName = value; } }
    public int DefaultMaxHp { get { return _defaultMaxHp; } set { _defaultMaxHp = value; } }
    public int DefaultAtk { get { return _defaultAtk; } set { _defaultAtk = value; } }
    public int DefaultDef { get { return _defaultDef; } set { _defaultDef = value; } }
    public float Range { get { return _range; } set { _range = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float SkillCool { get { return _skillCool; } set { _skillCool = value; } }
    public float SkillMultiplier { get { return _skillMultiplier; } set { _skillMultiplier = value; } }
    public EAttackType AttackType { get { return _attackType; } set { _attackType = value; } }
    public GameObject Prefab { get { return _prefab; } set { _prefab = value; } }

    public int Level { get { return _level; } set { _level = value; } }
    public EGrade Grade { get { return _grade; } set { _grade = value; } }

}
