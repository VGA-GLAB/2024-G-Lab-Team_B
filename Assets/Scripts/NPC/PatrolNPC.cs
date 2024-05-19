using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NPCクラスのサブクラス　
/// パトロール・立ち(座り)作業
/// (パトロールのみもアリ)
/// 作業中は当たり判定を無効にしている
/// </summary>
public class PatrolNPC : NPC
{
    #region 変数

    // パトロール関係
    [Space] [Header("===パトロール関係===")] [Header("移動速度")] [Tooltip("移動速度")]
    [SerializeField] private float _speed = 1f;
    [Header("到達したとみなす距離　※推奨：2人1組の場合数字を大きく")] [Tooltip("到達したとみなす距離")]
    [SerializeField] private float _distance = 0.1f;
    [Tooltip("巡回ステート")] private PatrolState _patrolState = default;
    [Header("経路となるオブジェクトの親オブジェクト")] [Tooltip("経路となるオブジェクトの親オブジェクト")]
    [SerializeField] private GameObject _parentRoute = default;
    [Tooltip("経路の位置情報")] private Vector3[] _positions = default;
    [Tooltip("到達した場所のインデックス番号")] private int _reachIndexNum = default;
    [Tooltip("めざす場所のインデックス番号")] private int _indexNum = default;
    [Header("各ポジションに到達したら、毎度その場で一時停止するか")] [Tooltip("各ポジションに到達したら、毎度その場で一時停止するか")]
    [SerializeField] private bool _isWaitEveryTime = default;

    // 作業関係
    [Space] [Header("===作業関係===")] 
    [Header("作業の時間  ※巡回再開までにアイドル時間もかかる")] [Tooltip("作業の時間")]
    [SerializeField] private float _standingWorkTime = default;
    [Tooltip("作業ステート")] private StandingWorkState _standingWorkState = default;
    [Tooltip("作業する場所：インデックス番号")] private int[] _standingWorkPositionIndexes = default;
    [Header("作業する場所")] [SerializeField] private GameObject[] _standingWorkPositions = default;
    [Tooltip("当たり判定を無くす")] private Collider[] _colliders = default;

    #endregion

    #region プロパティ

    /// <summary> 到達したとみなす距離 </summary>
    public float Distance
    {
        get => _distance;
        // set => _distanse = value;
    }

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

    /// <summary> 作業の時間 </summary>
    public float StandingWorkTime
    {
        get => _standingWorkTime;
    }

    /// <summary> 各ポジションに到達したら、毎度その場で一時停止するか </summary>
    public bool IsWaitEveryTime
    {
        get => _isWaitEveryTime;
        //set => _isWaitEveryTime = value;
    }

    /// <summary> 当たり判定を無くす </summary>
    public Collider[] Colliders
    {
        get => _colliders;
    }

    #endregion

    protected override void OnStart()
    {
        _patrolState = new PatrolState(this);
        _reachIndexNum = -1;
        _indexNum = 0; // 最初の目標地点
        // 経路を取得
        int childCount = _parentRoute.transform.childCount;
        _positions = new Vector3[childCount];
        for (var i = 0; i < Positions.Length; i++)
        {
            Positions[i] = _parentRoute.transform.GetChild(i).transform.position;
        }

        _standingWorkState = new StandingWorkState(this);
        // 作業の場所を入れる
        _standingWorkPositionIndexes = new int[_standingWorkPositions.Length];
        for (var i = 0; i < _standingWorkPositions.Length; i++)
        {
            var positionIndex = _standingWorkPositions[i];
            _standingWorkPositionIndexes[i] = positionIndex.transform.GetSiblingIndex();
        }
        NavMeshAgent.speed = _speed;
        NpcStateMachine.ChangeState(_patrolState);

        _colliders = GetComponentsInChildren<Collider>();
    }

    /// <summary>
    /// PatrolNPCが複数付いている場合、
    /// 各PatrolNPCの速度を反映させるために、OnEnableで設定する必要がある
    /// </summary>
    private void OnEnable()
    {
        if (NavMeshAgent)
        {
            NavMeshAgent.speed = _speed;
        }
    }

    private void OnDisable()
    {
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
        // 歩くアニメーション再生
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

        //有効化
        if(_npc.NavMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid)
            _npc.NavMeshAgent.isStopped = false;
        //Debug.Log("Enter: Patrol state");
    }

    public override void Update()
    {
        _npc.NavMeshAgent.SetDestination(_patrolNpc.Positions[_patrolNpc.IndexNum]);
        float distance = (_npc.transform.position - _patrolNpc.Positions[_patrolNpc.IndexNum]).sqrMagnitude;
        // だいたい近づいたら到達と見做す
        if (distance <= _patrolNpc.Distance)
        {
            _patrolNpc.IndexNum++; // 次の目標地点を更新
            if (_patrolNpc.IsWaitEveryTime)
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

        // Debug.Log("Update: Patrol state");
    }

    public override void Exit()
    {
        //無効化
        if(_npc.NavMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid)
            _npc.NavMeshAgent.isStopped = true;
        //Debug.Log("Exit: Patrol state");
    }
}


/// <summary>
/// 作業する場所で留まる機能
/// </summary>
public class StandingWorkState : StateBase
{
    private float _timer = default;
    private PatrolNPC _patrol;

    public StandingWorkState(PatrolNPC owner) : base(owner)
    {
        _patrol = owner;
    }

    public override void Enter()
    {
        // 当たり判定無効
        foreach (var collider in _patrol.Colliders)
        {
            collider.enabled = false;
        }

        // 立ち・座り作業のアニメーション再生
        if (_npc.Anim)
        {
            _npc.Anim.SetFloat("Speed", 0);
            if (_npc.IsStand)
            {
                // 立ち作業
                _npc.Anim.Play("Standing Idle");
                _npc.Anim.SetBool("Sit", false);
                _npc.Anim.SetBool("Stand", true);
            }
            else
            {
                // 座り作業
                _npc.Anim.Play("Desk Work");
                _npc.Anim.SetBool("Stand", false);
                _npc.Anim.SetBool("Sit", true);
            }

            _npc.Anim.SetBool("Work", true);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        // Debug.Log("Enter: StandingWork state");
    }

    // 作業の時間を超えたら、作業を終える
    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _patrol.StandingWorkTime)
        {
            Exit();
        }

        // Debug.Log("Update : StandingWork state");
    }

    public override void Exit()
    {
        // 当たり判定有効
        foreach (var collider in _patrol.Colliders)
        {
            collider.enabled = true;
        }

        _timer = 0f;
        _patrol.IsTimer = true;
        if (_npc.Anim)
        {
            _npc.Anim.SetBool("Work", false);
            _npc.Anim.SetFloat("Speed", _npc.NavMeshAgent.speed);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        // Debug.Log("Exit : StandingWork state");
    }
}

#endregion