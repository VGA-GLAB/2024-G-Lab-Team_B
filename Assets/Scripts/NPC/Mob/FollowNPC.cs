using UnityEngine;

/// <summary>
/// 一定距離離れたら速度を上げる
/// 相方が停止しているとき、待機アニメーションをさせる
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
    [Header("初期速度"), Tooltip("初期速度")] 
    [SerializeField] private float _defaultSpeed = 1f;
    [Tooltip("距離を出すための対象の位置")] private Vector3 _targetPos = default;
    [Tooltip("追尾ステート")] private FollowState _followState = default;
    private Vector3 _right = default;
    private Vector3 _left = default;
    
    #endregion

    #region プロパティ

    /// <summary> 相方 </summary>
    public Transform TargetNpcTransform => _targetNpcTransform;

    /// <summary> 横並びになる距離 </summary>
    public float Distance => _distance;

    /// <summary> 距離を出すための対象の位置 </summary>
    public Vector3 TargetPos
    {
        get => _targetPos;
        set => _targetPos = value;
    }
    
    /// <summary> 付いていく対象の右側の位置 </summary>
    public Vector3 Right
    {
        get => _right;
        set => _right = value;
    }
    
    /// <summary> 付いていく対象の左側の位置 </summary>
    public Vector3 Left
    {
        get => _left;
        set => _left = value;
    }
    
    #endregion

    protected override void OnStart()
    {
        NavMeshAgent.speed = _defaultSpeed;
        _followState = new FollowState(this);
        NpcStateMachine.ChangeState(_followState);
    }

    protected override void OnUpdate()
    {
        // 一定距離離れていたら速度を上げる
        var d = Vector3.Distance(transform.position, _targetPos);
        NavMeshAgent.speed = d >= 0.5f ? _speed : _defaultSpeed;
        // 到達したら待機アニメーションにする
        if (NavMeshAgent.remainingDistance <= 0.1f)
        {
            NavMeshAgent.isStopped = true;
            Anim.SetFloat("Speed", 0);
        }
        else
        {
            Anim.SetFloat("Speed", NavMeshAgent.speed, 1f,Time.deltaTime);
            NavMeshAgent.isStopped = false;
        }
        NavMeshAgent.SetDestination(_targetPos);
        
        ToDefaultState(_followState);
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
    private FollowNPC followNPC;

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
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        // Debug.Log("Enter: Follow state");
    }

    public override void Update()
    {
        followNPC.Right = followNPC.TargetNpcTransform.position + 
                          followNPC.TargetNpcTransform.TransformDirection(new Vector3(followNPC.Distance, 0, 0.3f));
        followNPC.Left = followNPC.TargetNpcTransform.position + 
                          followNPC.TargetNpcTransform.TransformDirection(new Vector3(-followNPC.Distance, 0, 0.3f));
        var pos = followNPC.transform.position;
        var distanceToRight = Vector3.Distance(pos, followNPC.Right);
        var distanceToLeft = Vector3.Distance(pos, followNPC.Left);
        // より近い方に向かう
        followNPC.TargetPos = distanceToRight < distanceToLeft ? followNPC.Right : followNPC.Left;
        // Debug.Log("Update: Follow state");
    }

    public override void Exit()
    {
        // Debug.Log("Exit: Follow state");
    }
}

#endregion