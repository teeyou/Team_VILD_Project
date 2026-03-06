using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Jin_TMP : MonoBehaviour
{
    private enum Direction { Left, Right }

    [SerializeField] private Direction _direction;
    [SerializeField] private TextMeshProUGUI _textPrefab;
    [SerializeField] private List<string> _toWrite;

    [SerializeField] private RectTransform _canvas;
    [SerializeField] private float _x = 20f;
    [SerializeField] private float _y = 50f;

    private RectTransform _prefabRectTransfor;

    private void Start()
    {
        // _prefabRectTransfor = _textPrefab.GetComponent<RectTransform>();
        // SplitScreen(); // <-↑ 필요 시 주석 푸세요.
        PrintText();
    }

    private void PrintText()
    {
        if (_canvas == null || _textPrefab == null) return;

        for (int i = 0; i < _toWrite.Count; i++)
        {
            TextMeshProUGUI nextText = Instantiate(_textPrefab, _canvas.transform);
            nextText.text = _toWrite[i];

            RectTransform rectTransform = nextText.GetComponent<RectTransform>();

            rectTransform.pivot = new Vector2(_direction == Direction.Left ? 0 : 1, 0);
            rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot;

            switch (_direction)
            {
                case Direction.Left:
                    nextText.alignment = TextAlignmentOptions.Left;
                    rectTransform.anchoredPosition = new Vector2(i * _x, i * _y);
                    break;

                case Direction.Right:
                    nextText.alignment = TextAlignmentOptions.Right;
                    rectTransform.anchoredPosition = new Vector2(-i * _x, i * _y);
                    break;
            }

        }
    }

    private void SplitScreen()
    {
        if (_toWrite.Count <= 1)
        {
            return;
        }

        _x = (_canvas.rect.width - _prefabRectTransfor.rect.width) / (_toWrite.Count - 1);
        _y = (_canvas.rect.height - _prefabRectTransfor.rect.height) / (_toWrite.Count - 1);

    }
}
