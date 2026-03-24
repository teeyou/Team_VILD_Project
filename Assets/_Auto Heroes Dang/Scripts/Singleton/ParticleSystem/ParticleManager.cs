using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    [Serializable]
    private class Particle
    {
        public string name;
        public ParticleSystem ps;
    }

    [SerializeField] private List<Particle> _psList;

    [SerializeField] private List<Particle> _hitList;

    public Dictionary<string, ParticleSystem> ParticleDict { get; private set; } = new Dictionary<string, ParticleSystem>();

    private Transform _vfxRoot = null;

    protected override void Awake()
    {
        base.Awake();

        Init();

        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        if (_vfxRoot == null)
        {
            _vfxRoot = new GameObject("VFX Root").transform;
        }

        ParticleDict.Clear();

        for (int i = 0; i < _psList.Count; i++)
        {
            ParticleDict[_psList[i].name] = _psList[i].ps;
        }

        for (int i = 0; i < _hitList.Count; i++)
        {
            ParticleDict[_hitList[i].name] = _hitList[i].ps;
        }

    }
    public GameObject Play(string psName, Vector3 pos, Quaternion rot)
    {
        if (ParticleDict.TryGetValue(psName, out ParticleSystem psPrefab))
        {
            ParticleSystem ps = Instantiate(psPrefab, pos, rot, _vfxRoot);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);

            return ps.gameObject;
        }

        Debug.LogError("!!! Particle Manager - Play Error !!!");
        return null;
    }

    public GameObject Play(string psName, Vector3 pos)
    {
        if (ParticleDict.TryGetValue(psName, out ParticleSystem psPrefab))
        {
            ParticleSystem ps = Instantiate(psPrefab, pos, Quaternion.identity, _vfxRoot);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);

            return ps.gameObject;
        }


        Debug.LogError("!!! Particle Manager - Play Error !!!");
        return null;
    }
}
