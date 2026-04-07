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

    public string Id { get { return _id; } }
    public string ChName { get { return _chName; } }
    public int DefaultMaxHp { get { return _defaultMaxHp; } }
    public int DefaultAtk { get { return _defaultAtk; } }
    public int DefaultDef { get { return _defaultDef; } }
    public float Range { get { return _range; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float SkillCool { get { return _skillCool; } }
    public float SkillMultiplier { get { return _skillMultiplier; } }
    public EAttackType AttackType { get { return _attackType; } }
    public GameObject Prefab { get { return _prefab; } }

    public int Level { get { return _level; } }
    public EGrade Grade { get { return _grade; } }

}
