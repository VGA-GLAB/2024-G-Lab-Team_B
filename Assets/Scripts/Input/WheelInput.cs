using System;
using UnityEngine;

public class WheelInput
{
    private event Action<int> OnWheelUp;
    private event Action<int> OnWheelDown;

    protected bool IsWheelInputUp => Input.GetAxis("Mouse ScrollWheel") > 0.05f;
    protected bool IsWheelInputDown => Input.GetAxis("Mouse ScrollWheel") < -0.05f;

    /// <summary>
    /// マウスホイールの入力に対する処理の登録
    /// ホイール上昇 -> -1, 下降 -> 1 が割り当てられるとする
    /// </summary>
    /// <param name="actions"></param>
    public void RegisterWheelEvents(params Action<int>[] actions)
    {
        foreach (var actionItem in actions) { OnWheelUp += actionItem; }
        foreach (var actionItem in actions) { OnWheelDown += actionItem; }
    }

    public void OnUpdate()
    {
        if (IsWheelInputUp) { OnWheelUp?.Invoke(-1); }
        if (IsWheelInputDown) { OnWheelDown?.Invoke(1); }
    }

    public void OnDestroy()
    {
        OnWheelUp = null;
        OnWheelDown = null;
    }
}
