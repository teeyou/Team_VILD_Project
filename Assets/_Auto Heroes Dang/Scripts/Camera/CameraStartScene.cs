using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStartScene : MonoBehaviour
{
    void Update()
    {
        if (InputManager.Instance.IsMouseLeftDown)
        {
            SceneLoader.Instance.LoadScene(ESceneId.SelectScene);
        }
    }
}
