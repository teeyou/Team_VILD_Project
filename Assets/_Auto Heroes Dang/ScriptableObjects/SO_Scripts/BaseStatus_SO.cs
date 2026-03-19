using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Data/Character Status Data", fileName = "CharacterData_")]
public class BaseStatus_SO : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _chName;
    [SerializeField] private int _defaultMaxHp;
    [SerializeField] private int _defaultAtk;
    [SerializeField] private int _defaultDef;
    [SerializeField] private float _range;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _skillCool;
    [SerializeField] private EAttackType _attackType;
    [SerializeField] private GameObject _prefab;

    public string Id { get { return _id; } }
    public string ChName { get { return _chName; } }
    public int DefaultMaxHp { get { return _defaultMaxHp; } }
    public int DefaultAtk { get { return _defaultAtk; } }
    public int DefaultDef { get { return _defaultDef; } }
    public float Range { get { return _range; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float SkillCool { get { return _skillCool; } }
    public EAttackType AttackType { get { return _attackType; } }
    public GameObject Prefab { get { return _prefab; } }

}
