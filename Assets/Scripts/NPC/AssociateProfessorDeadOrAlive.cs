using UnityEngine;
/// <summary>
/// 死亡モーションをする
/// IsDeadが偽になると、立ち上がる
/// 倒れてから残り時間10秒になる前までは、死亡フラグを偽にできる
/// </summary>
public class AssociateProfessorDeadOrAlive : DeadOrAliveBase
{
    [Header("数秒後に再生するステート名"), Tooltip("数秒後に再生するステート名")]
    [SerializeField] private string _latePlayStateName = default;
    [Header("秒数カウント開始"), Tooltip("秒数カウント開始")]
    private bool _isCount = default;
    [Header("残り時間を持つクラス"), Tooltip("残り時間を持つクラス")]
    protected CountDownTimer _countDownTimer = default;
    
    protected override void OnStart()
    {
        _countDownTimer = FindObjectOfType<CountDownTimer>();
        _canPlay = true;
        _animator.Play("Unwell"); // 倒れるまでは共通
        _isCount = false;
    }
    
    protected override void OnUpdate()
    {

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Laying Seizure"))
        {
            _isCount = true;
        }

        // AEDの数秒に立ち上がる
        if (_isCount)
        {
            LatePlayAnim(_latePlayStateName);
            _isCount = false;
        }
        
        // 残り時間が１０秒をきったら、フラグの干渉をできなくする
        if(_countDownTimer.Timer <= 10f){return;}
        
        if (!IsDead)
        {
            if (_canPlay)
            {
                _animator.SetTrigger("ElectricShock"); // AED
                _canPlay = false;
            }
        }
    }
}
