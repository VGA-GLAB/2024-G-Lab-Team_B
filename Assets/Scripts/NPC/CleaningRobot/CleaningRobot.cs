using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 掃除ロボット
/// 行動：待機、走行のみ
/// </summary>
public class CleaningRobot : MonoBehaviour
{
    #region 変数

    private NavMeshAgent _navMeshAgent = default;
    [Tooltip("ステートマシン")] private NPCStateMachine _nPCStateMachine = default;
    private CleaningRobotIdleState _cleaningRobotIdleState = default;
    private CleaningRobotPatrolState _cleaningRobotPatrolState = default;
    
    [Header("待機状態の継続時間")] [Tooltip("待機状態の継続時間")]
    [SerializeField] private float _idleTime = 3f; 
    [Tooltip("（待機時間の）時間計算")] private float _timer = 0f;
    [Tooltip("（待機時間の）時間計算するか")] private bool _isTimer = false;
    
    [Header("移動速度"), Tooltip("移動速度")]
    [SerializeField] private float _speed = 1f;
    [Header("到達したとみなす距離")] [Tooltip("到達したとみなす距離")]
    [SerializeField] private float _distance = 0.4f;
    [Header("経路となるオブジェクトの親オブジェクト")] [Tooltip("経路となるオブジェクトの親オブジェクト")]
    [SerializeField] private GameObject _parentRoute = default;    
    [Tooltip("経路の位置情報")] private Vector3[] _positions = default;
    [Tooltip("めざす場所のインデックス番号")] private int _indexNum = default;

    [Header("レイを出す始点")] [Tooltip("レイを出す始点")]
    [SerializeField] private GameObject _startPoint = default;
    [Header("レイの方向")] [Tooltip("レイの方向")]
    [SerializeField] private Vector3 _direction = new Vector3(0, 0, 1);
    [Header("レイの長さ")] [Tooltip("レイの長さ")]
    [SerializeField] private float _raycastLength = 1f;
    
    #endregion

    #region プロパティ

    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }

    /// <summary> 到達したとみなす距離 </summary>
    public float Distance { get => _distance;  }
    
    /// <summary> 経路の位置情報 </summary>
    public Vector3[] Positions { get => _positions; }
    
    /// <summary> めざす場所のインデックス番号 </summary>
    public int IndexNum { get => _indexNum; set => _indexNum = value; }
    
    #endregion
    
    void Start()
    {
        _nPCStateMachine = new NPCStateMachine();
        _cleaningRobotIdleState = new CleaningRobotIdleState(this);
        _cleaningRobotPatrolState = new CleaningRobotPatrolState(this);
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _speed;
        
        // 経路を取得
        int childCount = _parentRoute.transform.childCount;
        _positions = new Vector3[childCount];
        for (var i = 0; i < Positions.Length; i++)
        {
            Positions[i] = _parentRoute.transform.GetChild(i).transform.position;
        }

        _nPCStateMachine.ChangeState(_cleaningRobotPatrolState);
    }

    void Update()
    {
        // 更新 
        _nPCStateMachine.Update();
        
        // デフォルトのステートの再開までは待機
        if (_isTimer)
        {
            _timer += Time.deltaTime;
            _nPCStateMachine.ChangeState(_cleaningRobotIdleState);
        }
        FrontCheck();
        ToDefaultState(_cleaningRobotPatrolState);
    }
    
    /// <summary>
    /// アイドルステート後にデフォルトのステートに戻す
    /// </summary>
    protected void ToDefaultState(StateBase stateBase)
    {
        if (_timer > _idleTime)
        {
            _nPCStateMachine.ChangeState(stateBase);
            _isTimer = false;
            _timer = 0f;
        }
    }

    /// <summary>
    /// 正面に障害物があれば待機ステートへ遷移する
    /// </summary>
    void FrontCheck()
    {
        Vector3 raycastDirection = _startPoint.transform.TransformDirection(_direction);
        RaycastHit hit;
        var startPosition = _startPoint.transform.position;
        // Raycastを発射してHitしたかどうかを検出
        if (Physics.Raycast(startPosition, raycastDirection, out hit, _raycastLength))
        {
            _isTimer = true;
            Debug.DrawLine(startPosition, hit.point, Color.red);
        }
        else
        {
            var endPoint = _startPoint.transform.position + raycastDirection * _raycastLength;
            Debug.DrawLine(startPosition, endPoint, Color.green);
        }
    }
}

#region 各ステートの機能

/// <summary>
/// アイドル機能
/// </summary>
public class CleaningRobotIdleState : StateBase
{
    public CleaningRobotIdleState(CleaningRobot owner) : base(owner)
    {
    }

    public override void Enter()
    {
        // 無効化
        _cleaningRobot.NavMeshAgent.isStopped = true;
        // Debug.Log("Enter: CleaningRobotIdleState state");
    }

    public override void Update()
    {
        // Debug.Log("Update: CleaningRobotIdleState state");
    }

    public override void Exit()
    {
        // 有効化
        _cleaningRobot.NavMeshAgent.isStopped = false;
        //Debug.Log("Exit : CleaningRobotIdleState state");
    }
}


/// <summary>
/// パトロール機能
/// </summary>
public class CleaningRobotPatrolState : StateBase
{
    public CleaningRobotPatrolState(CleaningRobot owner) : base(owner)
    {
    }

    public override void Enter()
    {
        // 有効化
        _cleaningRobot.NavMeshAgent.isStopped = false;
        //Debug.Log("Enter: CleaningRobotPatrolState state");
    }

    public override void Update()
    {
        _cleaningRobot.NavMeshAgent.SetDestination(_cleaningRobot.Positions[_cleaningRobot.IndexNum]);
        float distance = (_cleaningRobot.transform.position - _cleaningRobot.Positions[_cleaningRobot.IndexNum]).sqrMagnitude;
        var d = _cleaningRobot.Distance;
        // だいたい近づいたら到達と見做す
        if (distance <= d * d)
        {
            _cleaningRobot.IndexNum++; // 次の目標地点を更新
        }
        
        if (_cleaningRobot.IndexNum == _cleaningRobot.Positions.Length)
        {
            _cleaningRobot.IndexNum = 0;
        }
        
        // 回転
        Vector3 nextCorner = _cleaningRobot.transform.position;
        if (_cleaningRobot.NavMeshAgent.path != null && _cleaningRobot.NavMeshAgent.path.corners.Length > 1)
        {
            nextCorner = _cleaningRobot.NavMeshAgent.path.corners[1];
            Debug.DrawLine(_cleaningRobot.transform.position, nextCorner, Color.yellow);
        }
        var to = nextCorner - _cleaningRobot.transform.position;
        float angle = Vector3.SignedAngle(_cleaningRobot.transform.forward, to, Vector3.up);
        // 角度が35゜を越えていたら
        if (Mathf.Abs(angle) > 35)
        {
            float rotMax = _cleaningRobot.NavMeshAgent.angularSpeed * Time.deltaTime;
            float rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            _cleaningRobot.transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }
        // Debug.Log("Update : CleaningRobotPatrolState state");
    }

    public override void Exit()
    {
        // 無効化
        _cleaningRobot.NavMeshAgent.isStopped = true;
        //Debug.Log("Exit : CleaningRobotPatrolState state");
    }
}

#endregion