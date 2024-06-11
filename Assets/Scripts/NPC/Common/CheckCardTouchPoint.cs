using UnityEngine;

/// <summary>
/// カードをかざすポイントか調べる
/// カードをかざすポイントに触れたら、そこを目的地とする
/// 着いたらカードをかざすモーションをする
/// 
/// 機能する条件(1)：カードをかざすポイントにコライダーが付いている
/// 機能する条件(2)：タグがCardTouchPointである
/// </summary>
public class CheckCardTouchPoint : MonoBehaviour
{ 
    private PatrolNPC _patrolNpc = default;
    [Header("カードをかざすポイントを見つけたか"), Tooltip("カードをかざすポイントを見つけたか")]
    [SerializeField] private bool _isCardTouchPoint = default;
    [Header("かざすアニメーションが終わるまでの待機時間"), Tooltip("かざすアニメーションが終わるまでの待機時間")]
    [SerializeField] private float _waitTime = 7f;
    private float _timer = default; 
    private PatrolNPC[] _patrolNpcs = default; 
    private IDoor _iDoor = default;

    private void Start()
    {
        _isCardTouchPoint = false;
        _patrolNpcs = GetComponents<PatrolNPC>();
    }

    void Update()
    {
        if(!_isCardTouchPoint) return;
        if (_isCardTouchPoint && _patrolNpc.NavMeshAgent.remainingDistance <= 0.05f)
        {
            _patrolNpc.NavMeshAgent.isStopped = true;
            _patrolNpc.Anim.SetFloat("Speed", 0);
            _patrolNpc.enabled = false;
        }
        
        if(_patrolNpc == null) return;
        if (_timer >= _waitTime)
        {
            _patrolNpc.enabled = true;
            _isCardTouchPoint = false;
            _patrolNpc = null;
            _timer = 0f;
            _iDoor.OpenDoor(); // ドアを開く
        }

        _timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CardTouchPoint")) return;
        _patrolNpc = GetActivePatrolNpc();
        _isCardTouchPoint = true;
        _patrolNpc.IsTimer = true;
        _patrolNpc.Anim.SetTrigger("Open");
        _iDoor = GetComponent<IDoor>(); // ドアに付いているインターフェースを取得
    }

    /// <summary>
    /// アクティブなPatrolNpcだけ取得
    /// </summary>
    PatrolNPC GetActivePatrolNpc()
    {
        foreach (var item in _patrolNpcs)
        {
            if (item.enabled) return item;
        }
        Debug.Log("アクティブなPatrolNPCがありません。");
        return null;
    }
}