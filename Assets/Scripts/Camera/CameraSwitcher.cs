using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCameraBase vcam1;//一人称
    public CinemachineVirtualCameraBase vcam2;//三人称

    public bool IsFirstPerson;

    private void Start()
    {
        IsFirstPerson = true;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightShift) && IsFirstPerson == true)
        {
            //三人称にする（Priorityの値が大きいほうが優先される）
            vcam1.Priority = 0;
            vcam2.Priority = 1;
            IsFirstPerson = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightShift) && IsFirstPerson == false)
        {
            //一人称に戻す
            vcam1.Priority = 1;
            vcam2.Priority = 0;
            IsFirstPerson = true;
        }
    }
}
