using System.Linq;
using UnityEngine;

/// <summary>
/// NPCクラスのサブクラス　テスト用
/// パトロール・立ち作業
/// (パトロールのみもアリ)
/// </summary>
public class PatrolNPC : NPC
{
    #region 変数

    // パトロール関係
    [Space] [Header("===パトロール関係===")] [Header("移動速度"), SerializeField] [Tooltip("移動速度")]
    float _speed = 1f;

    [Tooltip("巡回ステート")] PatrolState _patrolState = default;

    [Header("経路となるオブジェクトの親オブジェクト"), SerializeField] [Tooltip("経路となるオブジェクトの親オブジェクト")]
    GameObject _parentRoute = default;


    [Tooltip("経路の位置情報")] Vector3[] _positions = default;
    [Tooltip("到達した場所のインデックス番号")] int _reachIndexNum = default;
    [Tooltip("めざす場所のインデックス番号")] int _indexNum = default;

    [Header("各ポジションに到達したら、毎度その場で一時停止するか"), SerializeField] [Tooltip("各ポジションに到達したら、毎度その場で一時停止するか")]
    bool _isWait = default;

    // 立ち作業関係
    [Space] [Header("===立ち作業関係===")] [Header("立ち作業の時間  ※巡回再開までにアイドル時間もかかる"), SerializeField] [Tooltip("立ち作業の時間")]
    float _standingWorkTime = default;

    [Tooltip("立ち作業ステート")] StandingWorkState _standingWorkState = default;

    [Tooltip("立ち作業する場所：インデックス番号")] int[] _standingWorkPositionIndexes = default;
    [Header("立ち作業する場所"), SerializeField] GameObject[] _standingWorkPositions = default;

    #endregion

    #region プロパティ

    /// <summary> 経路の位置情報 </summary>
    public Vector3[] Positions
    {
        get => _positions;
    }

    /// <summary> めざす場所のインデックス番号 </summary>
    public int IndexNum
    {
        get => _indexNum;
        set => _indexNum = value;
    }

    /// <summary> 立ち作業の時間 </summary>
    public float StandingWorkTime
    {
        get => _standingWorkTime;
    }

    /// <summary> 各ポジションに到達したら、毎度その場で一時停止するか </summary>
    public bool IsWait
    {
        get => _isWait;
        //set => _isWait = value;
    }

    #endregion

    protected override void OnStart()
    {
        _patrolState = new PatrolState(this);
        NavMeshAgent.speed = _speed;
        _indexNum = 0; // 最初の目標地点
        // 経路を取得
        int childCount = _parentRoute.transform.childCount;
        _positions = new Vector3[childCount];
        for (var i = 0; i < Positions.Length; i++)
        {
            Positions[i] = _parentRoute.transform.GetChild(i).transform.position;
        }

        _standingWorkState = new StandingWorkState(this);
        // 立ち作業の場所を入れる
        _standingWorkPositionIndexes = new int[_standingWorkPositions.Length];
        for (var i = 0; i < _standingWorkPositions.Length; i++)
        {
            var positionIndex = _standingWorkPositions[i];
            _standingWorkPositionIndexes[i] = positionIndex.transform.GetSiblingIndex();
        }

        NpcStateMachine.ChangeState(_patrolState);
    }

    protected override void OnUpdate()
    {
        ToDefaultState(_patrolState);

        // 到達した瞬間に作業場所かどうか見る
        if (_standingWorkPositionIndexes.Contains(_reachIndexNum + 1) && _reachIndexNum != _indexNum - 1)
        {
            NpcStateMachine.ChangeState(_standingWorkState);
        }

        _reachIndexNum = _indexNum - 1;
    }
}

#region ステート機能

/// <summary>
/// パトロール機能
/// </summary>
public class PatrolState : StateBase
{
    PatrolNPC _patrolNpc;

    public PatrolState(PatrolNPC owner) : base(owner)
    {
        _patrolNpc = owner;
    }

    public override void Enter()
    {
        // TODO: 歩くアニメーション再生

        //有効化
        _npc.NavMeshAgent.isStopped = false;
        //Debug.Log("Enter: Patrol state");
    }

    public override void Update()
    {
        _npc.NavMeshAgent.SetDestination(_patrolNpc.Positions[_patrolNpc.IndexNum]);
        float distance = (_npc.transform.position - _patrolNpc.Positions[_patrolNpc.IndexNum]).sqrMagnitude;
        // だいたい近づいたら到達と見做す
        if (distance <= 1f)
        {
            _patrolNpc.IndexNum++; // 次の目標地点を更新
            if (_patrolNpc.IsWait)
            {
                _npc.IsTimer = true;
            }
        }

        if (_patrolNpc.IndexNum == _patrolNpc.Positions.Length)
        {
            _patrolNpc.IndexNum = 0;
        }

        // 回転
        Vector3 nextCorner = _npc.transform.position;
        if (_npc.NavMeshAgent.path != null && _npc.NavMeshAgent.path.corners.Length > 1)
        {
            nextCorner = _npc.NavMeshAgent.path.corners[1];
            Debug.DrawLine(_npc.transform.position, nextCorner, Color.yellow);
        }

        var to = nextCorner - _npc.transform.position;
        float angle = Vector3.SignedAngle(_npc.transform.forward, to, Vector3.up);
        // 角度が45゜を越えていたら
        if (Mathf.Abs(angle) > 45)
        {
            float rotMax = _npc.NavMeshAgent.angularSpeed * Time.deltaTime;
            float rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            _npc.transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }

        Debug.Log("Update: Patrol state");
    }

    public override void Exit()
    {
        //無効化
        _npc.NavMeshAgent.isStopped = true;
        //Debug.Log("Exit: Patrol state");
    }
}


/// <summary>
/// 立ち作業する場所で留まる機能
/// </summary>
public class StandingWorkState : StateBase
{
    float _timer = default;
    PatrolNPC _patrol;

    public StandingWorkState(PatrolNPC owner) : base(owner)
    {
        _patrol = owner;
    }

    public override void Enter()
    {
        // TODO: 立ち作業のアニメーション再生

        // Debug.Log("Enter: StandingWork state");
    }

    // 立ち作業の時間を超えたら、立ち作業を終える
    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _patrol.StandingWorkTime)
        {
            Exit();
        }

        Debug.Log("Update : StandingWork state");
    }

    public override void Exit()
    {
        _timer = 0f;
        _patrol.IsTimer = true;
        // Debug.Log("Exit : StandingWork state");
    }
}

#endregion