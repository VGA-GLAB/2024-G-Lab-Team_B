using UnityEngine;
/// <summary>
/// 死亡モーションをするオブジェクトの基底クラス
/// </summary>
public class DeadOrAliveBase : MonoBehaviour, ICanDead
{
    #region 変数

    [Header("死ぬか"), Tooltip("死ぬか")]
    [SerializeField] protected bool _isDead = default; 
    protected Animator _animator = default;
    [Header("何秒後に再生するか"), Tooltip("何秒後に再生するか")]
    [SerializeField] protected float _timeToPlay = 10f;
    protected bool _canPlay = false;

    #endregion
    
    /// <summary> 死ぬか </summary>
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }
    
    protected virtual void OnStart(){}
    protected virtual void OnUpdate(){}
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        OnStart();
    }

    void Update()
    {
        OnUpdate();
    }
    
    /// <summary>
    /// 数秒後にモーションを再生する
    /// </summary>
    protected void LatePlayAnim(string stateName)
    {
        _timeToPlay -= Time.deltaTime;
        if (_timeToPlay <= 0)
        {
            _animator.Play(stateName);
            _canPlay = false;
        }
    }
}
