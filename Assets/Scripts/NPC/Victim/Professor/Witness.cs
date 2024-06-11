using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 名誉教授の部屋で名誉教授がプレイヤーを発見したら、ステージを退場する。
/// ※フラグを記録する際にアクティブ状態でなければならない(RecordsDataSaver)。(死亡プレハブ)
/// 死亡キャラが生成させるまでは巡回などを制限し、見えないところに置いておく。
/// </summary>
public class Witness : MonoBehaviour
{
    #region 変数
    
    [Header("名誉教授室の中にある方のOffMeshLinkポイント")] [Tooltip("名誉教授室の中にある方のOffMeshLinkポイント")]
    [SerializeField] private GameObject _insidePoint = default;
    [Tooltip("名誉教授室の内か")] private bool _isInside = default;
    [Header("退場するために向かう場所"), Tooltip("退場するために向かう場所")] 
    [SerializeField] private GameObject _exitPosition = default;
    [Tooltip("移動速度"), SerializeField] private float _speed = 3f;
    private NavMeshAgent _navMeshAgent = default;
    private bool _toTargetPosition = default;
    private Vector3 _exitPos = default;
    private DrinkPoison _drinkPoison = default;
    private float _time = default;
    private CountDownTimer _countDownTimer = default;
    private ProfessorDeadOrAlive _professorDeadOrAlive = default;
    private GameObject _deadChara = default;
    private Animator _animator = default;
    private bool _isWitness = default;
    
    #endregion
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.isStopped = false;
        if(!_insidePoint) Debug.LogWarning("名誉教授室の中にある方のOffMeshLinkポイント(InsidePoint)をアサインしてください。");
        if(!_exitPosition) Debug.LogWarning("名誉教授の退場先(ExitPosition)をアサインしてください。");
        _exitPos = _exitPosition.transform.position;
        _drinkPoison = GetComponent<DrinkPoison>();
        _time = _drinkPoison.Timeline[_drinkPoison.Timeline.Length - 1];
        _countDownTimer = FindObjectOfType<CountDownTimer>();
    }

    void Update()
    {
        if(_countDownTimer.Timer < 0) return;
        if (_isWitness)
        {
            if(_drinkPoison.enabled) _drinkPoison.enabled = false; // フラグ変更不可にする

            // 死亡プレハブを生成する時間以下になったらとき
            if (_time > _countDownTimer.Timer)
            {
                if(!_deadChara) _deadChara = Instantiate(_drinkPoison.DeadCharaPrefab, transform.position, transform.rotation);
                if (_professorDeadOrAlive == null)
                {
                    _professorDeadOrAlive = FindFirstObjectByType<ProfessorDeadOrAlive>();
                }
                else
                {
                    _professorDeadOrAlive.IsDead = true;
                    _professorDeadOrAlive.Medicine = null;
                }
            }
        }
        
        // (1)ステージから退場。(2)フラグが記録されるまでの間プレハブを隠す。
        if(_toTargetPosition)
        {
            _drinkPoison.enabled = false;
            _navMeshAgent.SetDestination(_exitPos);
            float distance = (transform.position - _exitPos).sqrMagnitude;
            if (distance < 0.1f)
            {
                _navMeshAgent.enabled = false;
                _toTargetPosition = false;
                var hidePos = new Vector3(0, 100, 0);
                transform.position = hidePos;
                _animator.SetFloat("Speed", 0);
            }
        }
        
        // 以下、部屋の中でプレイヤーを見つけたかどうかの判定
        if (!_navMeshAgent.isOnOffMeshLink) return; // リンク上にいないとき
        var endPos = _navMeshAgent.currentOffMeshLinkData.endPos;
        if (endPos != _insidePoint.transform.position) // 教授室側へ向かっていないとき
        {
            _isInside = false;
            return;
        }
        _isInside = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TransparentPlayer")) return;
        if (other.CompareTag("Player") || other.CompareTag("RecordedPlayer"))
        {
            if (_isInside)
            {
                _toTargetPosition = true;
                _isWitness = true;
                _navMeshAgent.speed = _speed;
                _drinkPoison.ChangeEnabledToFalse(100); // すべて偽にする
                Debug.Log("教授室内でプレイヤーを発見");
            }
        }
    }
}
