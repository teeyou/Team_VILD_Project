using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESceneId
{
    StartScene = 0,
    SelectScene = 1,
    FieldScene = 2,
    BattleScene = 3,
}

public class SceneLoader : Singleton<SceneLoader>
{
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

}
