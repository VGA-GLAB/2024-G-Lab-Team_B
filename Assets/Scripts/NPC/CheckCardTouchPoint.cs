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

    private void Start()
    {
        _patrolNpc = GetComponent<PatrolNPC>();
        _isCardTouchPoint = false;
    }

    void Update()
    {
        if (_patrolNpc.NavMeshAgent.remainingDistance <= 0.05f && _isCardTouchPoint)
        {
            _patrolNpc.NavMeshAgent.isStopped = true;
            _patrolNpc.Anim.SetTrigger("Open"); 
            _isCardTouchPoint = false;
            _patrolNpc.enabled = true;
            _patrolNpc.IsTimer = true; // _patrolNpcで設定されたIdle時間だけ待機する
            // Debug.Log("カードをかざす！！");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CardTouchPoint"))
        {
            _patrolNpc.enabled = false;
            _patrolNpc.NavMeshAgent.SetDestination(other.transform.position);
            _isCardTouchPoint = true;
            // Debug.Log("発見");
        }
    }
}