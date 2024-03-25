using UnityEngine;

/// <summary>
/// IsDeadのフラグが偽なら、その場で待機する
/// </summary>
public class ProfessorDeadOrAlive : MonoBehaviour
{
    [Header("死ぬか"), Tooltip("死ぬか")]
    [SerializeField] private bool _isDead = default; 
    private Animator _animator = default;
    [Header("何秒後に死ぬか"), Tooltip("何秒後に死ぬか")]
    [SerializeField] private float _timeToDead = 10f;
    [Header("薬"), Tooltip("薬")]
    [SerializeField] private GameObject _medicine = default;
    private bool _canPlay = false;

    /// <summary> 死ぬか </summary>
    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }
    /// <summary> 薬 </summary>
    public GameObject Medicine
    {
        get => _medicine;
        set => _medicine = value;
    }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        _canPlay = true;
        Vector3 targetPos = _medicine.transform.position;
        // ターゲットのY座標を自分と同じにすることで2次元に制限する。
        targetPos.y = transform.position.y;
        transform.LookAt(targetPos);
        _medicine.SetActive(false);
    }

    private void Update()
    {
        if (_canPlay)
        {
            if (_isDead)
            {
                PlayDeadAnim();
            }
        }
    }

    /// <summary>
    /// 死亡モーションを再生する
    /// </summary>
    void PlayDeadAnim()
    {
        _timeToDead -= Time.deltaTime;
        if (_timeToDead <= 0)
        {
            _animator.Play("Unwell"); // ふらつく → 倒れる
            _canPlay = false;
        }
    }
}
