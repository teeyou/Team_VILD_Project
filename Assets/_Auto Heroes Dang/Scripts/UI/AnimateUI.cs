using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateUI : MonoBehaviour
{
    public enum AnimateType
    {
        Move,
        MoveBack,
        PingPong,
        Rotate,
        Opacity,
        FillAmount,
        // 추후 스케일 추가할 수도 있음
    }

    [System.Serializable]
    public class AnimationData
    {
        public AnimateType type;
        public float speed;     // move(Back), fillamount 사용 안함
        public float duration;  // 재생 시간. bool 이 아닌 9999 사용
        public bool playAnimate = false;  // 체크 시 자동으로 실행

        [HideInInspector]  public float timer;     // 0으로 두면 됩니다. (각 애니메이션이 진행한 시간)
    }


    [Header("애니메이션 리스트")]
    [SerializeField] private List<AnimationData> _animations;

    [Header("끝나면 꺼짐 여부")]
    [SerializeField] private bool _endSetActive = false;

    [Header("이동할 위치")]
    [SerializeField] private Vector2 _targetPosition;   // 최종 위치
    private Vector2 _startPosition;

    private RectTransform _rect;
    private Quaternion _startRotation;

    private CanvasGroup _canvasGroup; // 오퍼시티용. 자식까지 써야할 수 있어 Graphic가 아니라 Canvas Group으로 합니다.
    private Image _image; // FillAmount용

    private bool _animateOn = false;


    private void Awake()
    {
        _animateOn = false;

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

    // active 상태에서 자동 실행
    private void OnEnable()
    {
        foreach (AnimationData a in _animations)
        {
            if (a.playAnimate) _animateOn = true;
        }
    }

    // 비활성화 시 수치 초기화
    private void OnDisable()
    {
        ResetAnimate();
    }

    private void Update()
    {
        if (!_animateOn)
        {
            return;
        }

        float t = Time.deltaTime;
        bool allAnimateEnd = true;

        foreach (AnimationData a in _animations)
        {
            if (!a.playAnimate) continue;

            //보정
            if (a.timer >= a.duration)
            {
                if (a.type == AnimateType.Move) _rect.anchoredPosition = _targetPosition;
                if (a.type == AnimateType.MoveBack) _rect.anchoredPosition = _startPosition;
                if (a.type == AnimateType.FillAmount) _image.fillAmount = 0f;

                continue;
            }

            allAnimateEnd = false;

            switch (a.type)
            {
                case AnimateType.Move:
                    MoveUI(a);
                    break;

                case AnimateType.MoveBack:
                    MoveBackUI(a);
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

        if (allAnimateEnd)
        {
            _animateOn = false;

            if (_endSetActive)
            {
                gameObject.SetActive(false);
            }
        }

    }

    
    // 해당 위치로 이동
    private void MoveUI(AnimationData n)
    {
        float t = n.timer / n.duration;

        if (t >= 0.99f)
        {
            _rect.anchoredPosition = _targetPosition;
        }
        else 
        {
            _rect.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition, t);
        }

    }

    // 복귀
    private void MoveBackUI(AnimationData n)
    {
        float t = n.timer / n.duration;

        if (t >= 0.99f)
        {
            _rect.anchoredPosition = _startPosition;
        }
        else 
        {
            _rect.anchoredPosition = Vector2.Lerp(_targetPosition, _startPosition, t);
        }

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

    // 채우기 = 마스크를 0으로 하는 방식 (스킬쿨타임 등)
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

    // 호출용 (전체 실행)
    public void PlayAnimate()
    {
        ResetAnimate();
        _animateOn = true;

        foreach (AnimationData a in _animations)
        {
            a.playAnimate = true;
        }
    }

    //호출용 일부 실행. enum 기준(AnimateType)
    public void PlayAnimate(int enumIndex)
    {
        ResetAnimate();
        _animateOn = true;

        foreach (AnimationData a in _animations)
        {
            if (a.type == (AnimateType)enumIndex)
            {
                a.playAnimate = true;
            }
        }

    }


    public void ResetAnimate()
    {
        foreach (AnimationData a in _animations)
        {
            a.timer = 0f;
            a.playAnimate = false;
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

    public void SetAnimDuration(float duration)
    {
        for (int i = 0; i < _animations.Count; i++)
        {
            _animations[i].duration = duration;
        }
    }
}