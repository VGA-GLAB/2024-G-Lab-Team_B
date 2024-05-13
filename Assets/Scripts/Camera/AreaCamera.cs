using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//日本語対応
public class AreaCamera : MonoBehaviour
{
    CameraSwitcher _cameraSwitcher;

    [SerializeField, Header("一人称カメラ")]
    private CinemachineVirtualCameraBase _inAreaFirstPerson;//一人称

    [SerializeField, Header("三人称カメラ")]
    private CinemachineFreeLook _inAreaThirdPerson;//三人称

    private bool _isAreaCheng = false;//エリアが変わったかどうか
    public bool IsAreaCheng { get => _isAreaCheng; set => _isAreaCheng = value; }

    public CinemachineVirtualCameraBase InAreaFirstPerson { get => _inAreaFirstPerson; set => _inAreaFirstPerson = value; }

    public CinemachineFreeLook InAreaThirdPerson { get => _inAreaThirdPerson; set => _inAreaThirdPerson = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            _inAreaFirstPerson.Priority = 10;
            //_inAreaThirdPerson.Priority = 10;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _inAreaFirstPerson.Priority = 0;
        _inAreaThirdPerson.Priority = 0;
    }
}
