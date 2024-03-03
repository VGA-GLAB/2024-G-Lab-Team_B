using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField,Header("一人称カメラ")]
    private CinemachineVirtualCameraBase firstPerson;//一人称
    [SerializeField, Header("三人称カメラ")] 
    private CinemachineFreeLook thirdPerson;//三人称

    public bool IsFirstPerson;//一人称かどうか

    private void Start()
    {
        IsFirstPerson = true;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightShift) && IsFirstPerson == true)
        {
            //三人称にする（Priorityの値が大きいほうが優先される）
            FirstPerson.Priority = 0;
            ThirdPerson.Priority = 1;
            IsFirstPerson = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightShift) && IsFirstPerson == false)
        {
            //一人称に戻す
            FirstPerson.Priority = 1;
            ThirdPerson.Priority = 0;
            IsFirstPerson = true;
        }
    }
    public CinemachineVirtualCameraBase FirstPerson
    {
        get { return firstPerson; }
    }

    public CinemachineVirtualCameraBase ThirdPerson
    {
        get { return thirdPerson; }
    }

    public int FirstPersonPriority
    {
        
        set { firstPerson.Priority = value; }
    }

    public int ThirdPersonPriority
    {
        
        set { thirdPerson.Priority = value; }
    }
}
