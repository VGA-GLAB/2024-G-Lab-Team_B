using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// リストに入れた場所で待機アニメーションを再生する
/// ※タイムラインに沿った行動中は使用不可(4/22時点)
/// 使えるとき：PatrolNPCスクリプトが1つだけアタッチされているもの
/// </summary>
public class InsertWaiting : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent = default;
    [Header("到達したと見做す距離"), SerializeField] private float _distance = 0.5f;
    private PatrolNPC _patrolNpc = default;
    [Tooltip("配列の何番目を見るか")] private int _num = 0;
    [Header("立ち止まる地点"), SerializeField] private GameObject[] _positions = default;
    [Header("立ち止まる時間"), SerializeField] private int[] _waitTime = default;
    private float _timer = default;
    private Animator _animator = default;
    private float _defaultSpeed = default; 
    private float _resetTimer = default;
    private bool _canMove = false;
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _patrolNpc = GetComponent<PatrolNPC>();
        _animator = GetComponent<Animator>();
        _defaultSpeed = _navMeshAgent.speed;
        _canMove = true;
    }

    void Update()
    {
        if(_positions.Length == 0) return; // 待機場所の指定がないとき
        if (!_canMove)
        {
            // 配列の長さが１のときに、ずっと待機してしまわないように時間制限
            _resetTimer += Time.deltaTime;
            if (_resetTimer > 3f)
            {
                _canMove = true;
                _resetTimer = 0;
            }
        }
        if (_navMeshAgent.remainingDistance > _distance) return; // 到達していないとき

        if (_canMove)
        {
            var dis = (transform.position - _positions[_num].transform.position).sqrMagnitude;
            if (dis <= _distance * _distance) // 到達後 場所が一致する時
            {
                _timer += Time.deltaTime;
                if (_patrolNpc.enabled) // 止まってなかったとき
                {
                    _patrolNpc.enabled = false;
                    _animator.SetFloat("Speed", 0);
                }
            }

            if (_timer >= _waitTime[_num]) // 待機時間を過ぎたとき
            {
                _patrolNpc.enabled = true;
                _animator.SetFloat("Speed", _defaultSpeed);
                _num++;
                _timer = 0f;
            }
        }
        
        if (_num == _positions.Length)
        {
            if (_positions.Length == 1)
            {
                _canMove = false;
            }
            _num = 0;
        }
    }
}
