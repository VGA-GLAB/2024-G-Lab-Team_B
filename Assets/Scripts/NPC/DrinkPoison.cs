using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 指定した残り時間になったときに、特定の行動をする
/// 毒を飲んで死亡するモーションをするオブジェクトを生成する
/// ※飲んでから死ぬまでの流れは、生成したオブジェクトが行う
/// </summary>
public class DrinkPoison : MonoBehaviour, ICanDead
{
    #region 変数
   
    [Header("残り時間を持つクラス"), Tooltip("残り時間を持つクラス")]
    private CountDownTimer _countDownTimer = default;

    [Header("毒を飲んで死亡するモーションをするキャラクター"), Tooltip("毒を飲んで死亡するモーションをするキャラクター")]
    [SerializeField] private GameObject _deadCharaPrefab = default;

    [Tooltip("生成したキャラ")]
    private GameObject _character = default;

    [Header("死ぬか"), Tooltip("死ぬか")]
    [SerializeField] private bool _isDead = default;

    [Header("0:散歩-A 1:散歩-B 2:教授室へ 3:毒接種 (4:死亡)")] [Header("各行動の開始時間(例：残り時間〇秒)"), Tooltip("各行動の開始時間")]
    [SerializeField] private float[] _timeline = new float[5];

    private NavMeshAgent _navMeshAgent = default;

    [Header("PatrolNPC[0]:自由行動  [1]-[3]:(タイムラインの0-2にあたる)")]
    [Header("時間に応じて変える行動パターン"), Tooltip("時間に応じて変える行動パターン")]
    [SerializeField] private PatrolNPC[] _patrolNpcs = default;
    
    [Header("毒を飲む場所"), Tooltip("毒を飲む場所")]
    [SerializeField] private GameObject _poisonPoint = default;
    private ProfessorDeadOrAlive _professorDeadOrAlive = default;
    [Header("薬"), Tooltip("薬")]
    [SerializeField] private GameObject _medicine = default;
    private bool _canDrinkMedicine = default;

    #endregion
    
    /// <summary> 死ぬか </summary>
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }

    void Start()
    {
        ChangeEnabledToFalse(0);
        _patrolNpcs[0].enabled = true;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _countDownTimer = FindObjectOfType<CountDownTimer>();
    }

    void Update()
    {
        if (_countDownTimer.Timer > _timeline[0])
        {
            return;
        }
        if (_countDownTimer.Timer <= 0)
        {
            return;
        }

        TimeLineMove();
    }

    /// <summary>
    /// タイムラインに合わせて行動する
    /// </summary>
    void TimeLineMove()
    {
        var remainingTime = _countDownTimer.Timer;
        if (remainingTime < _timeline[3])
        {
            // ４. 教授は、教授部屋に常備している薬（降圧薬？胃薬？）を飲む。
            ChangeEnabledToFalse(100);
            if (!_character)
            {
                _character = Instantiate(_deadCharaPrefab, transform.position, transform.rotation);
            }
            if (!_professorDeadOrAlive)
            {
                _professorDeadOrAlive = FindObjectOfType<ProfessorDeadOrAlive>();
                // Todo: _isDeadフラグを適用する場所
                _professorDeadOrAlive.IsDead = _isDead;
                _professorDeadOrAlive.Medicine = _medicine;
                gameObject.SetActive(false);
            }
        }
        else if (remainingTime < _timeline[2])
        {
            // Todo: 外部から_isDeadフラグを切り替える場合、この時点で決定しておく必要がある (Poisonタグを使わない場合)
            // ３. 薬を飲むために、教授部屋に戻る。
            if (_patrolNpcs[3].enabled != true)
            {
                ChangeEnabledToFalse(3);
                _patrolNpcs[3].enabled = true;
            }
            // 毒のある場所に着いたら巡回を停止する
            var distance = (transform.position - _poisonPoint.transform.position).sqrMagnitude;
            if (distance <= 0.2f)
            {
                // 巡回を停止し、その場で待機
                _navMeshAgent.isStopped = true;
                _patrolNpcs[3].Anim.SetFloat("Speed", 0);
                ChangeEnabledToFalse(100);
                _canDrinkMedicine = true; // 薬のタグ判定を許可
            }
        }
        else if (remainingTime < _timeline[1])
        {
            // ２. 吹き抜けラウンジを歩いたり、他の部屋へ遊びに行く。
            if (_patrolNpcs[2].enabled != true)
            {
                ChangeEnabledToFalse(2);
                _patrolNpcs[2].enabled = true;
            }
        }
        else if (remainingTime < _timeline[0])
        {
            // １. 教授は、教授部屋から外に出て、施設内をお散歩する。
            if (_patrolNpcs[1].enabled != true)
            {
                ChangeEnabledToFalse(1);
                _patrolNpcs[1].enabled = true;
            }
        }
    }

    /// <summary>
    /// 使わないPatrolNPCの機能を停止する
    /// </summary>
    /// <param name="num"></param>
    void ChangeEnabledToFalse(int num)
    {
        for (int i = 0; i < _patrolNpcs.Length; i++)
        {
            if (i != num)
            {
                _patrolNpcs[i].enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Poisonタグならば、IsDead
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (_canDrinkMedicine)
        {
            if (other.gameObject == _medicine)
            {
                if (other.CompareTag("Poison"))
                {
                    _isDead = true;
                }
                else
                {
                    _isDead = false;
                }
            }
        }
    }
}