using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCameraBase vcam1;//��l��
    public CinemachineVirtualCameraBase vcam2;//�O�l��

    public bool IsFirstPerson;

    private void Start()
    {
        IsFirstPerson = true;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightShift) && IsFirstPerson == true)
        {
            //�O�l�̂ɂ���iPriority�̒l���傫���ق����D�悳���j
            vcam1.Priority = 0;
            vcam2.Priority = 1;
            IsFirstPerson = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightShift) && IsFirstPerson == false)
        {
            //��l�̂ɖ߂�
            vcam1.Priority = 1;
            vcam2.Priority = 0;
            IsFirstPerson = true;
        }
    }
}
