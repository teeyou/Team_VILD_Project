using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif


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
    [SerializeField] private AudioSource _loopSfxSource;

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

    [Header("볼륨 파라미터")]
    [SerializeField] private string _masterVolumeParameter = "MasterVolume";
    [SerializeField] private string _bgmVolumeParameter = "BGMVolume";
    [SerializeField] private string _sfxVolumeParameter = "SFXVolume";

    [Header("설정 UI")]
    [SerializeField] private GameObject _configCanvas;
    [SerializeField] private GameObject _configPanel;
    [SerializeField] private GameObject _settingButtonObject;

    private Coroutine _forceCharacterCaptureBgmRoutine;
    [SerializeField] private string _characterCaptureSceneName = "CharacterCapture";
    [SerializeField] private string _characterCaptureBgmName = "Ending";
    [SerializeField] private string _fieldBgmName = "FieldBGM";

    private float _masterVolumeValue = 1f;
    private float _bgmVolumeValue = 1f;
    private float _sfxVolumeValue = 1f;

    private bool _muteSfxByScene = false;

    private const string MASTER_VOLUME_KEY = "Audio_Master";
    private const string BGM_VOLUME_KEY = "Audio_BGM";
    private const string SFX_VOLUME_KEY = "Audio_SFX";

    private Dictionary<string, Clip> _clipDict = new Dictionary<string, Clip>();
    private Dictionary<string, SceneBGM> _sceneBgmDict = new Dictionary<string, SceneBGM>();

    private string _currentBgmName = string.Empty;

    public string CurrentBgmName => _currentBgmName;

    public bool IsConfigOpen => _configPanel != null && _configPanel.activeSelf;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
            return;

        Init();
        DontDestroyOnLoad(gameObject.transform.root.gameObject);
    }

    private void Start()
    {
        LoadVolumeSettings();
        BindVolumeSliders();

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
        UnbindVolumeSliders();
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
        StopLoopSFX();

        if (_forceCharacterCaptureBgmRoutine != null)
        {
            StopCoroutine(_forceCharacterCaptureBgmRoutine);
            _forceCharacterCaptureBgmRoutine = null;
        }

        if (scene.name == _characterCaptureSceneName)
        {
            if (_bgmSource != null)
            {
                _bgmSource.Stop();
                _bgmSource.clip = null;
            }

            _currentBgmName = string.Empty;
            PlayBGM(_characterCaptureBgmName);

            _forceCharacterCaptureBgmRoutine = StartCoroutine(Co_ForceCharacterCaptureBGM());
            return;
        }

        bool isLoadingScene = scene.name == _loadingSceneName;

        _muteSfxByScene = isLoadingScene;
        ApplyAllVolumes();

        if (_settingButtonObject != null)
            _settingButtonObject.SetActive(!isLoadingScene);

        if (_configCanvas != null && isLoadingScene)
            _configCanvas.SetActive(false);

        if (isLoadingScene)
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
        if (SceneManager.GetActiveScene().name == _characterCaptureSceneName)
            return;

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

    public void PlayLoopSFX(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return;

        if (_clipDict.TryGetValue(clipName, out Clip clipData))
        {
            if (_loopSfxSource.clip == clipData.clip && _loopSfxSource.isPlaying)
                return;

            _loopSfxSource.clip = clipData.clip;
            _loopSfxSource.volume = clipData.volume;
            _loopSfxSource.loop = true;
            _loopSfxSource.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager : Loop SFX 클립 없음 - {clipName}");
        }
    }

    public void StopLoopSFX()
    {
        if (_loopSfxSource == null)
            return;

        if (!_loopSfxSource.isPlaying)
            return;

        _loopSfxSource.Stop();
        _loopSfxSource.clip = null;
    }

    private void BindVolumeSliders()
    {
        if (_sliderMaster != null)
            _sliderMaster.onValueChanged.AddListener(SetMasterVolume);

        if (_sliderBGM != null)
            _sliderBGM.onValueChanged.AddListener(SetBGMVolume);

        if (_sliderSFX != null)
            _sliderSFX.onValueChanged.AddListener(SetSFXVolume);
    }

    private void UnbindVolumeSliders()
    {
        if (_sliderMaster != null)
            _sliderMaster.onValueChanged.RemoveListener(SetMasterVolume);

        if (_sliderBGM != null)
            _sliderBGM.onValueChanged.RemoveListener(SetBGMVolume);

        if (_sliderSFX != null)
            _sliderSFX.onValueChanged.RemoveListener(SetSFXVolume);
    }

    private void LoadVolumeSettings()
    {
        _masterVolumeValue = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        _bgmVolumeValue = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        _sfxVolumeValue = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        ApplyAllVolumes();

        if (_sliderMaster != null)
            _sliderMaster.value = _masterVolumeValue;

        if (_sliderBGM != null)
            _sliderBGM.value = _bgmVolumeValue;

        if (_sliderSFX != null)
            _sliderSFX.value = _sfxVolumeValue;
    }

    private void ApplyAllVolumes()
    {
        ApplyVolume(_masterVolumeParameter, _masterVolumeValue);
        ApplyVolume(_bgmVolumeParameter, _bgmVolumeValue);

        float finalSfx = _muteSfxByScene ? 0f : _sfxVolumeValue;
        ApplyVolume(_sfxVolumeParameter, finalSfx);
    }

    public void SetMasterVolume(float value)
    {
        _masterVolumeValue = value;
        ApplyAllVolumes();

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        _bgmVolumeValue = value;
        ApplyAllVolumes();

        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        _sfxVolumeValue = value;
        ApplyAllVolumes();

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(string parameterName, float sliderValue)
    {
        if (_audioMixer == null)
            return;

        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float mixerValue = Mathf.Log10(clampedValue) * 20f;

        _audioMixer.SetFloat(parameterName, mixerValue);
    }

    private IEnumerator Co_ForceCharacterCaptureBGM()
    {
        while (SceneManager.GetActiveScene().name == _characterCaptureSceneName)
        {
            if (_currentBgmName == _fieldBgmName || !_bgmSource.isPlaying)
            {
                if (_bgmSource != null)
                {
                    _bgmSource.Stop();
                    _bgmSource.clip = null;
                }

                _currentBgmName = string.Empty;
                PlayBGM(_characterCaptureBgmName);
            }

            yield return new WaitForSeconds(0.2f);
        }

        _forceCharacterCaptureBgmRoutine = null;
    }


    public void OpenConfig()
    {
        if (_configCanvas != null)
            _configCanvas.SetActive(true);

        if (_configPanel != null)
            _configPanel.SetActive(true);
    }

    public void CloseConfig()
    {
        if (_configCanvas != null)
            _configCanvas.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}