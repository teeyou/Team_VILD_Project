using UnityEngine;
using TMPro;

/*
현재 로딩 씬 Dot에 붙어있는 컴포넌트
로딩씬에 ... 애니메이션 + 캐릭터 애니메이션 move 전환
 */

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dotText;
    [SerializeField] private string[] _dotArray;
    [SerializeField] private float _time = 0.5f;     

    private float animateTimer = 0f;
    private int index = 0;

    // -------------------테스트용. 추후 수정 필요---------------------
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        // _animator = FindAnyObjectByType<Animator>();
        _animator.SetBool("Move", true);
    }
    // ------------------------------------------------------------------ 끄는 거 세팅 필요할 수 있음

    void Update()
    {
        if (_dotArray == null || _dotText == null) 
        {
            return;
        }

        animateTimer += Time.deltaTime;

        if (animateTimer >= _time)
        {
            animateTimer -= _time;

            index++;
            if (index >= _dotArray.Length)
            {
                index = 0;
            }

            _dotText.text = _dotArray[index];
        }
    }
}
