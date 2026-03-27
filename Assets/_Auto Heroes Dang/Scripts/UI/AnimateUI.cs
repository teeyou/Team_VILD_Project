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
        public float speed;     // move, fillamount 사용 안함
        public float duration;  // 핑퐁, 회전, 투명도만 사용.
        public float timer;     // 각 애니메이션이 진행한 시간. 0으로 두면 됩니다.
    }


    [Header("애니메이션 리스트")]
    [SerializeField] private List<AnimationData> _animations;

    [Header("이동할 위치")]
    [SerializeField] private Vector2 _targetPosition;   // 최종 위치
    private Vector2 _startPosition;

    private RectTransform _rect;
    private Quaternion _startRotation;

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
        _startRotation = _rect.localRotation;

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
                    FillAmountUI(a);
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

        // 필요 시 보정 사용 (0 / 1)
        // if (speed > 0 && _canvasGroup.alpha >= 0.999f) _canvasGroup.alpha = 1f;
        // else if (speed < 0 && _canvasGroup.alpha <= 0.001f) _canvasGroup.alpha = 0f;
    }

    // 채우기 (스킬쿨타임 등)
    private void FillAmountUI(AnimationData n)
    {
        if (_image == null || _image.type != Image.Type.Filled) return;

        float t = n.timer / n.duration;
        _image.fillAmount = Mathf.Lerp(1f, 0f, t);

        // 틈 보완용
        if (t >= 0.99f && _image.fillAmount != 0f)
        {
            _image.fillAmount = 0f;
        }

    }

    public void ResetAnimate()
    {
        foreach (var a in _animations)
        {
            a.timer = 0f;
        }

        if (_rect != null)
        {
            _rect.anchoredPosition = _startPosition;
            _rect.localRotation = _startRotation;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
        }

        if (_image != null && _image.type == Image.Type.Filled)
        {
            _image.fillAmount = 0f;
        }
    }

}