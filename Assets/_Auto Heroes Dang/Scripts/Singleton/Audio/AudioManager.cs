using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
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
        [Range(0f, 1f)] public float volume = 1f;
    }

    [System.Serializable]
    private class SceneBGM
    {
        public string sceneName;
        public bool keepCurrentBgm = false;
        public string bgmClipName;
    }

    [System.Serializable]
    private class StageBattleBGM
    {
        public string stagePrefix;
        public string battleBgmName;
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
    [SerializeField] private List<Clip> _clipList = new List<Clip>();

    [Header("씬별 BGM 설정")]
    [SerializeField] private List<SceneBGM> _sceneBgmList = new List<SceneBGM>();

    [Header("스테이지별 전투 BGM 설정")]
    [SerializeField] private List<StageBattleBGM> _stageBattleBgmList = new List<StageBattleBGM>();

    [Header("로딩씬 이름")]
    [SerializeField] private string _loadingSceneName = "LoadingScene";

    [Header("기본 BGM 이름")]
    [SerializeField] private string _defaultFieldBgmName = "FieldBGM";
    [SerializeField] private string _defaultBattleBgmName = "Battle1BGM";
    [SerializeField] private string _victoryBgmName = "VictoryBGM";

    private Dictionary<string, Clip> _clipDict = new Dictionary<string, Clip>();
    private Dictionary<string, SceneBGM> _sceneBgmDict = new Dictionary<string, SceneBGM>();

    private string _currentBgmName = string.Empty;

    public string CurrentBgmName => _currentBgmName;

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
        _clipDict.Clear();
        _sceneBgmDict.Clear();

        for (int i = 0; i < _clipList.Count; i++)
        {
            Clip clipData = _clipList[i];

            if (clipData == null || clipData.clip == null || string.IsNullOrEmpty(clipData.name))
                continue;

            _clipDict[clipData.name] = clipData;
        }

        for (int i = 0; i < _sceneBgmList.Count; i++)
        {
            SceneBGM sceneData = _sceneBgmList[i];

            if (sceneData == null || string.IsNullOrEmpty(sceneData.sceneName))
                continue;

            _sceneBgmDict[sceneData.sceneName] = sceneData;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _loadingSceneName)
            return;

        if (_sceneBgmDict.TryGetValue(scene.name, out SceneBGM sceneBgmData))
        {
            if (sceneBgmData.keepCurrentBgm)
                return;

            if (!string.IsNullOrEmpty(sceneBgmData.bgmClipName))
            {
                PlayBGM(sceneBgmData.bgmClipName);
                return;
            }
        }

        Debug.LogWarning($"AudioManager : {scene.name} 씬에 대응되는 BGM 설정이 없습니다.");
    }

    public void PlayBGM(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        if (_clipDict.TryGetValue(clipName, out Clip clipData))
        {
            if (_bgmSource.clip == clipData.clip && _bgmSource.isPlaying)
                return;

            _bgmSource.clip = clipData.clip;
            _bgmSource.volume = clipData.volume;
            _bgmSource.loop = true;
            _bgmSource.Play();

            _currentBgmName = clipName;
        }
        else
        {
            Debug.LogWarning($"AudioManager : BGM 클립 없음 - {clipName}");
        }
    }

    public void PlaySFX(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        if (_clipDict.TryGetValue(clipName, out Clip clipData))
        {
            _sfxSource.PlayOneShot(clipData.clip, clipData.volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager : SFX 클립 없음 - {clipName}");
        }
    }

    public void PlayFieldBGM()
    {
        PlayBGM(_defaultFieldBgmName);
    }

    public void PlayBattleBGMForCurrentStage()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string battleBgmName = GetBattleBGMNameByScene(currentSceneName);

        PlayBGM(battleBgmName);
    }

    public void PlayVictoryBGM()
    {
        PlayBGM(_victoryBgmName);
    }

    private string GetBattleBGMNameByScene(string sceneName)
    {
        for (int i = 0; i < _stageBattleBgmList.Count; i++)
        {
            StageBattleBGM data = _stageBattleBgmList[i];

            if (data == null)
                continue;

            if (string.IsNullOrEmpty(data.stagePrefix))
                continue;

            if (string.IsNullOrEmpty(data.battleBgmName))
                continue;

            if (sceneName.StartsWith(data.stagePrefix))
                return data.battleBgmName;
        }

        return _defaultBattleBgmName;
    }
}