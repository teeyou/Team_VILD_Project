using UnityEngine;
using TMPro;

public class GB_TMP : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] TMP_Text[] _texts;
    #endregion


    void Start()
    {
        _texts[0].text = "박기범 입니다.";
        _texts[1].text = "아무래도 도움을 많이 받게 될거 같습니다.";
        _texts[2].text = "코드적으로 많이 떨어지긴 하지만";
        _texts[3].text = "앞으로 잘 따라가 보도록 하겠습니다.";
        _texts[4].text = "잘 부탁드립니다";
    }


}
