using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUpperUI : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offsetUp = new Vector3(0, 2.0f, 0); // 필요 시 위로 올릴 수치

    [SerializeField] private Camera _mainCamera;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (_mainCamera == null) 
        { 
            _mainCamera = Camera.main;
        }

    }

    void LateUpdate()
    {
        if (_target == null)
        { 
            return;
        }

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(_target.position + _offsetUp);

        if (screenPos.z < 0.0f)
        {
            rectTransform.localScale = Vector3.zero;
        }
        else
        {
            rectTransform.localScale = Vector3.one;
            transform.position = screenPos;
        }
    }
}
