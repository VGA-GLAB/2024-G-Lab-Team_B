using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 被害者の基底クラス
/// ※死亡場所に着く前に死亡時刻になると、その場で死亡する
/// </summary>
public class VictimBase : MonoBehaviour, ICanDead
{
    #region 変数
    
    [Header("残り時間を持つクラス"), Tooltip("残り時間を持つクラス")]
    protected CountDownTimer _countDownTimer = default;
    [Header("死亡するモーションをするキャラクター"), Tooltip("死亡するモーションをするキャラクター")]
    [SerializeField] protected GameObject _deadCharaPrefab = default;
    [Tooltip("生成したキャラ")] protected GameObject _character = default;
    [Header("死ぬか"), Tooltip("死ぬか")]
    [SerializeField] protected bool _isDead = default;
    [Header("各行動の開始時間(例：残り時間〇秒)"), Tooltip("各行動の開始時間")]
    [SerializeField] protected float[] _timeline = new float[5];
    protected NavMeshAgent _navMeshAgent = default;
    [Header("PatrolNPC[0]:自由行動  [1]-:(タイムラインの0-にあたる)")]
    [Header("時間に応じて変える行動パターン"), Tooltip("時間に応じて変える行動パターン")]
    [SerializeField] protected PatrolNPC[] _patrolNpcs = default;
    [Header("死ぬ場所"), Tooltip("死ぬ場所")]
    [SerializeField] protected GameObject _deadPoint = default;
    
    #endregion
    
    /// <summary> 死ぬか </summary>
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }

    protected virtual void OnStart() {}
    protected virtual void OnUpdate() {}
    
    void Start()
    {
        ChangeEnabledToFalse(0);
        _patrolNpcs[0].enabled = true;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _countDownTimer = FindObjectOfType<CountDownTimer>();
        OnStart();
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
        OnUpdate();
    }
    
    /// <summary>
    /// 使わないPatrolNPCの機能を停止する
    /// </summary>
    /// <param name="num"></param>
    protected void ChangeEnabledToFalse(int num)
    {
        for (int i = 0; i < _patrolNpcs.Length; i++)
        {
            if (i != num)
            {
                _patrolNpcs[i].enabled = false;
            }
        }
    }
}
