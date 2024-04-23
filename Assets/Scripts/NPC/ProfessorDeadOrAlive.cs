using System.Collections;
using UnityEngine;

/// <summary>
/// 薬を飲む　→　倒れるor伸び
/// IsDeadのフラグが偽なら、その場で待機する
/// </summary>
public class ProfessorDeadOrAlive : DeadOrAliveBase
{
    [Header("薬"), Tooltip("薬")]
    [SerializeField] private GameObject _medicine = default;
    [Header("数秒後に再生するステート名"), Tooltip("数秒後に再生するステート名")]
    [SerializeField] private string _latePlayStateName = default;
    [Header("死亡回避後のアニメーション"), Tooltip("死亡回避後のアニメーション")]
    [SerializeField] private string _deathAvoidance = default;

    /// <summary> 薬 </summary>
    public GameObject Medicine
    {
        get => _medicine;
        set => _medicine = value;
    }

    protected override void OnStart()
    {
        _canPlay = true;
        Vector3 targetPos = _medicine.transform.position;
        // ターゲットのY座標を自分と同じにすることで2次元に制限する。
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        _medicine.SetActive(false);
    }
    
    protected override void OnUpdate()
    {
        if (_canPlay)
        {
            if (_isDead)
            {
                if (_deathAvoidance == null) 
                    Debug.LogWarning("再生するアニメーションのステート名を設定してください。");
                else LatePlayAnim(_latePlayStateName);
            }
            else
            {
                // 死亡回避
                if (_deathAvoidance == null) 
                    Debug.LogWarning("死亡回避した後に、再生するアニメーションのステート名を設定してください。");
                else
                {
                    LatePlayAnim(_deathAvoidance);
                    // // Todo: 遷移のパラメータが確定したら以下を使用する
                    // StartCoroutine(LateTransition());
                }
            }
        }
    }

    protected override void Transition()
    {
        // _animator.SetBool("nobi", true);
    }
}
