using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum ESceneId
{
    StartScene = 0,
    SelectScene = 1,
    FieldScene = 2,
    LoadingScene = 3,
    CharacterCapture = 4,
    Stage1_1,
    Stage1_2,
    Stage1_3,
    Stage2_1,
    Stage2_2,
    Stage2_3,
    Stage3_1,
    Stage3_2,
    Stage3_3,
    ZFieldTest,
}

public class SceneLoader : Singleton<SceneLoader>
{
    private bool _isLoading = false;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(EGameStage id)
    {
        LoadScene(id.ToString());
    }

    public void LoadScene(ESceneId id)
    {
        LoadScene(id.ToString());
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading)
        {
            return;
        }

        StartCoroutine(CoLoadScene(sceneName));
    }

    private IEnumerator CoLoadScene(string sceneName)
    {
        _isLoading = true;

        // 로딩 씬 로드
        string loadingSceneName = ESceneId.LoadingScene.ToString();
        AsyncOperation loadingOp = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        while (!loadingOp.isDone)
        {
            yield return null;
        }

        Debug.Log("로딩 씬 로드 완료");



        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f); // 로딩 씬에서 2초 대기 후 씬 전환

        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            yield return null;
        }

        // 게임 씬 전환을 Additive로 할 경우에 로딩 씬 언로드 수행

        //AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(loadingSceneName);
        //while (!unloadOp.isDone)
        //{
        //    yield return null;
        //}

        _isLoading = false;
    }
}
