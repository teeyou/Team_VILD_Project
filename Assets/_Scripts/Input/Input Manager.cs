using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InputManager : Singleton<InputManager>
{
    public bool IsPressedS { get; private set; } = false;   // 스테이터스
    public bool IsPressedI { get; private set; } = false;   // 인벤토리
    public bool IsPressedM { get; private set; } = false;   // 맵
    public bool IsPressedESC { get; private set; } = false; // 설정 또는 종료

    // Test Code
    public bool IsPressedNum1 { get; private set; } = false;
    public bool IsPressedNum2 { get; private set; } = false;
    public bool IsPressedNum3 { get; private set; } = false;
    public bool IsPressedNum4 { get; private set; } = false;

    public bool IsPressedSpace { get; private set; } = false;

    private void Update()
    {
        IsPressedS = Input.GetKeyDown(KeyCode.S);

        IsPressedI = Input.GetKeyDown(KeyCode.I);

        IsPressedM = Input.GetKeyDown(KeyCode.M);

        IsPressedESC = Input.GetKeyDown(KeyCode.Escape);

        // Test Code
        IsPressedNum1 = Input.GetKeyDown(KeyCode.Alpha1);
        IsPressedNum2 = Input.GetKeyDown(KeyCode.Alpha2);
        IsPressedNum3 = Input.GetKeyDown(KeyCode.Alpha3);
        IsPressedNum4 = Input.GetKeyDown(KeyCode.Alpha4);

        IsPressedSpace = Input.GetKeyDown(KeyCode.Space);
    }
}
