using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : Singleton<ParticleSystemManager>
{
    [Serializable]
    private class Particle
    {
        public string name;
        public ParticleSystem ps;
    }

    [SerializeField] private List<Particle> _psList;

    public Dictionary<string, ParticleSystem> ParticleDict { get; private set; } = new Dictionary<string, ParticleSystem>();

    protected override void Awake()
    {
        base.Awake();

        Init();

        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        ParticleDict.Clear();

        for (int i = 0; i < _psList.Count; i++)
        {
            ParticleDict[_psList[i].name] = _psList[i].ps;
        }
    }
    public void Play(string psName, Vector3 pos)
    {
        if (ParticleDict.TryGetValue(psName, out ParticleSystem psPrefab))
        {
            ParticleSystem ps = Instantiate(psPrefab, pos, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);
        }
    }
}
