using UnityEngine;

public class WheelInput
{
    public bool IsWheelInputUp => Input.GetAxis("Mouse ScrollWheel") > 0.05f;
    public bool IsWheelInputDown => Input.GetAxis("Mouse ScrollWheel") < -0.05f;

    public void OnUpdate()
    {
        if (IsWheelInputUp) { Demo(-1); }
        if (IsWheelInputDown) { Demo(1); }
    }

    private void Demo(int value)
    {
        //ここにUIの割り当て（配列のインデックスにそろえる）
        //以下Demo
        int currentIndex = 0;
        int[] demoArray = new int[5];

        if (currentIndex + value >= demoArray.Length) { currentIndex = 0; }
        else if (currentIndex + value < 0) { currentIndex = demoArray.Length - 1; }
        else { currentIndex += value; }
    }
}
