using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InputManager : Singleton<InputManager>
{
    public bool IsPressedS { get; private set; } = false;   // 스테이터스
    public bool IsPressedI { get; private set; } = false;   // 인벤토리
    public bool IsPressedM { get; private set; } = false;   // 맵
    public bool IsPressedH { get; private set; } = false;   // 마을 (홈)
    public bool IsPressedE { get; private set; } = false;   // 대장간
    public bool IsPressedESC { get; private set; } = false; // 설정 또는 종료

    public bool IsPressedSpace { get; private set; } = false;

    public bool IsMouseLeftDown { get; private set; } = false;
    public bool IsMouseLeftStay { get; private set; } = false;
    public bool IsMouseLeftUp { get; private set; } = false;

    private void Update()
    {
        IsPressedS = Input.GetKeyDown(KeyCode.S);

        IsPressedI = Input.GetKeyDown(KeyCode.I);

        IsPressedM = Input.GetKeyDown(KeyCode.M);

        IsPressedH = Input.GetKeyDown(KeyCode.H);

        IsPressedE = Input.GetKeyDown(KeyCode.E);

        IsPressedESC = Input.GetKeyDown(KeyCode.Escape);

        IsPressedSpace = Input.GetKeyDown(KeyCode.Space);

        IsMouseLeftDown = Input.GetMouseButtonDown(0);
        IsMouseLeftStay = Input.GetMouseButton(0);
        IsMouseLeftUp = Input.GetMouseButtonUp(0);
    }
}
