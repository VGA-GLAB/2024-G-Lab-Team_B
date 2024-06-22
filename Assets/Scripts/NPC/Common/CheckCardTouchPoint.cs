using System.Collections;
using System.Collections.Generic;
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
    [Header("カードをかざすポイントを見つけたか"), Tooltip("カードをかざすポイントを見つけたか")]
    [SerializeField] private bool _isCardTouchPoint = default;
    [Header("かざすアニメーションが終わるまでの待機時間"), Tooltip("かざすアニメーションが終わるまでの待機時間")]
    [SerializeField] private float _waitTime = 7f;
    private PatrolNPC _patrolNpc = default;
    private float _timer = default; 
    private PatrolNPC[] _patrolNpcs = default; 
    private List<IDoor> _iDoors = new List<IDoor>();
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Open = Animator.StringToHash("Open");

    private void Start()
    {
        _isCardTouchPoint = false;
        _patrolNpcs = GetComponents<PatrolNPC>();
    }

    private void Update()
    {
        if(!_isCardTouchPoint) return;
        if (_isCardTouchPoint && _patrolNpc.NavMeshAgent.remainingDistance <= 0.05f)
        {
            _patrolNpc.NavMeshAgent.isStopped = true;
            _patrolNpc.Anim.SetFloat(Speed, 0);
            _patrolNpc.enabled = false;
        }
        
        if(_patrolNpc == null) return;
        
        if (_timer >= _waitTime)
        {
            foreach (var item in _iDoors)
            {
                item.OpenDoor();
            }
            _patrolNpc.enabled = true;
            _isCardTouchPoint = false;
            _patrolNpc = null;
            _timer = 0f;
            StartCoroutine(LateClose());
        }

        _timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("CardTouchPoint")) return;
        _iDoors.Clear();
        _patrolNpc = GetActivePatrolNpc();
        _isCardTouchPoint = true;
        _patrolNpc.IsTimer = true;
        _patrolNpc.Anim.SetTrigger(Open);
        var ctp = other.gameObject.GetComponent<CardTouchPoint>();
        // ドアに付いているインターフェースを取得
        foreach (var item in ctp.Doors)
        {
            _iDoors.Add(item);
        }
    }

    /// <summary>
    /// アクティブなPatrolNpcだけ取得
    /// </summary>
    private PatrolNPC GetActivePatrolNpc()
    {
        foreach (var item in _patrolNpcs)
        {
            if (item.enabled) return item;
        }
        Debug.Log("アクティブなPatrolNPCがありません。");
        return null;
    }

    private IEnumerator LateClose() 
    {
        yield return new WaitForSeconds(5);
        foreach (var item in _iDoors)
        {
            item.CloseDoor();
        }
    }
}