using UnityEngine;
using UnityEngine.UI;

public class BattleTotalHpBar : MonoBehaviour
{
    [Header("팀 전체 HP Fill")]
    [SerializeField] private Image _playerHpFill;
    [SerializeField] private Image _enemyHpFill;

    private int _playerMaxTotalHp;
    private int _enemyMaxTotalHp;

    // 전투 시작 시 플레이어/적의 최대 총 HP를 저장하고
    // 바를 가득 찬 상태로 초기화하는 함수
    public void Init(int playerMaxTotalHp, int enemyMaxTotalHp)
    {
        _playerMaxTotalHp = Mathf.Max(1, playerMaxTotalHp);
        _enemyMaxTotalHp = Mathf.Max(1, enemyMaxTotalHp);

        UpdateBar(true, playerMaxTotalHp);
        UpdateBar(false, enemyMaxTotalHp);
    }

    // 현재 총 HP를 받아서 팀 전체 HP바 fillAmount를 갱신하는 함수
    // isPlayer가 true면 플레이어 팀, false면 적 팀을 갱신
    public void UpdateBar(bool isPlayer, int currentTotalHp)
    {
        if (isPlayer)
        {
            if (_playerHpFill == null)
                return;

            float ratio = (float)currentTotalHp / _playerMaxTotalHp;
            _playerHpFill.fillAmount = Mathf.Clamp01(1-ratio);
        }
        else
        {
            if (_enemyHpFill == null)
                return;

            float ratio = (float)currentTotalHp / _enemyMaxTotalHp;
            _enemyHpFill.fillAmount = Mathf.Clamp01(1 - ratio);
        }
    }
}