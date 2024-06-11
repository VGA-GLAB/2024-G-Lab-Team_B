using System.Linq;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 警備員
/// 巡回：待機、歩く のみ
/// ・「毎度待機」が真でも、各待機場所の待機時間が設定されていた場合、そちらの待機時間に上書きされる
/// ・各待機場所と待機時間の順番を一致させる必要がある
/// </summary>
public class PoliceOfficer : MonoBehaviour
{
    #region"変数"
    
    private Animator _anim = default;
    private NavMeshAgent _navMeshAgent = default;
    private NPCStateMachine _nPCStateMachine = default;
    private PoliceOfficerIdleState _policeOfficerIdleState = default;
    private PoliceOfficerPatrolState _policeOfficerPatrolState = default;
    
    [Header("移動速度"), Tooltip("移動速度")]
    [SerializeField] private float _speed = 1f;
    [Header("到達したとみなす距離")] [Tooltip("到達したとみなす距離")]
    [SerializeField] private float _distance = 0.05f;
    [Header("経路となるオブジェクトの親オブジェクト")] [Tooltip("経路となるオブジェクトの親オブジェクト")]
    [SerializeField] private GameObject _parentRoute = default;
    [Tooltip("経路の位置情報")] private Vector3[] _positions = default;
    [Tooltip("到達した場所のインデックス番号")] private int _reachIndexNum = default;
    [Tooltip("めざす場所のインデックス番号")] private int _indexNum = default;
    
    [Header("各ポジションで毎度待機するか(IdleTimeに依存)")] [Tooltip("各ポジションで毎度待機するか")]
    [SerializeField] private bool _isWaitEveryTime = default;
    [Header("待機状態の継続時間")] [Tooltip("待機状態の継続時間")]
    [SerializeField] private float _idleTime = 2f; 
    [Space(25)] [Header("※以下ふたつの配列の要素数を一致させてください")]
    [Header("待機地点"), SerializeField] private GameObject[] _waitPositions = default;
    [Header("待機時間"), SerializeField] private float[] _idleTimes = default;
    [Tooltip("（待機時間の）時間計算するか")] private bool _isTimer = false; 
    [Tooltip("（待機時間の）時間計算")] private float _timer = 0f;
    [Tooltip("待機地点：インデックス番号")] private int[] _waitPositionIndexes = default;
    private float _defaultIdleTime = default;
    
    #endregion
    
    #region プロパティ
    
    /// <summary> アニメーター </summary>
    public Animator Anim => _anim;

    /// <summary> ナビメッシュ コンポーネント </summary>
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    /// <summary> ステートマシン </summary>
    protected NPCStateMachine NpcStateMachine => _nPCStateMachine;

    /// <summary> 移動速度 </summary>
    public float Speed => _speed;

    /// <summary> 到達したとみなす距離 </summary>
    public float Distance => _distance;

    /// <summary> 経路の位置情報 </summary>
    public Vector3[] Positions => _positions;

    /// <summary> めざす場所のインデックス番号 </summary>
    public int IndexNum { get => _indexNum; set => _indexNum = value; }
    
    /// <summary> 各ポジションで毎度待機するか </summary>
    public bool IsWaitEveryTime { get => _isWaitEveryTime; set => _isWaitEveryTime = value; }
    
    /// <summary> （待機時間の）時間計算するか </summary>
    public bool IsTimer { get => _isTimer; set => _isTimer = value; }
    
    /// <summary> 待機時間 </summary>
    public float IdleTime { get => _idleTime; set => _idleTime = value; }
    
    /// <summary> 待機時間 </summary>
    public float DefaultIdleTime { get => _defaultIdleTime; set => _defaultIdleTime = value; }
    
    /// <summary> 待機時間 </summary>
    public float[] IdleTimes => _idleTimes;

    /// <summary> 待機地点 </summary>
    public GameObject[] WaitPositions => _waitPositions;

    #endregion
    
    protected virtual void OnStart()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    private void Start()
    {
        _policeOfficerIdleState = new PoliceOfficerIdleState(this);
        _policeOfficerPatrolState = new PoliceOfficerPatrolState(this);
        _nPCStateMachine = new NPCStateMachine();
        _timer = 0f;
        _anim = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _defaultIdleTime = _idleTime;
        _navMeshAgent.speed = _speed;
        
        _reachIndexNum = -1;
        _indexNum = 0; // 最初の目標地点
        // 経路を取得
        int childCount = _parentRoute.transform.childCount;
        _positions = new Vector3[childCount];
        for (var i = 0; i < Positions.Length; i++)
        {
            Positions[i] = _parentRoute.transform.GetChild(i).transform.position;
        }
        
        _waitPositionIndexes = new int[_waitPositions.Length];
        for (var i = 0; i < _waitPositions.Length; i++)
        {
            var positionIndex = _waitPositions[i];
            _waitPositionIndexes[i] = positionIndex.transform.GetSiblingIndex();
        }
        NpcStateMachine.ChangeState(_policeOfficerPatrolState);

        if (_waitPositions.Length != _idleTimes.Length)
        {
            Debug.LogError("待機場所と待機時間の要素数が一致していません。");
        }
        OnStart();
    }

    private void Update()
    {
        // 更新 
        _nPCStateMachine.Update();

        // デフォルトのステートの再開までは待機
        if (_isTimer)
        {
            _timer += Time.deltaTime;
            _nPCStateMachine.ChangeState(_policeOfficerIdleState);
        }

        // 到達した瞬間に立ち止まる地点かどうか見る
        if (_waitPositionIndexes.Contains(_reachIndexNum + 1) && _reachIndexNum != _indexNum - 1)
        {
            NpcStateMachine.ChangeState(_policeOfficerIdleState);
        }
        _reachIndexNum = _indexNum - 1;   
        ToDefaultState(_policeOfficerPatrolState);
        OnUpdate();
    }
    
    /// <summary>
    /// アイドルステート後にデフォルトのステートに戻す
    /// </summary>
    private void ToDefaultState(StateBase stateBase)
    {
        if (_timer > _idleTime)
        {
            _nPCStateMachine.ChangeState(stateBase);
            _isTimer = false;
            _timer = 0f;
        }
    }
}

#region ステート機能

/// <summary>
/// アイドル機能
/// </summary>
public class PoliceOfficerIdleState : StateBase
{
    public PoliceOfficerIdleState(PoliceOfficer owner) : base(owner)
    {
    }

    public override void Enter()
    {
        if (_policeOfficer.Anim)
        {
            _policeOfficer.Anim.SetFloat("Speed", 0);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }

        // Debug.Log("Enter: PoliceOfficerIdleState state");
    }

    public override void Update()
    {
        // Debug.Log("Update: PoliceOfficerIdleState state");
    }

    public override void Exit()
    {
        if (_policeOfficer.Anim)
        {
            _policeOfficer.Anim.SetFloat("Speed", _policeOfficer.NavMeshAgent.speed);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        //Debug.Log("Exit : PoliceOfficerIdleState state");
    }
}

/// <summary>
/// 警備員のパトロール機能
/// </summary>
public class PoliceOfficerPatrolState : StateBase
{
    public PoliceOfficerPatrolState(PoliceOfficer owner) : base(owner)
    {
    }

    public override void Enter()
    {
        // 歩くアニメーション再生
        if (_policeOfficer.Anim)
        {
            _policeOfficer.Anim.SetBool("Stand", true);
            _policeOfficer.Anim.SetFloat("Speed", _policeOfficer.NavMeshAgent.speed);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }

        //有効化
        _policeOfficer.NavMeshAgent.isStopped = false;
        //Debug.Log("Enter: PoliceOfficerPatrolState state");
    }

    public override void Update()
    {
        _policeOfficer.NavMeshAgent.SetDestination(_policeOfficer.Positions[_policeOfficer.IndexNum]);
        float distance = (_policeOfficer.transform.position - _policeOfficer.Positions[_policeOfficer.IndexNum]).sqrMagnitude;
        // だいたい近づいたら到達と見做す
        if (distance <= _policeOfficer.Distance)
        {
            _policeOfficer.IndexNum++; // 次の目標地点を更新
            if (_policeOfficer.IsWaitEveryTime) // 毎度待機するとき
            {
                // その地点で待機時間の指定がないときは、デフォルトの待機時間に直す
                _policeOfficer.IdleTime = _policeOfficer.DefaultIdleTime;
                _policeOfficer.IsTimer = true;
            }
            // 待機場所ごとに待機時間を変えるとき
            if (_policeOfficer.IdleTimes.Length != 0)
            {
                for (var i = 0; i < _policeOfficer.WaitPositions.Length; i++)
                {
                    if (_policeOfficer.WaitPositions[i].transform.position
                        == _policeOfficer.Positions[_policeOfficer.IndexNum - 1])
                    {
                        _policeOfficer.IdleTime = _policeOfficer.IdleTimes[i];
                        _policeOfficer.IsTimer = true;
                        break;
                    }
                }
            }
        }

        if (_policeOfficer.IndexNum == _policeOfficer.Positions.Length)
        {
            _policeOfficer.IndexNum = 0;
        }

        // 回転
        Vector3 nextCorner = _policeOfficer.transform.position;
        if (_policeOfficer.NavMeshAgent.path != null && _policeOfficer.NavMeshAgent.path.corners.Length > 1)
        {
            nextCorner = _policeOfficer.NavMeshAgent.path.corners[1];
            Debug.DrawLine(_policeOfficer.transform.position, nextCorner, Color.yellow);
        }
        var to = nextCorner - _policeOfficer.transform.position;
        float angle = Vector3.SignedAngle(_policeOfficer.transform.forward, to, Vector3.up);
        // 角度が45゜を越えていたら
        if (Mathf.Abs(angle) > 45)
        {
            float rotMax = _policeOfficer.NavMeshAgent.angularSpeed * Time.deltaTime;
            float rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            _policeOfficer.transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }
        // Debug.Log("Update: PoliceOfficerPatrolState state");
    }

    public override void Exit()
    {
        //無効化
        _policeOfficer.NavMeshAgent.isStopped = true;
        //Debug.Log("Exit: PoliceOfficerPatrolState state");
    }
}

#endregion