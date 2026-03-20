using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [System.Serializable]
    private class Clip
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _sliderMaster;
    [SerializeField] private Slider _sliderBGM;
    [SerializeField] private Slider _sliderSFX;

    [SerializeField] private List<Clip> _clipList;
    public Dictionary<string, AudioClip> ClipDict { get; private set; } = new Dictionary<string, AudioClip>();


    protected override void Awake()
    {
        base.Awake();

        Init();
        
        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        ClipDict.Clear();

        for (int i = 0; i < _clipList.Count; i++)
        {
            ClipDict[_clipList[i].name] = _clipList[i].clip;
        }
    }


    public void PlayBGM(string clipName)
    {
        if (ClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            if (_bgmSource.clip == clip)
            {
                return;
            }

            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        else
        {
            Debug.Log("클립 없음. 오타 확인");
        }

    }

    public void StopBGM()
    {
        if (_bgmSource.clip == null)
        {
            return;
        }

        _bgmSource.Stop();
    }

    public void PlaySFX(string clipName)
    {
        if (ClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            _sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("클립 없음. 오타 확인");
        }
    }
}
