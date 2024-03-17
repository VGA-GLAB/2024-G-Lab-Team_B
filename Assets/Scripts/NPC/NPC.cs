using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 待機・回避の機能のみをもつ
/// プレイヤーが一定距離まで接近したら、避ける
/// ぶつかったらよろけるアニメーションを再生
/// ※ 必須：NavMeshをベイク
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NPC : MonoBehaviour
{
    #region"変数"

    // [Header("アニメーター"), SerializeField] [Tooltip("アニメーター")]
    // Animator _animator = default;

    [Tooltip("ステートマシン")] NPCStateMachine _nPCStateMachine = default;

    [Tooltip("アイドルステート")] IdleState _idleState = default;
    [Tooltip("回避ステート")] AvoidState _avoidState = default;

    [Header("待機状態の継続時間"), SerializeField] [Tooltip("待機状態の継続時間")]
    float _idleTime = 2f;

    [Tooltip("（待機時間の）時間計算")] float _timer = 0f;
    [Tooltip("（待機時間の）時間計算するか")] bool _isTimer = false;

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

    /// <summary> ナビメッシュ コンポーネント </summary>
    public NavMeshAgent NavMeshAgent
    {
        get => _navMeshAgent;
    }

    /// <summary> ステートマシン </summary>
    public NPCStateMachine NpcStateMachine
    {
        get => _nPCStateMachine;
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

    #endregion

    protected virtual void OnStart()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    void Start()
    {
        _idleState = new IdleState(this);
        _avoidState = new AvoidState(this);
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _nPCStateMachine = new NPCStateMachine();
        _timer = 0f;

        OnStart();
    }

    void Update()
    {
        // 更新 
        _nPCStateMachine.Update();

        // デフォルトのステートの再開までは待機
        if (_isTimer)
        {
            _timer += Time.deltaTime;
            _nPCStateMachine.ChangeState(_idleState);
        }

        DecideAvoidPoint();
        // テスト用 アニメーション終了したときに実行するメソッドを呼び出す
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Stagger();
        }

        OnUpdate();
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
    /// ※コライダーはNPCの前面に配置していると想定
    /// ※プレイヤーが透明化中は回避しない
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("RecordedPlayer"))
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("TransparentPlayer"))
            {
                //Debug.Log(other.gameObject.name + "のレイヤー番号 : " + other.gameObject.layer);
                _nPCStateMachine.ChangeState(_avoidState);
            }
        }
    }

    /// <summary>
    /// NPC自体にプレイヤーが接触したら、よろけるアニメーションを再生
    /// ※アニメーション内でAnimationEventを設定する想定
    /// ※プレイヤーが透明化中でもぶつかる
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("RecordedPlayer"))
        {
            // 無効化
            _navMeshAgent.isStopped = true;
            // よろけるアニメーション再生

            var dir = other.transform.position - transform.position;
            var pos = transform.position;
            transform.position = pos - dir.normalized; // otherとは逆方向に移動

            Debug.Log("よろける！！");
        }
    }

    /// <summary>
    /// TODO: よろめくアニメーションが、終了するタイミングで呼ぶ(AnimationEvent)
    /// 変更するステート：回避ステート
    /// </summary>
    public void Stagger()
    {
        _nPCStateMachine.ChangeState(_avoidState);
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
        // TODO: 待機アニメーション再生

        //Debug.Log("Enter: Idle state");
    }

    public override void Update()
    {
        //Debug.Log("Update: Idle state");
    }

    public override void Exit()
    {
        //Debug.Log("Exit : Idle state");
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
        // TODO: 歩くアニメーション再生

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

#endregion