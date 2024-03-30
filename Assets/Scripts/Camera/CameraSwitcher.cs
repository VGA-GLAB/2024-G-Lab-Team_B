using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField,Header("一人称カメラ")]
    private CinemachineVirtualCameraBase _firstPerson;//一人称

    [SerializeField, Header("三人称カメラ")] 
    private CinemachineFreeLook _thirdPerson;//三人称

    [SerializeField, Header("ダメージのインターバル")]
    private float _damageInterval = 1f;

    [SerializeField, Header("ダメージ数")]
    private int _damageDealt = 1;

    private IDamage _damage;
    private bool _isDamageInterval = false;
    private CancellationToken _token;
    private bool _isFirstPerson = true;//一人称かどうか

    public bool IsFirstPerson { get => _isFirstPerson; set => _isFirstPerson = value; }

    public CinemachineVirtualCameraBase FirstPerson { get => _firstPerson; set => _firstPerson = value; }

    public CinemachineFreeLook ThirdPerson { get => _thirdPerson; set => _thirdPerson = value; }

    private void Start()
    {
        _damage = FindObjectOfType<PlayerHPController>().GetComponent<IDamage>();
        _token = this.GetCancellationTokenOnDestroy();
    }

    private void Update()
    {
        CameraChange();

        if (!_isFirstPerson && !_isDamageInterval)
        {
            AbilityDamage().Forget();
        }
    }

    /// <summary>
    /// 能力使用時一定間隔でダメージをくらう
    /// </summary>
    private async UniTask AbilityDamage()
    {
        _isDamageInterval = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _token);
        _damage.SendDamage(_damageDealt);
        _isDamageInterval = false;
    }

    /// <summary>
    /// １人称と３人称を切り替える
    /// </summary>
    private void CameraChange()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (_isFirstPerson)
            {
                //三人称にする（Priorityの値が大きいほうが優先される）
                _firstPerson.Priority = 0;
                _thirdPerson.Priority = 1;
                _isFirstPerson = false;
            }
            else
            {
                //一人称に戻す
                _firstPerson.Priority = 1;
                _thirdPerson.Priority = 0;
                _isFirstPerson = true;
            }
        }
    }
}
