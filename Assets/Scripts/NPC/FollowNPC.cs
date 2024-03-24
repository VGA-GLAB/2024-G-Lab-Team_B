using UnityEngine;

/// <summary>
/// 一定距離離れたら待つ
/// </summary>
public class FollowNPC : NPC
{
    #region 変数

    [Header("相方"), Tooltip("相方")]
    [SerializeField] private Transform _targetNpcTransform = default;
    [Header("横並びになる距離"), Tooltip("横並びになる距離")]
    [SerializeField] private float _distance = 1.0f;
    [Header("速める際の速度"), Tooltip("速める際の速度")]
    [SerializeField] private float _speed = 1.8f;
    // 付いていく対象の左右の位置
    private Vector3 _right = default;
    private Vector3 _left = default;
    [Header("初期速度"), Tooltip("初期速度")] 
    [SerializeField] private float _defaultSpeed = 1f;
    [Tooltip("距離を出すための対象の位置")] private Vector3 _targetPos = default;
    [Tooltip("追尾ステート")] private FollowState _followState = default;

    #endregion

    protected override void OnStart()
    {
        NavMeshAgent.speed = _defaultSpeed;
        _followState = new FollowState(this);
        NpcStateMachine.ChangeState(_followState);
    }

    protected override void OnUpdate()
    {
        ToDefaultState(_followState);
    }

    public void Follow()
    {
        _right = _targetNpcTransform.localPosition +
                 _targetNpcTransform.TransformDirection(new Vector3(_distance, 0, 0));
        _left = _targetNpcTransform.localPosition +
                _targetNpcTransform.TransformDirection(new Vector3(-_distance, 0, 0));

        var pos = transform.position;
        var distanceToRight = Vector3.Distance(pos, _right);
        var distanceToLeft = Vector3.Distance(pos, _left);

        // より近い方に向かう
        if (distanceToRight < distanceToLeft)
        {
            // _rightがより近い場合の処理
            _targetPos = _right;
        }
        else
        {
            // _leftがより近い場合の処理
            _targetPos = _left;
        }

        // 一定距離離れていたら速度を上げる
        var d = Vector3.Distance(pos, _targetPos);
        if (d >= _distance + 0.5f)
        {
            NavMeshAgent.speed = _speed;
        }
        else
        {
            NavMeshAgent.speed = _defaultSpeed;
        }

        NavMeshAgent.SetDestination(_targetPos);
    }

    /// <summary>
    /// 付いていく対象の左右の位置をギズモで描画
    /// 黄色い２つの球体
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_right, 0.05f);
        Gizmos.DrawSphere(_left, 0.05f);
    }
}

#region ステート機能

/// <summary>
/// 追尾ステート
/// </summary>
public class FollowState : StateBase
{
    FollowNPC followNPC;

    public FollowState(FollowNPC owner) : base(owner)
    {
        followNPC = owner;
    }

    public override void Enter()
    {
        if (_npc.Anim)
        {
            _npc.Anim.SetBool("Stand", true);
            _npc.Anim.SetBool("Sit", false);
            _npc.Anim.SetFloat("Speed", _npc.NavMeshAgent.speed);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        //Debug.Log("Enter: Follow state");
    }

    public override void Update()
    {
        followNPC.Follow();
        // Debug.Log("Update: Follow state");
    }

    public override void Exit()
    {
        //Debug.Log("Exit: Follow state");
    }
}

#endregion