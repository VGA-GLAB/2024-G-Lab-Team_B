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
    private Vector3 _targetPos = default;
    private bool _canRotate = false;

    /// <summary> 薬 </summary>
    public GameObject Medicine
    {
        get => _medicine;
        set => _medicine = value;
    }

    protected override void OnStart()
    {
        _canPlay = true;
        if (_medicine != null)
        {
            _targetPos = _medicine.transform.position;
            // ターゲットのY座標を自分と同じにすることで2次元に制限する。
            _targetPos.y = transform.position.y;
            transform.LookAt(_targetPos);
            _medicine.SetActive(false);
        }

        _animator.Play("TakeMedicine");
    }
    
    protected override void OnUpdate()
    {
        if (_canPlay)
        {
            if (_isDead)
            {
                if (_deathAvoidance == null) 
                    Debug.LogWarning("再生するアニメーションのステート名を設定してください。");
                else
                {
                    LatePlayAnim(_latePlayStateName);
                    if(_canRotate) Rotate();
                }
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
                    // StartCoroutine(LateTransition(_deathAvoidance));
                }
            }
        }
    }

    /// <summary>
    /// 回転
    /// </summary>
    void Rotate()
    {
        var to = _targetPos - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, -to, Vector3.up);
        // 角度が35゜を越えていたら
        if (Mathf.Abs(angle) > 35)
        {
            float rotMax = 200f * Time.deltaTime;
            float rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }
        else _canRotate = false;
    }

    /// <summary>
    /// 薬を飲むアニメーションクリップで呼ぶ
    /// </summary>
    public void ChangeRotateFlag()
    {
        _canRotate = true;
    }

    // protected override void Transition()
    // {
    //     _animator.SetBool(name, true);
    // }
}
