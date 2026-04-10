using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/Character Status Data", fileName = "CharacterData_")]
public class BaseStatus_SO : ScriptableObject
{
    [SerializeField] private string _displayName;
    [SerializeField] private string _id;
    [SerializeField] private string _chName;
    [SerializeField] private int _defaultMaxHp;
    [SerializeField] private int _defaultAtk;
    [SerializeField] private int _defaultDef;
    [SerializeField] private float _range;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _skillCool;
    [SerializeField] private float _skillMultiplier;
    [SerializeField] private EAttackType _attackType;
    [SerializeField] private GameObject _prefab;

    [SerializeField] private int _level;
    [SerializeField] private EGrade _grade;

    public string DisplayName { get { return _displayName; } }
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
