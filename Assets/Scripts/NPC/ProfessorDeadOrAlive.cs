using UnityEngine;

/// <summary>
/// 薬を飲む　→　倒れるor待機
/// IsDeadのフラグが偽なら、その場で待機する
/// </summary>
public class ProfessorDeadOrAlive : DeadOrAliveBase
{
    [Header("薬"), Tooltip("薬")]
    [SerializeField] private GameObject _medicine = default;
    [Header("数秒後に再生するステート名"), Tooltip("数秒後に再生するステート名")]
    [SerializeField] private string _latePlayStateName = default;

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
                LatePlayAnim(_latePlayStateName);
            }
        }
    }
}
