using UnityEngine;

public class InputReceiverDemo : MonoBehaviour
{
    private WheelInput _wheelInput = default;

    private int _currentIndex = 0;
    private int[] _demoArray = new int[5];

    private void Start()
    {
        _wheelInput = new();
        _wheelInput.RegisterWheelUpEvent(Demo);
        _wheelInput.RegisterWheelDownEvent(Demo);
    }

    private void Update() => _wheelInput.OnUpdate();

    private void OnDestroy() => _wheelInput.OnDestroy();

    /// <summary> サンプルの関数（配列のインデックス変更） </summary>
    private void Demo(int value)
    {
        //ここにUIの割り当て（配列のインデックスにそろえる）
        //以下Demo

        if (_currentIndex + value >= _demoArray.Length) { _currentIndex = 0; }
        else if (_currentIndex + value < 0) { _currentIndex = _demoArray.Length - 1; }
        else { _currentIndex += value; }
    }
}
