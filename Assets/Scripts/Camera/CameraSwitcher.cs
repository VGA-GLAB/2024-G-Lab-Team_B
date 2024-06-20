using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using static CriAudioManager;
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

    // [SerializeField, Header("ダメージのインターバル")]
    // private float _damageInterval = 1f;
    //
    // [SerializeField, Header("ダメージ数")]
    // private int _damageDealt = 1;

    [SerializeField, Header("使用制限時間")] 
    private float _timeLimit = 30;

    private IDamage _damage;
    private bool _isDamageInterval = false;
    private CancellationToken _token;
    private bool _isFirstPerson = true;//一人称かどうか
    /// <summary>Trueならマウスクリックができる。Falseならできない</summary>
    private bool _isClick = false;
    //一回だけSEを呼ぶときに使う変数
    bool _playOne;
    //強制終了する際に一回だけ呼ぶときに使う変数
    bool _playOneCancelSe;

    /// <summary>TimeLimit２の音が鳴っているかの判定をする変数</summary>
    bool _isTimeLimit2;     //Falseが音が出てないTrueが音がでている

    public bool IsFirstPerson { get => _isFirstPerson; set => _isFirstPerson = value; }

    public CinemachineVirtualCameraBase FirstPerson { get => _firstPerson; set => _firstPerson = value; }

    public CinemachineFreeLook ThirdPerson { get => _thirdPerson; set => _thirdPerson = value; }
    public bool IsClick { get => _isClick; set => _isClick = value; }

    private void Start()
    {
        _damage = FindObjectOfType<PlayerHPController>().GetComponent<IDamage>();
        _token = this.GetCancellationTokenOnDestroy();

        CinemachineFreeLook _thirdPerson = _firstPerson as CinemachineFreeLook;
    }

    private void Update()
    {
        //タイムリミットの値が０より大きい時は視点の切り替えができる
        if (_timeLimit > 0)
        {
            CameraChange();         //一人称と三人称を切り替える   
        }
        //タイムリミットの値が０より小さくなったら強制的に１人称視点に切り替わる
        else
        {
            if (!_playOneCancelSe)
            {
                _firstPerson.Priority = 1;
                _thirdPerson.Priority = 0;
                _isFirstPerson = true;
                CriAudioManager.Instance.StopLoopSE();
                CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Cancellation_01");
                _thirdPerson.ChangeToFreeLook(_firstPerson);
                _playOneCancelSe = true;
            }
        }
        if (_timeLimit < 5)
        {
            if (!_playOne)      //一回だけLoopSE2を再生する
            {
                CriAudioManager.Instance.StopLoopSE();
                CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_TimeLimit_02");
                _playOne = !_playOne;
                _isTimeLimit2 = true;
            }
        }
        //_isFirstPersonがFalseなら_timeLimitを減らす
        if (!_isFirstPerson)
        {
            _timeLimit -= Time.deltaTime;
        }
        
        if (!_isFirstPerson && !_isDamageInterval)
        {
            AbilityDamage().Forget();
        }
    }
    /// <summary>強制的に1人称にするメソッド</summary>
    public void ChangeFirstPerson()
    {
        //一人称に戻す
        CriAudioManager.Instance.StopLoopSE();
        _firstPerson.Priority = 1;
        _thirdPerson.Priority = 0;
        _isFirstPerson = true;
        _thirdPerson.ChangeToFreeLook(_firstPerson);
    }

    /// <summary>
    /// 能力使用時一定間隔でダメージをくらう
    /// </summary>
    private async UniTask AbilityDamage()
    {
        _isDamageInterval = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _token);
        // _damage.SendDamage(_damageDealt);
        _isDamageInterval = false;
    }

    /// <summary>
    /// １人称と３人称を切り替える
    /// </summary>
    private void CameraChange()
    {
        if (_isClick)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if (_isFirstPerson)
                {
                    //三人称にする（Priorityの値が大きいほうが優先される）
                    _firstPerson.Priority = 0;
                    _thirdPerson.Priority = 1;
                    _isFirstPerson = false;
                    CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Use_01");
                    //trueならTImeLimit2のSEを鳴らす
                    if (_isTimeLimit2)
                    {
                        CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_TimeLimit_02");
                    }
                    //falseならTimeLimit1のSEを鳴らす
                    else
                    {
                        StartCoroutine(LimitSE());   
                    }
                    _isClick = false;
                }
                else
                {
                    //一人称に戻す
                    _firstPerson.Priority = 1;
                    _thirdPerson.Priority = 0;
                    _isFirstPerson = true;
                    CriAudioManager.Instance.StopLoopSE();
                    CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_Cancellation_01");
                    _thirdPerson.ChangeToFreeLook(_firstPerson);
                }
            }   
        }
    }

    IEnumerator LimitSE()
    {
        yield return new WaitForSeconds(1.5f);
        CriAudioManager.Instance.StopSE(0);
        if (_isFirstPerson == false)
        {
            CriAudioManager.Instance.PlaySE(CueSheetType.SE, "SE_Ability_TimeLimit_01");   
        }
        _isClick = true;

    }
}
