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
    private CinemachineVirtualCamera _inAreaFirstPerson;//一人称

    [SerializeField, Header("三人称カメラ")]
    private CinemachineFreeLook _inAreaThirdPerson;//三人称

    //CinemachineOrbitalTransposer _orbitalTransposer;

    public CinemachineVirtualCamera InAreaFirstPerson { get => _inAreaFirstPerson; set => _inAreaFirstPerson = value; }

    public CinemachineFreeLook InAreaThirdPerson { get => _inAreaThirdPerson; set => _inAreaThirdPerson = value; }

    [Header("どれくらい近くまでを描画するか")]
    public float _nearClipDistance;

    [Header("どれくらい遠くまでを描画するか")]
    public float _farClipDistance;

    [Header("優先するカメラの値（Priority）")]
    public int _highPriority;

    [Header("優先しないカメラの値（Priority）")]
    public int _lowPriority;

    // Start is called before the first frame update
    void Start()
    {
        _cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        //_orbitalTransposer =FindObjectOfType<CinemachineOrbitalTransposer>();
    }

    

    public void Return()
    {
        _inAreaFirstPerson.Priority = _lowPriority;
        _inAreaThirdPerson.Priority = _lowPriority;
    }

    // Update is called once per frame
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //_inAreaFirstPerson.Priority = _highPriority;
            if (_cameraSwitcher.IsFirstPerson == true)
            {
                _inAreaFirstPerson.Priority = _highPriority;

            }
            else
            {
                _inAreaThirdPerson.Priority = _highPriority;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (_cameraSwitcher.IsFirstPerson == true)
                {
                    _inAreaFirstPerson.Priority = _lowPriority;
                    _inAreaThirdPerson.Priority = _highPriority;
                }
                else
                {
                    _inAreaThirdPerson.Priority = _lowPriority;
                    _inAreaFirstPerson.Priority = _highPriority;

                        _inAreaThirdPerson.ChangeToFreeLook(_inAreaFirstPerson);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Return();
    }
}
