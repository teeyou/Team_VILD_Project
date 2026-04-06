using TMPro;
using UnityEngine;

public class StagePanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _stageTitleText;
    [SerializeField] private TMP_Text _goldRewardText;

    public void RefreshUI()
    {
        _stageTitleText.text = GameManager.Instance.GetCurrentStageDisplayName();
        _goldRewardText.text = GameManager.Instance.GetCurrentStageGoldReward().ToString("N0");
    }
}