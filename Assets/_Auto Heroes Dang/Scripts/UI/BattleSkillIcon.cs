using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
        버튼 클릭 할 때마다 호출되게 하지 않고,
        BattleUIManager에서 TrySkill 함수 내에서 isPossible일 경우 이 애니메이션 호출하도록 변경 필요.

*/


public class BattleSkillIcon : MonoBehaviour
{
    private Image _mask;
    private bool _isAnimating = false;
    private float _timer = 0f;
    private float _duration = 1f;

    private void Awake()
    {
        _mask = GetComponent<Image>();
    }

    private void OnEnable()
    {
        ResetAnimate();
    }

    private void Update()
    {
        if (!_isAnimating) return;

        // 임시 코드 - 스킬 자원이 2칸 이상없는 경우 애니메이션 금지
        if (!BattleUIManager.Instance.CheckSKillPossible())
        {
            return;
        }

        _timer += Time.deltaTime;
        float progress = _timer / _duration;

        _mask.fillAmount = Mathf.Lerp(1f, 0f, progress);

        if (progress >= 1.0f)
        {
            StopAnimate();
        }
    }

    // 외부에서 호출
    public void Play(float duration)
    {
        _duration = duration;
        _timer = 0f;
        _isAnimating = true;
        _mask.fillAmount = 1f;
    }

    public void StopAnimate()
    {
        _isAnimating = false;
        _mask.fillAmount = 0f;
    }

    public void ResetAnimate()
    {
        _isAnimating = false;
        _timer = 0f;
        if (_mask != null) _mask.fillAmount = 0f;
    }

}
