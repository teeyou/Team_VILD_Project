using UnityEngine;
using UnityEngine.UI;

public class UIButtonSfxAutoBinder : MonoBehaviour
{
    [SerializeField] private string _defaultClickSfxName = "UIClick";

    private void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            if (button.GetComponent<UIButtonSfx>() != null)
                continue;

            UIButtonSfx sfxPlayer = button.gameObject.AddComponent<UIButtonSfx>();
        }
    }
}