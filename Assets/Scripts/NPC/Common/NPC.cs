﻿using System.Collections;
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

    private Animator _anim = default;
    private NPCStateMachine _nPCStateMachine = default;
    private IdleState _idleState = default;
    private AvoidState _avoidState = default;
    private Vector3 _avoidPoint = default;
    private NavMeshAgent _navMeshAgent = default;
    private WaitForSeconds _wfs = default;
    private GameObject _lookAtTarget = default;
    private bool _canLookAt = default;
    [Tooltip("（待機時間の）時間計算")] private float _timer = 0f;
    [Tooltip("（待機時間の）時間計算するか")] private bool _isTimer = false;
    [Tooltip("コルーチンでの待ち時間")] private float _waitTime = default;
    [Header("待機状態の継続時間")] [Tooltip("待機状態の継続時間")]
    [SerializeField] private float _idleTime = 2f;
    [Header("立ちか座りか(待機と作業に影響)")] [Tooltip("立ちか座りか(待機と作業に影響)")]
    [SerializeField] private bool _isStand = default;
    [Header("レイを出す始点")] [Tooltip("レイを出す始点")]
    [SerializeField] private GameObject _startPoint = default;
    [Header("レイの方向")] [Tooltip("レイの方向")]
    [SerializeField] private Vector3 _direction = default;
    [Header("レイの長さ")] [Tooltip("レイの長さ")]
    [SerializeField] private float _raycastLength = 2f;
    [Header("レイを出す始点(壁越しさせない用)")] [Tooltip("レイを出す始点(壁越しさせない用)")]
    [SerializeField] private GameObject _rayOrigin = default;
    
    #endregion

    #region"プロパティ"

    /// <summary> アニメーター </summary>
    public Animator Anim => _anim;

    /// <summary> ナビメッシュ コンポーネント </summary>
    public NavMeshAgent NavMeshAgent => _navMeshAgent;

    /// <summary> ステートマシン </summary>
    protected NPCStateMachine NpcStateMachine => _nPCStateMachine;

    /// <summary> 回避先の位置情報 </summary>
    public Vector3 AvoidPoint
    {
        get => _avoidPoint;
        set => _avoidPoint = value;
    }

    /// <summary> 立ちか座りか </summary>
    public bool IsStand => _isStand;

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

    private void Start()
    {
        _idleState = new IdleState(this);
        _avoidState = new AvoidState(this);
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _nPCStateMachine = new NPCStateMachine();
        _timer = 0f;
        NpcStateMachine.ChangeState(_idleState);
        _waitTime = 2f;
        _wfs = new WaitForSeconds(_waitTime);
        _canLookAt = false;

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
            _nPCStateMachine.ChangeState(_idleState);
        }

        DecideAvoidPoint();
        // テスト用 アニメーション終了したときに実行するメソッドを呼び出す
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Stagger();
        }

        if (_canLookAt)
        {
            _navMeshAgent.isStopped = true;
            LookAtPlayer();
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
    private void DecideAvoidPoint()
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
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter(other);

        // プレイヤーが透明化中なら即リターン
        if (other.gameObject.layer == LayerMask.NameToLayer("TransparentPlayer")){ return; }
        // プレイヤーなら
        if (other.CompareTag("Player") || other.CompareTag("RecordedPlayer"))
        {
            // 壁越しの回避をさせない
            var origin = _rayOrigin.transform.position;
            if(Physics.Raycast(origin, other.transform.position - origin, out RaycastHit hit))
            {
                if(hit.collider.gameObject == other.gameObject)
                {
                    _nPCStateMachine.ChangeState(_avoidState);
                }
            }
        }
    }

    protected virtual void TriggerEnter(Collider other)
    {
    }

    /// <summary>
    /// NPC自体にプレイヤーが接触したら、よろけるアニメーションを再生
    /// ※アニメーション内でAnimationEventを設定する想定
    /// ※プレイヤーが透明化中でもぶつかる
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("RecordedPlayer"))
        {
            // 無効化
            _navMeshAgent.isStopped = true;
            // よろけるアニメーション再生
            if (_anim)
            {
                _anim.SetTrigger("Collision");
            }
            else
            {
                Debug.Log("アニメーターが設定されていません");
            }
            var dir = other.transform.position - transform.position;
            var pos = transform.position;
            transform.position = pos - dir.normalized; // otherとは逆方向に移動
            _isTimer = true;
            _lookAtTarget = other.gameObject;
            // Debug.Log("よろける！！");
        }
    }

    /// <summary>
    /// TODO: よろめくアニメーションが、終了するタイミングで呼ぶ(AnimationEvent)
    /// 変更するステート：回避ステート
    /// </summary>
    public void Stagger()
    {
        _navMeshAgent.isStopped = true;
        _canLookAt = true;
        _anim.ResetTrigger("Collision");
        StartCoroutine(AvoidStateCoroutine());
    }

    private IEnumerator AvoidStateCoroutine()
    {
        _nPCStateMachine.ChangeState(_idleState);
        yield return _wfs;
        _lookAtTarget = null;
        _canLookAt = false;
        _navMeshAgent.isStopped = false;
        _nPCStateMachine.ChangeState(_avoidState);
    }

    /// <summary>
    /// ぶつかったプレイヤーを見る
    /// </summary>
    private void LookAtPlayer()
    {
        if(_lookAtTarget == null) return;
        var pos = _lookAtTarget.transform.position;
        var to = pos - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, to, Vector3.up);
        // 角度が5゜を越えていたら
        if (Mathf.Abs(angle) > 5)
        {
            _anim.SetFloat("Speed", 2f);
            float rotMax = 100f * Time.deltaTime;
            float rot = Mathf.Min(Mathf.Abs(angle), rotMax);
            transform.Rotate(0f, rot * Mathf.Sign(angle), 0f);
        }
        else
        {
            _canLookAt = false;
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
        if (_npc.Anim)
        {
            _npc.Anim.SetBool(_npc.IsStand ? "Stand" : "Sit", true); // 立っている : 座っている
            _npc.Anim.SetFloat("Speed", 0);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }

        // Debug.Log("Enter: Idle state");
    }

    public override void Update()
    {
        // Debug.Log("Update: Idle state");
    }

    public override void Exit()
    {
        if (_npc.Anim)
        {
            _npc.Anim.SetFloat("Speed", _npc.NavMeshAgent.speed);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
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
        if (_npc.Anim)
        {
            _npc.Anim.SetFloat("Speed", _npc.NavMeshAgent.speed);
            _npc.Anim.SetBool("Stand", true);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        
        //有効化
        _npc.NavMeshAgent.isStopped = false;
        // 回避先をめざして移動
        _npc.NavMeshAgent.SetDestination(_npc.AvoidPoint);
        //Debug.Log("Enter: Avoid state");
    }

    public override void Update()
    {
        // 回避先に着いたらアイドルステートへ
        if (_npc.NavMeshAgent.remainingDistance <= 0.5f)
        {
            _npc.IsTimer = true;
        }

        // Debug.Log("Update : Avoid state");
    }

    public override void Exit()
    {
        //Debug.Log("Exit : Avoid state");
    }
}

#endregion