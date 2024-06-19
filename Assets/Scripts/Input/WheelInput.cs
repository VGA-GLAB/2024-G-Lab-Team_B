using System;
using UnityEngine;

public class WheelInput
{
    private event Action<int> OnWheelUp;
    private event Action<int> OnWheelDown;

    protected bool IsWheelInputUp => Input.GetAxis("Mouse ScrollWheel") > 0.05f;
    protected bool IsWheelInputDown => Input.GetAxis("Mouse ScrollWheel") < -0.05f;

    /// <summary>
    /// マウスホイールが上昇したときのAction
    /// 引数の値には「-1」が割り当てられるものとする
    /// </summary>
    public void RegisterWheelUpEvent(params Action<int>[] actions)
    {
        foreach (var actionItem in actions) { OnWheelUp += actionItem; }
    }

    /// <summary>
    /// マウスホイールが下降したときのAction
    /// 引数の値には「1」が割り当てられるものとする
    /// </summary>
    public void RegisterWheelDownEvent(params Action<int>[] actions)
    {
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
