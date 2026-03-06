using TMPro;
using UnityEngine;

public class HW_TMP : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _textArr; 
    void Start()
    {
        _textArr[0].text = "TEAM VILD 화이팅";
        _textArr[1].text = "다들 믿습니다 ";
        _textArr[2].text = "저희 잘되겠죠...?";
        _textArr[3].text = "주제선정부터 쉽지가 않네요.........";
        _textArr[4].text = "김태웅입니다.";
    }
}
