using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateUI : MonoBehaviour
{
    public enum AnimateType
    {
        Move,
        PingPong,
        Rotate,
        Opacity,
        FillAmount
    }

    [System.Serializable]
    public class AnimationData
    {
        public AnimateType type;
        public float speed;
        public float duration;
        public float timer;
    }


    [Header("애니메이션 리스트")]
    [SerializeField] private List<AnimationData> _animations;

    [Header("이동할 위치")]
    [SerializeField] private Vector2 _targetPosition;   // 최종 위치
    private Vector2 _startPosition;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup; // 오퍼시티용. 자식까지 써야할 수 있어 Graphic가 아니라 Canvas Group으로 합니다.
    private Image _image; // FillAmount용

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        if (_rect == null)
        {
            Debug.Log("Rect 없음. 인스펙터 확인");
        }
        _startPosition = _rect.anchoredPosition;


        if (_animations.Exists(n => n.type == AnimateType.Opacity)) 
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                Debug.Log("오퍼시티 사용 불가. Canvas Group 없음");
            }
        }


        if (_animations.Exists(n => n.type == AnimateType.FillAmount)) 
        {
            _image = GetComponent<Image>();

            if (_image == null)
            {
                Debug.Log("FillAmount 사용 불가. 이미지 없음");
            }

        }
    }

    private void Update()
    {
        float t = Time.deltaTime;

        foreach (AnimationData a in _animations)
        {
            if (a.timer >= a.duration)
                continue;

            switch (a.type)
            {
                case AnimateType.Move:
                    MoveUI(a);
                    break;

                case AnimateType.PingPong:
                    PingPongUI(a);
                    break;

                case AnimateType.Rotate:
                    RotateUI(a.speed, t);
                    break;

                case AnimateType.Opacity:
                    OpacityUI(a.speed, t);
                    break;

                case AnimateType.FillAmount:
                    FillAmountUI(a.speed, t);
                    break;
            }

            a.timer += t;
        }
    }


    // 해당 위치로 이동
    private void MoveUI(AnimationData n)
    {
        float t = n.timer / n.duration;

        _rect.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition, t);
    }

    // 쓸 지는 모르겠는데 핑퐁
    private void PingPongUI(AnimationData n)
    {
        float t = Mathf.PingPong(n.timer * n.speed, 1f);
        _rect.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition, t);
    }

    // 회전
    private void RotateUI(float speed, float t)
    {
        _rect.Rotate(Vector3.forward * speed * t);
    }

    // 투명도
    private void OpacityUI(float speed, float t)
    {
        if (_canvasGroup == null) return;

        _canvasGroup.alpha += speed * t;
        _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha);
    }

    // 채우기 (스킬쿨타임 등)
    private void FillAmountUI(float speed, float t)
    {
        if (_image == null || _image.type != Image.Type.Filled) return;
        _image.fillAmount += speed * t;
    }

}