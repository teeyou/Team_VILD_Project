using TMPro;
using UnityEngine;

public class JH_TMP : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _textArray;
    void Start()
    {
        _textArray[0].text = "최지훈입니다";
        _textArray[1].text = "가야될 길이";
        _textArray[2].text = "험하지만";
        _textArray[3].text = "다같이 파이팅해서";
        _textArray[4].text = "프로젝트 완성해봅시다!!";
    }
}
