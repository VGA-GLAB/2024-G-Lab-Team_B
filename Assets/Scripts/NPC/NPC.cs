﻿using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 基本はパトロール
/// プレイヤーが一定距離まで接近したら、避ける
/// ※１　NavMeshをベイク必須
/// ※２　プレイヤーに「NavMeshObstacle」コンポーネントをつけるとより良い
/// （プレイヤーが静止しているときに、障害物としてみなす機能）
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
    #region"変数"

    [Tooltip("ステートマシン")] NPCStateMachine _nPCStateMachine = default;

    [Tooltip("アイドルステート")] IdleState _idleState = default;
    [Tooltip("巡回ステート")] PatrolState _patrolState = default;
    [Tooltip("回避ステート")] AvoidState _avoidState = default;
    [Tooltip("立ち作業ステート")] StandingWorkState _standingWorkState = default;

    [Header("移動速度"), SerializeField] [Tooltip("移動速度")]
    float _speed = 1f;

    [Header("経路となるオブジェクトの親オブジェクト"), SerializeField] [Tooltip("経路となるオブジェクトの親オブジェクト")]
    GameObject _parentRoute = default;

    [Tooltip("経路の位置情報")] Vector3[] _positions = default;

    [Tooltip("到達した場所のインデックス番号")] int _reachIndexNum = default;

    [Tooltip("めざす場所のインデックス番号")] int _indexNum = default;

    [Header("立ち作業の時間"), SerializeField] [Tooltip("立ち作業の時間")]
    float _standingWorkTime = default;

    [Tooltip("立ち作業する場所：インデックス番号")] int[] _standingWorkPositionIndexes = default;

    [Header("立ち作業する場所"), SerializeField] GameObject[] _standingWorkPositions = default;

    [Header("各ポジションに到達したら、毎度その場で一時停止するか"), SerializeField] [Tooltip("各ポジションに到達したら、毎度その場で一時停止するか")]
    bool _isWait = default;

    [Header("アイドル状態の継続時間"), SerializeField] [Tooltip("アイドル状態の継続時間")]
    float _idleTime = 2f;

    [Tooltip("（アイドル時間の）時間計算")] float _timer = 0f;

    [Tooltip("（アイドル時間の）時間計算するか")] bool _isTimer = false;

    NavMeshAgent _navMeshAgent = default;
    [Tooltip("回避先")] Vector3 _avoidPoint = default;

    [Header("レイを出す始点"), SerializeField] [Tooltip("レイを出す始点")]
    GameObject _startPoint = default;

    [Header("レイの方向"), SerializeField] [Tooltip("レイの方向")]
    Vector3 _direction = default;

    [Header("レイの長さ"), SerializeField] [Tooltip("レイの長さ")]
    float _raycastLength = 2f;

    #endregion

    #region"プロパティ"

    /// <summary> 速度 </summary>
    public float Speed
    {
        get => _speed;
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

    /// <summary> ナビメッシュ コンポーネント </summary>
    public NavMeshAgent NavMeshAgent
    {
        get => _navMeshAgent;
    }

    /// <summary> 回避先の位置情報 </summary>
    public Vector3 AvoidPoint
    {
        get => _avoidPoint;
        set => _avoidPoint = value;
    }

    /// <summary> （アイドル時間の）時間計算するか </summary>
    public bool IsTimer
    {
        get => _isTimer;
        set => _isTimer = value;
    }

    /// <summary> 各ポジションに到達したら、毎度その場で一時停止するか </summary>
    public bool IsWait
    {
        get => _isWait;
        //set => _isWait = value;
    }

    /// <summary> 立ち作業の時間 </summary>
    public float StandingWorkTime
    {
        get => _standingWorkTime;
    }

    #endregion

    void Start()
    {
        _idleState = new IdleState(this);
        _patrolState = new PatrolState(this);
        _avoidState = new AvoidState(this);
        _standingWorkState = new StandingWorkState(this);

        _navMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = Speed;

        _nPCStateMachine = new NPCStateMachine();
        _nPCStateMachine.ChangeState(_patrolState);

        _timer = 0f;
        IndexNum = 0; // 最初の目標地点

        // 経路を取得
        int childCount = _parentRoute.transform.childCount;
        _positions = new Vector3[childCount];
        for (var i = 0; i < Positions.Length; i++)
        {
            Positions[i] = _parentRoute.transform.GetChild(i).transform.position;
        }

        // 立ち作業の場所を入れる
        _standingWorkPositionIndexes = new int[_standingWorkPositions.Length];
        for (var i = 0; i < _standingWorkPositions.Length; i++)
        {
            var positionIndex = _standingWorkPositions[i];
            _standingWorkPositionIndexes[i] = positionIndex.transform.GetSiblingIndex();
        }
    }

    void Update()
    {
        // 更新 
        _nPCStateMachine.Update();

        // パトロール再開までは待機
        if (_isTimer)
        {
            _timer += Time.deltaTime;
            _nPCStateMachine.ChangeState(_idleState);
        }

        // 到達した瞬間に作業場所かどうか見る
        if (_standingWorkPositionIndexes.Contains(_indexNum - 1) && _reachIndexNum != _indexNum - 1)
        {
            _nPCStateMachine.ChangeState(_standingWorkState);
        }

        _reachIndexNum = _indexNum - 1;

        // 一定時間が経過したらパトロール再開 
        if (_timer > _idleTime)
        {
            _nPCStateMachine.ChangeState(_patrolState);
            _isTimer = false;
            _timer = 0f;
        }

        DecideAvoidPoint();
    }

    /// <summary>
    /// 回避先を決める
    /// </summary>
    void DecideAvoidPoint()
    {
        // 回避先を決める 
        Vector3 raycastDirection = _startPoint.transform.TransformDirection(_direction);
        RaycastHit hit;
        var startPosition = _startPoint.transform.position;
        // Raycastを発射してHitしたかどうかを検出
        if (Physics.Raycast(startPosition, raycastDirection, out hit, _raycastLength))
        {
            // レイの左右反転
            _direction.x *= -1;
            Debug.DrawLine(startPosition, hit.point, Color.red);
        }
        else
        {
            var endPoint = _startPoint.transform.position + raycastDirection * _raycastLength;
            Debug.DrawLine(startPosition, endPoint, Color.green);
            AvoidPoint = endPoint;
        }
    }

    /// <summary>
    /// プレイヤーがコライダー内に侵入したら、避けるステートに切り替える
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _nPCStateMachine.ChangeState(_avoidState);
        }
    }
}

#region 各ステートの機能

/// <summary>
/// アイドル機能
/// </summary>
public class IdleState : StateBase
{
    public IdleState(NPC owner) : base(owner)
    {
    }

    public override void Enter()
    {
        //Debug.Log("Enter: Idle state");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        //Debug.Log("Exit : Idle state");
    }
}

/// <summary>
/// パトロール機能
/// </summary>
public class PatrolState : StateBase
{
    public PatrolState(NPC owner) : base(owner)
    {
    }

    public override void Enter()
    {
        //速度を戻す
        _npc.NavMeshAgent.speed = _npc.Speed;
        //有効化
        _npc.NavMeshAgent.isStopped = false;
        //Debug.Log("Enter: Patrol state");
    }

    public override void Update()
    {
        _npc.NavMeshAgent.SetDestination(_npc.Positions[_npc.IndexNum]);
        float distance = (_npc.transform.position - _npc.Positions[_npc.IndexNum]).sqrMagnitude;
        // だいたい近づいたら到達と見做す
        if (distance <= 1f)
        {
            _npc.IndexNum++; // 次の目標地点を更新
            if (_npc.IsWait)
            {
                _npc.IsTimer = true;
            }
        }

        if (_npc.IndexNum == _npc.Positions.Length)
        {
            _npc.IndexNum = 0;
        }

        // 滑らかに進行方向へ向く
        _npc.transform.LookAt(Vector3.Lerp(_npc.transform.forward + _npc.transform.position,
            _npc.Positions[_npc.IndexNum], 0.007f));
        // Debug.Log("Update: Patrol state");
    }

    public override void Exit()
    {
        //無効化
        _npc.NavMeshAgent.isStopped = true;
        //Debug.Log("Exit: Patrol state");
    }
}

/// <summary>
/// プレイヤーを避ける 
/// </summary>
public class AvoidState : StateBase
{
    public AvoidState(NPC owner) : base(owner)
    {
    }

    public override void Enter()
    {
        //有効化
        _npc.NavMeshAgent.isStopped = false;
        // 回避先をめざして移動
        _npc.NavMeshAgent.SetDestination(_npc.AvoidPoint);
        //Debug.Log("Enter: Avoid state");
    }

    public override void Update()
    {
        // 回避先に着いたらアイドルステートへ
        if (_npc.NavMeshAgent.remainingDistance == 0.0f)
        {
            _npc.IsTimer = true;
        }

        //Debug.Log("Update : Avoid state");
    }

    public override void Exit()
    {
        //Debug.Log("Exit : Avoid state");
    }
}


/// <summary>
/// 立ち作業する場所で留まる機能
/// </summary>
public class StandingWorkState : StateBase
{
    float _timer = default;

    public StandingWorkState(NPC owner) : base(owner)
    {
    }

    public override void Enter()
    {
        //Debug.Log("Enter: StandingWork state");
    }

    // 立ち作業の時間を超えたら、立ち作業を終える
    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _npc.StandingWorkTime)
        {
            Exit();
        }

        //Debug.Log("Update : StandingWork state");
    }

    public override void Exit()
    {
        _timer = 0f;
        _npc.IsTimer = true;
        // Debug.Log("Exit : StandingWork state");
    }
}

#endregion