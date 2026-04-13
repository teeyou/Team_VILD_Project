using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString());
                    _instance = go.AddComponent<T>();
                }

            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        //if (Instance == this)
        //{
        //    _instance = null;
        //}
    }
}
