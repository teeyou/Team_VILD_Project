using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [System.Serializable]
    private class Clip
    {
        public string name;
        public AudioClip clip;
    }

    [System.Serializable]
    private class SceneBGM
    {
        public string sceneName;
        public string bgmClipName;
    }

    [Header("오디오 소스")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("오디오 믹서")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("볼륨 슬라이더")]
    [SerializeField] private Slider _sliderMaster;
    [SerializeField] private Slider _sliderBGM;
    [SerializeField] private Slider _sliderSFX;

    [Header("클립 목록")]
    [SerializeField] private List<Clip> _clipList;

    [Header("씬별 BGM 설정")]
    [SerializeField] private List<SceneBGM> _sceneBgmList;

    [Header("로딩씬 이름")]
    [SerializeField] private string _loadingSceneName = "LoadingScene";

    public Dictionary<string, AudioClip> ClipDict { get; private set; } = new Dictionary<string, AudioClip>();

    private Dictionary<string, string> _sceneToBgmDict = new Dictionary<string, string>();

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        Init();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneLoaded(currentScene, LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Init()
    {
        ClipDict.Clear();
        _sceneToBgmDict.Clear();

        for (int i = 0; i < _clipList.Count; i++)
        {
            if (_clipList[i] == null || _clipList[i].clip == null || string.IsNullOrEmpty(_clipList[i].name))
                continue;

            ClipDict[_clipList[i].name] = _clipList[i].clip;
        }

        for (int i = 0; i < _sceneBgmList.Count; i++)
        {
            if (_sceneBgmList[i] == null)
                continue;

            if (string.IsNullOrEmpty(_sceneBgmList[i].sceneName))
                continue;

            if (string.IsNullOrEmpty(_sceneBgmList[i].bgmClipName))
                continue;

            _sceneToBgmDict[_sceneBgmList[i].sceneName] = _sceneBgmList[i].bgmClipName;
        }
    }

    // 씬이 로드될 때 호출
    // 로딩씬이면 기존 BGM 유지
    // 실제 게임 씬이면 그 씬에 맞는 BGM으로 변경
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _loadingSceneName)
        {
            return;
        }

        if (_sceneToBgmDict.TryGetValue(scene.name, out string bgmClipName))
        {
            PlayBGM(bgmClipName);
        }
        else
        {
            Debug.LogWarning($"AudioManager : {scene.name} 씬에 대응되는 BGM이 없습니다.");
        }
    }

    public void PlayBGM(string clipName)
    {
        if (ClipDict.TryGetValue(clipName, out AudioClip clip))
        {
            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
            {
                return;
            }

            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager : BGM 클립 없음 - {clipName}");
        }
    }

    public void StopBGM()
    {
        if (_bgmSource.clip == null)
            return;

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
            Debug.LogWarning($"AudioManager : SFX 클립 없음 - {clipName}");
        }
    }
}