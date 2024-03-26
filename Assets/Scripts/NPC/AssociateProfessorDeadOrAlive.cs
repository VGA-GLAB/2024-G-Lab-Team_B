using UnityEngine;
/// <summary>
/// 死亡モーションをする
/// IsDeadが偽になると、立ち上がる
/// ※倒れるまではIsDeadの真偽に関係なく実行される
/// IsDeadの影響が出るのは、倒れてからになる
/// </summary>
public class AssociateProfessorDeadOrAlive : DeadOrAliveBase
{
    [Header("数秒後に再生するステート名"), Tooltip("数秒後に再生するステート名")]
    [SerializeField] private string _latePlayStateName = default;
    [Header("秒数カウント開始"), Tooltip("秒数カウント開始")]
    private bool _isCount = default;
    
    protected override void OnStart()
    {
        _canPlay = true;
        _animator.Play("Unwell"); // 倒れるまでは共通
        _isCount = false;
    }
    
    protected override void OnUpdate()
    {
        if (!IsDead)
        {
            if (_canPlay)
            {
                _animator.SetTrigger("ElectricShock"); // AED
                _canPlay = false;
            }
        }

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
    }
}
