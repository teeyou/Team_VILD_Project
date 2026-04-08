using TMPro;
using UnityEngine;

public class StagePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _stageTitleText;
    [SerializeField] private TMP_Text _goldRewardText;

    [SerializeField] private TMP_Text _bossStageTitleText;
    [SerializeField] private TMP_Text _bossGoldRewardText;

    public void RefreshUI()
    {
        _stageTitleText.text = GameManager.Instance.GetCurrentStageDisplayName();
        _goldRewardText.text = GameManager.Instance.GetCurrentStageGoldReward().ToString("N0");

        _bossStageTitleText.text = GameManager.Instance.GetCurrentStageDisplayName();
        _bossGoldRewardText.text = GameManager.Instance.GetCurrentStageGoldReward().ToString("N0");
    }
}