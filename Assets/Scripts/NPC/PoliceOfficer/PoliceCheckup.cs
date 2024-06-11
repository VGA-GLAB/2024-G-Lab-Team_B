using UnityEngine;

/// <summary>
/// 職務質問の機能
/// N距離までプレイヤーが接近したら、走って追いかける
/// M距離までプレイヤーが接近したら、職質する
/// N>Mとする
/// </summary>
public class PoliceCheckup : PoliceOfficer
{
    #region"変数"
    
    private PoliceOfficerCheckupState _policeOfficerCheckupState = default;
    [Header("レイを出す始点(壁越しさせない用)")] [Tooltip("レイを出す始点(壁越しさせない用)")]
    [SerializeField] private GameObject _rayOrigin = default;
    [Header("走る速度")] [Tooltip("走る速度")]
    [SerializeField] private float _runningSpeed = 2f;
    [Header("追いかけ始める距離")] [Tooltip("追いかけ始める距離")]
    [SerializeField] private float _trackingStartDistance = 10f;
    [Header("職質を開始する距離")] [Tooltip("職質を開始する距離")] 
    [SerializeField] private float _checkupStartDistance = 2f;
    
    [Header("職質を継続する時間")] [Tooltip("職質を継続する時間")] 
    [SerializeField] private float _checkupTime = 5f;
    private float _checkupTimer = default;
    private bool _isCheckupTime = default;
   
    [Tooltip("追跡対象")] private GameObject _target = default;
    private bool _toTracking = default;
    private PlayerMoveController _playerMoveController = default;
    private PlayerItemController _playerItemController = default;
    private bool _canTrackingAndCheckup = default;
   
    [Header("職質終了後のインターバル")][Tooltip("職質終了後のインターバル")] 
    [SerializeField] private float _interval = 10f;
    private float _intervalTimer = default;
    [Tooltip("インターバルの計算をするか")] private bool _isInterval = default;
    [Tooltip("追跡するか")] private bool _isTrack = default;
    [Tooltip("職質するか")] private bool _isCheckup = default;
    
    #endregion

    #region プロパティ

    /// <summary> 走る速度 </summary>
    public float RunningSpeed => _runningSpeed;

    /// <summary> 追いかけ始める距離 </summary>
    public float TrackingStartDistance => _trackingStartDistance;

    /// <summary> 職質を開始する距離 </summary>
    public float CheckupStartDistance => _checkupStartDistance;

    /// <summary> 職質を継続する時間 </summary>
    public float CheckupTime => _checkupTime;

    /// <summary> 計測用 </summary>
    public float CheckupTimer { get => _checkupTimer; set => _checkupTimer = value; }
    
    /// <summary> 追跡対象 </summary>
    public GameObject Target => _target;

    /// <summary> 追跡と職質ができるか </summary>
    public bool CanTrackingAndCheckup { get => _canTrackingAndCheckup; set => _canTrackingAndCheckup = value; }
    
    /// <summary> インターバルの計算をするか </summary>
    public bool IsInterval { get => _isInterval; set => _isInterval = value; }
    
    /// <summary> 追跡するか </summary>
    public bool IsTrack => _isTrack;

    /// <summary> 職質するか </summary>
    public bool IsCheckup { get => _isCheckup; set => _isCheckup = value; }
    
    #endregion
    
    protected override void OnStart()
    {
        _policeOfficerCheckupState = new PoliceOfficerCheckupState(this);
        _playerMoveController = FindObjectOfType<PlayerMoveController>();
        _playerItemController = FindObjectOfType<PlayerItemController>();
        _canTrackingAndCheckup = true;
        _isInterval = false;
    }

    protected override void OnUpdate()
    {
        if (_isInterval)
        {
            _intervalTimer += Time.deltaTime;
            _checkupTimer = 0f;
            _isCheckupTime = false;
        }
        if (_intervalTimer >= _interval)
        {
            _canTrackingAndCheckup = true;
            _checkupTimer = 0f;
            _toTracking = false;
            if (_intervalTimer >= _interval + 1)
            {
                _isInterval = false;
                _intervalTimer = 0f;
            }
        }

        if (_intervalTimer == 0) _canTrackingAndCheckup = true;

        if (!_canTrackingAndCheckup)
        {
            Reset();
        }

        if (_isCheckupTime)
        {
            _checkupTimer += Time.deltaTime;
        }
        if(_target != null && _intervalTimer == 0) CheckDistance();
    }

    /// <summary>
    /// 追跡を開始するか
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
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
                    _target = other.gameObject;
                    if (!_toTracking)
                    {
                        NpcStateMachine.ChangeState(_policeOfficerCheckupState);
                        _toTracking = true;
                    }
                }
                else
                {
                    _target = null;
                }
            }
        }
    }

    /// <summary>
    /// プレイヤーがコライダー（視野）外に行ったら、追跡条件を偽にする
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("RecordedPlayer"))
        {
            _toTracking = false;
            _target = null;
            _canTrackingAndCheckup = false;
        }
    }

    private void Reset()
    {
        _isTrack = false;
        _isCheckup = false;
        _checkupTimer = 0;
        _isCheckupTime = false;
    }

    /// <summary>
    /// 距離に応じて追跡・職質のフラグを切り替える
    /// </summary>
    private void CheckDistance()
    {
        float distance = (transform.position 
                          - Target.transform.position).sqrMagnitude;
        var cSDistance = CheckupStartDistance;
        var tSDistance = TrackingStartDistance;
        // Debug.Log("distance : " + distance + "  cSDistance : " + cSDistance*cSDistance
        // + "  tSDistance : " + tSDistance*tSDistance);
        if (distance <= cSDistance * cSDistance)
        {
            _isCheckup = true;
            _isTrack = false;
            _isCheckupTime = true;
        }
        else if (distance <= tSDistance * tSDistance)
        {
            _isCheckup = false;
            _isTrack = true;
            _isCheckupTime = false;
        }
    }
    
    /// <summary>
    /// プレイヤーの機能を制御する
    /// </summary>
    /// <param name="n">0:Move 1:Item 2:両方</param>
    /// <param name="flag"></param>
    public void ChangeEnabled(int n, bool flag)
    {
        if (n == 0)
        {
            _playerMoveController.enabled = flag;
        }
        else if (n == 1)
        {
            _playerItemController.enabled = flag;
        }
        else if (n == 2)
        {
            _playerMoveController.enabled = flag;
            _playerItemController.enabled = flag;
        }
        else
            Debug.LogWarning("第一引数が間違っています。");
    }
}


#region ステート機能

/// <summary>
/// プレイヤーとの距離に応じて処理を変える
/// </summary>
public class PoliceOfficerCheckupState : StateBase
{
    private PoliceCheckup _policeCheckup;
    private float _speedValue;
    public PoliceOfficerCheckupState(PoliceCheckup owner) : base(owner)
    {
        _policeCheckup = owner;
    }

    public override void Enter()
    {
        _policeCheckup.NavMeshAgent.isStopped = false;
        // Debug.Log("Enter: PoliceOfficerCheckupState state");
    }

    public override void Update()
    {
        // 追跡対象がいないとき
        if (_policeCheckup.Target == null && _policeCheckup.CheckupTimer == 0)
        {
            _policeOfficer.IsTimer = true;
            _policeCheckup.NavMeshAgent.isStopped = true;
            return;
        }

        if (_policeCheckup.IsCheckup)
        {
            CheckUp();
        }
        else if (_policeCheckup.IsTrack)
        {
            Tracking();
        }

        if (_policeOfficer.NavMeshAgent.isStopped)
        {
            _policeOfficer.Anim.SetFloat("Speed", 0f);
        }

        // Debug.Log("Update: PoliceOfficerCheckupState state");
    }

    public override void Exit()
    {
        // 無効化
        _policeCheckup.NavMeshAgent.isStopped = true;
        if (_policeOfficer.Anim)
        {
            _policeOfficer.NavMeshAgent.speed = _policeOfficer.Speed; // 歩き速度
            // _policeOfficer.Anim.SetFloat("Speed", _policeOfficer.NavMeshAgent.speed, 1.5f, Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("アニメーターが設定されていません");
        }
        // Debug.Log("Exit : PoliceOfficerCheckupState state");
    }

    /// <summary>
    /// 追跡
    /// </summary>
    private void Tracking()
    {
        if (_policeCheckup.Target == null)
        {
            _policeCheckup.IsTimer = true;
            return;
        }
        _policeOfficer.NavMeshAgent.isStopped = false;
        if (_policeOfficer.NavMeshAgent.speed < _policeCheckup.RunningSpeed)
        {
            // 徐々に速度上昇
            _speedValue += Time.deltaTime * 0.8f;
            _policeOfficer.NavMeshAgent.speed = _speedValue;
        }
        else if (_policeOfficer.NavMeshAgent.speed > _policeCheckup.RunningSpeed)
        {
            // 超えたらジャストに設定
            _policeOfficer.NavMeshAgent.speed = _policeCheckup.RunningSpeed;
        }
        _policeOfficer.Anim.SetFloat("Speed", _policeOfficer.NavMeshAgent.speed, 0.7f, Time.deltaTime);
        
        if (_policeCheckup.Target != null)
        {
            _policeOfficer.NavMeshAgent.SetDestination(_policeCheckup.Target.transform.position);
        }
    }
    
    /// <summary>
    /// 職質機能
    /// プレイヤーの行動制限をする
    /// ・アイテム ・ムーブ
    /// </summary>
    private void CheckUp()
    {
        if (_policeCheckup.CheckupTimer > _policeCheckup.CheckupTime)
        {
            // 職質終了
            _policeCheckup.ChangeEnabled(2, true);
            _policeCheckup.CanTrackingAndCheckup = false;
            _policeCheckup.IsCheckup = false;
            _policeCheckup.IsInterval = true;
            _policeOfficer.NavMeshAgent.isStopped = false;
            Debug.Log("職質終了");
        }
        else
        {
            // 制限
            _policeCheckup.ChangeEnabled(2, false);
            _policeOfficer.NavMeshAgent.isStopped = true;
        }
    }
}

#endregion