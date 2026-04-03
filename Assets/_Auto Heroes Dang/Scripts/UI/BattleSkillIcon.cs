using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/*
빼놓기는 했는데, 배틀 UI 매니저에서 해당 애니메이션 실행되고 있음

*/


public class BattleSkillIcon : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _coolTimeText;

    private Image _mask;
    private bool _isAnimating = false;
    private float _timer = 0f;
    private float _duration = 1f;

    public bool IsAnimating => _isAnimating;

    private void Awake()
    {
        _mask = GetComponent<Image>();
        _button = GetComponentInParent<Button>();
    }

    private void OnEnable()
    {
        ResetAnimate();
        Play(3f); // 처음 시작하고 쿨다운 돎.
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

        // 쿨타임용 애니메이션
        float cool = Mathf.Max(0f, _duration - _timer);


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
        _button.interactable = false;
    }

    public void StopAnimate()
    {
        _isAnimating = false;
        _mask.fillAmount = 0f;
        _coolTimeText.text = "";
        _button.interactable = true;
    }

    public void ResetAnimate()
    {
        _isAnimating = false;
        _timer = 0f;
        if (_mask != null) _mask.fillAmount = 0f;
        if (_coolTimeText != null) _coolTimeText.text = "";
        if (_button != null) _button.interactable = true;
    }

}
