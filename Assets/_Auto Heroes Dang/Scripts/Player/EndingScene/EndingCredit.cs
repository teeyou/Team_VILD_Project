using System.Collections;
using UnityEngine;

public class EndingCredit : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] _canvasGroups;
    [SerializeField] private CanvasGroup _canvasOff;
    [SerializeField] private float _fadeDuration = 2.0f;
    [SerializeField] private float _show = 0.5f;         // 스탭롤 보여주는 시간
    [SerializeField] private float _wait = 0.5f;         // 다음 스탭롤 전 대기시간
    [SerializeField] private float _startDelay = 0.5f;   // 시작 전 대기시간
    [SerializeField] private float _endDelay = 0.5f;     // 종료 후 대기시간

    [SerializeField] private EndingMotion[] _characters;
    [SerializeField] private ParticleSystem _fire;

    private Canvas _audioCanvas;

    // 시간 캐싱용
    private WaitForSeconds _showTime;
    private WaitForSeconds _waitTime;

    private void Awake()
    {
        _showTime = new WaitForSeconds(_show);
        _waitTime = new WaitForSeconds(_wait);

        foreach (CanvasGroup cg in _canvasGroups) if (cg != null) cg.alpha = 0f;

        _audioCanvas = Object.FindObjectOfType<Canvas>();
        if (_audioCanvas != null) _audioCanvas.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_startDelay);

        for (int i = 0; i < _canvasGroups.Length; i++)
        {
            CanvasGroup cg = _canvasGroups[i];
            if (cg == null) continue;

            // 페이드 인
            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeDuration);
                yield return null;
            }
            cg.alpha = 1f;
            yield return _showTime; // 스탭롤 보여주는 시간

            // 페이드 아웃
            elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
                yield return null;
            }
            cg.alpha = 0f;
            yield return _waitTime; // 다음 스탭롤 전 대기시간
        }

        CanvasGroup canvas = _canvasOff;
        float e = 0f;
        while (e < _fadeDuration)
        {
            e += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(1f, 0f, e / _fadeDuration);
            yield return null;
        }
        canvas.alpha = 0f;
        _canvasOff.gameObject.SetActive(false);
        if(_audioCanvas!=null) _audioCanvas.gameObject.SetActive(true); // 오디오 버튼 활성화

        yield return new WaitForSeconds(_endDelay);

        foreach (EndingMotion motion in _characters)
        {
            motion.MotionStart();
        }
        _fire.Play();

    }

    public void GoTitleScene() 
    {
        SceneLoader.Instance.LoadScene(ESceneId.StartScene);
    }

}
